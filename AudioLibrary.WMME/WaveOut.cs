using System;
using System.Runtime.InteropServices;
using System.Threading;
using AudioLibrary.WMME.Native;

namespace AudioLibrary.WMME
{
    internal class WaveOut : IDisposable
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
}
