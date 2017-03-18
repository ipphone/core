using System;
using System.Runtime.InteropServices;
using System.IO;
using AudioLibrary.Interfaces;
using System.Threading;
using AudioLibrary.WMME.Native;

namespace AudioLibrary.WMME
{
    internal class AudioPlayer : IAudioPlayer
    {
        private WaveOut _player;
        private WAVEFORMATEX _format;
        private Stream _stream;
        private AudioDevice _device;

        public AudioPlayer(AudioDevice device)
        {
            _device = device;
        }

        public void Play(string fileName)
        {
            CloseFile();

            try
            {
                WaveStream stream = new WaveStream(fileName);
                if (stream.Length <= 0)
                    throw new Exception("Invalid WAV file");

                _format = stream.Format;
                if (_format.formatTag != WaveFormatTag.PCM && _format.formatTag != WaveFormatTag.IEEE_FLOAT)
                    throw new Exception("Olny PCM files are supported");

                _stream = stream;
            }
            catch
            {
                CloseFile();
            }

            if (_stream != null)
            {
                _stream.Position = 0;
                _player = new WaveOut(_device.DeviceId, _format, 16384, 3, new BufferFillEventHandler(Filler));
            }
        }

        public void Stop()
        {
            if (_player != null)
                try
                {
                    _player.Dispose();
                }
                finally
                {
                    _player = null;
                }
        }

        private void CloseFile()
        {
            Stop();

            if (_stream != null)
                try
                {
                    _stream.Close();
                }
                finally
                {
                    _stream = null;
                }
        }

        private void Filler(IntPtr data, int size)
        {
            byte[] b = new byte[size];
            if (_stream != null)
            {
                int pos = 0;
                while (pos < size)
                {
                    int toget = size - pos;
                    int got = _stream.Read(b, pos, toget);
                    if (got < toget)
                        _stream.Position = 0; // loop if the file ends
                    pos += got;
                }
            }
            else
            {
                for (int i = 0; i < b.Length; i++)
                    b[i] = 0;
            }
            System.Runtime.InteropServices.Marshal.Copy(b, 0, data, size);
        }

        public void Dispose()
        {
            CloseFile();
        }
    }

    internal delegate void BufferFillEventHandler(IntPtr data, int size);

    internal class WaveOut
    {
        private IntPtr m_WaveOut;
        private WaveOutBuffer m_Buffers; // linked list
        private WaveOutBuffer m_CurrentBuffer;
        private Thread m_Thread;
        private BufferFillEventHandler m_FillProc;
		private bool m_Finished;
		private byte m_zero;

        private WaveNative.WaveDelegate m_BufferProc = new WaveNative.WaveDelegate(WaveOutBuffer.WaveOutProc);

        public WaveOut(int device, WAVEFORMATEX format, int bufferSize, int bufferCount, BufferFillEventHandler fillProc)
        {
			m_zero = format.wBitsPerSample == 8 ? (byte)128 : (byte)0;
            m_FillProc = fillProc;

            var errorCode = (MMErrors)WaveNative.waveOutOpen(out m_WaveOut, device, ref format, m_BufferProc, 0, (int)CallBackFlag.CALLBACK_FUNCTION);
            if (errorCode != MMErrors.MMSYSERR_NOERROR)
                throw new MixerException(errorCode, Audio.GetErrorDescription(FuncName.fnWaveOutOpen, errorCode));

            AllocateBuffers(bufferSize, bufferCount);
            m_Thread = new Thread(new ThreadStart(ThreadProc));
            m_Thread.Start();
        }

        ~WaveOut()
        {
            Dispose();
        }

        public void Dispose()
        {
            if (m_Thread != null)
				try
				{
					m_Finished = true;
					if (m_WaveOut != IntPtr.Zero)
						WaveNative.waveOutReset(m_WaveOut);
					m_Thread.Join();
					m_FillProc = null;
					FreeBuffers();
					if (m_WaveOut != IntPtr.Zero)
						WaveNative.waveOutClose(m_WaveOut);
				}
				finally
				{
					m_Thread = null;
					m_WaveOut = IntPtr.Zero;
				}
            GC.SuppressFinalize(this);
        }

        private void ThreadProc()
        {
            while (!m_Finished)
            {
                Advance();
				if (m_FillProc != null && !m_Finished)
					m_FillProc(m_CurrentBuffer.Data, m_CurrentBuffer.Size);
				else
				{
					// zero out buffer
					byte v = m_zero;
					byte[] b = new byte[m_CurrentBuffer.Size];
					for (int i = 0; i < b.Length; i++)
						b[i] = v;
					Marshal.Copy(b, 0, m_CurrentBuffer.Data, b.Length);

				}
                m_CurrentBuffer.Play();
            }
			WaitForAllBuffers();
		}

        #region Buffers

        private void AllocateBuffers(int bufferSize, int bufferCount)
        {
            FreeBuffers();
            if (bufferCount > 0)
            {
                m_Buffers = new WaveOutBuffer(m_WaveOut, bufferSize);
                WaveOutBuffer Prev = m_Buffers;
                try
                {
                    for (int i = 1; i < bufferCount; i++)
                    {
                        WaveOutBuffer Buf = new WaveOutBuffer(m_WaveOut, bufferSize);
                        Prev.NextBuffer = Buf;
                        Prev = Buf;
                    }
                }
                finally
                {
                    Prev.NextBuffer = m_Buffers;
                }
            }
        }

        private void FreeBuffers()
        {
            m_CurrentBuffer = null;
            if (m_Buffers != null)
            {
                WaveOutBuffer First = m_Buffers;
                m_Buffers = null;

                WaveOutBuffer Current = First;
                do
                {
                    WaveOutBuffer Next = Current.NextBuffer;
                    Current.Dispose();
                    Current = Next;
                } while(Current != First);
            }
        }

        private void Advance()
        {
            m_CurrentBuffer = m_CurrentBuffer == null ? m_Buffers : m_CurrentBuffer.NextBuffer;
            m_CurrentBuffer.WaitFor();
        }

        private void WaitForAllBuffers()
        {
            WaveOutBuffer Buf = m_Buffers;
            while (Buf.NextBuffer != m_Buffers)
            {
                Buf.WaitFor();
                Buf = Buf.NextBuffer;
            }
        }
    
        #endregion
    }

    internal class WaveOutBuffer : IDisposable
    {
        public WaveOutBuffer NextBuffer;

        private AutoResetEvent m_PlayEvent = new AutoResetEvent(false);
        private IntPtr m_WaveOut;

        private WAVEHDR m_Header;
        private byte[] m_HeaderData;
        private GCHandle m_HeaderHandle;
        private GCHandle m_HeaderDataHandle;

        private bool m_Playing;

        internal static void WaveOutProc(IntPtr hdrvr, int uMsg, int dwUser, ref WAVEHDR wavhdr, int dwParam2)
        {
            if (uMsg == WaveNative.MM_WOM_DONE)
            {
                try
                {
                    GCHandle h = (GCHandle)wavhdr.dwUser;
                    WaveOutBuffer buf = (WaveOutBuffer)h.Target;
                    buf.OnCompleted();
                }
                catch
                { }
            }
        }

        public WaveOutBuffer(IntPtr waveOutHandle, int size)
        {
            m_WaveOut = waveOutHandle;

            m_HeaderHandle = GCHandle.Alloc(m_Header, GCHandleType.Pinned);
            m_Header.dwUser = (IntPtr)GCHandle.Alloc(this);
            m_HeaderData = new byte[size];
            m_HeaderDataHandle = GCHandle.Alloc(m_HeaderData, GCHandleType.Pinned);
            m_Header.lpData = m_HeaderDataHandle.AddrOfPinnedObject();
            m_Header.dwBufferLength = size;

            var errorCode = (MMErrors)WaveNative.waveOutPrepareHeader(m_WaveOut, ref m_Header, Marshal.SizeOf(m_Header));
            if (errorCode != MMErrors.MMSYSERR_NOERROR)
                throw new MixerException(errorCode, Audio.GetErrorDescription(FuncName.fnCustom, errorCode));
        }
        ~WaveOutBuffer()
        {
            Dispose();
        }
        public void Dispose()
        {
            if (m_Header.lpData != IntPtr.Zero)
            {
                WaveNative.waveOutUnprepareHeader(m_WaveOut, ref m_Header, Marshal.SizeOf(m_Header));
                m_HeaderHandle.Free();
                m_Header.lpData = IntPtr.Zero;
            }
            m_PlayEvent.Close();
            if (m_HeaderDataHandle.IsAllocated)
                m_HeaderDataHandle.Free();
            GC.SuppressFinalize(this);
        }

        public int Size
        {
            get { return m_Header.dwBufferLength; }
        }

        public IntPtr Data
        {
            get { return m_Header.lpData; }
        }

        public bool Play()
        {
            lock (this)
            {
                m_PlayEvent.Reset();
                m_Playing = (MMErrors)WaveNative.waveOutWrite(m_WaveOut, ref m_Header, Marshal.SizeOf(m_Header)) == MMErrors.MMSYSERR_NOERROR;
                return m_Playing;
            }
        }
        public void WaitFor()
        {
            if (m_Playing)
            {
                m_Playing = m_PlayEvent.WaitOne();
            }
            else
            {
                Thread.Sleep(0);
            }
        }
        public void OnCompleted()
        {
            m_PlayEvent.Set();
            m_Playing = false;
        }
    }

	internal class WaveStream : Stream, IDisposable
	{
		private Stream m_Stream;
		private long m_DataPos;
		private long m_Length;

		private WAVEFORMATEX m_Format;

		public WAVEFORMATEX Format
		{
			get { return m_Format; }
		}

		private string ReadChunk(BinaryReader reader)
		{
			byte[] ch = new byte[4];
			reader.Read(ch, 0, ch.Length);
			return System.Text.Encoding.ASCII.GetString(ch);
		}

		private void ReadHeader()
		{
			BinaryReader Reader = new BinaryReader(m_Stream);
			if (ReadChunk(Reader) != "RIFF")
				throw new Exception("Invalid file format");

			Reader.ReadInt32(); // File length minus first 8 bytes of RIFF description, we don't use it

			if (ReadChunk(Reader) != "WAVE")
				throw new Exception("Invalid file format");

			if (ReadChunk(Reader) != "fmt ")
				throw new Exception("Invalid file format");

			int len = Reader.ReadInt32();
			if (len < 16) // bad format chunk length
				throw new Exception("Invalid file format");

			m_Format = new WAVEFORMATEX(22050, 16, 2); // initialize to any format
			m_Format.formatTag = (WaveFormatTag)Reader.ReadInt16();
			m_Format.nChannels = Reader.ReadInt16();
			m_Format.nSamplesPerSec = Reader.ReadInt32();
			m_Format.nAvgBytesPerSec = Reader.ReadInt32();
			m_Format.nBlockAlign = Reader.ReadInt16();
			m_Format.wBitsPerSample = Reader.ReadInt16(); 

			// advance in the stream to skip the wave format block 
			len -= 16; // minimum format size
			while (len > 0)
			{
				Reader.ReadByte();
				len--;
			}

			// assume the data chunk is aligned
			while(m_Stream.Position < m_Stream.Length && ReadChunk(Reader) != "data")
				;

			if (m_Stream.Position >= m_Stream.Length)
				throw new Exception("Invalid file format");

			m_Length = Reader.ReadInt32();
			m_DataPos = m_Stream.Position;

			Position = 0;
		}

		public WaveStream(string fileName) : this(new FileStream(fileName, FileMode.Open))
		{
		}
		public WaveStream(Stream S)
		{
			m_Stream = S;
			ReadHeader();
		}
		~WaveStream()
		{
			Dispose();
		}
		public void Dispose()
		{
			if (m_Stream != null)
				m_Stream.Close();
			GC.SuppressFinalize(this);
		}

		public override bool CanRead
		{
			get { return true; }
		}
		public override bool CanSeek
		{
			get { return true; }
		}
		public override bool CanWrite
		{
			get { return false; }
		}
		public override long Length
		{
			get { return m_Length; }
		}
		public override long Position
		{
			get { return m_Stream.Position - m_DataPos; }
			set { Seek(value, SeekOrigin.Begin); }
		}
		public override void Close()
		{
			Dispose();
		}
		public override void Flush()
		{
		}
		public override void SetLength(long len)
		{
			throw new InvalidOperationException();
		}
		public override long Seek(long pos, SeekOrigin o)
		{
			switch(o)
			{
				case SeekOrigin.Begin:
					m_Stream.Position = pos + m_DataPos;
					break;
				case SeekOrigin.Current:
					m_Stream.Seek(pos, SeekOrigin.Current);
					break;
				case SeekOrigin.End:
					m_Stream.Position = m_DataPos + m_Length - pos;
					break;
			}
			return this.Position;
		}
		public override int Read(byte[] buf, int ofs, int count)
		{
			int toread = (int)Math.Min(count, m_Length - Position);
			return m_Stream.Read(buf, ofs, toread);
		}
		public override void Write(byte[] buf, int ofs, int count)
		{
			throw new InvalidOperationException();
		}
	}
}
