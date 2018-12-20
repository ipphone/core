using System;
using System.IO;
using AudioLibrary.WMME.Native;

namespace AudioLibrary.WMME
{
    internal class WaveStream : Stream, IDisposable
    {
        private Stream m_Stream;
        private long m_DataPos;
        private long m_Length;

        public WAVEFORMATEX Format { get; private set; }

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

            Format = new WAVEFORMATEX(22050, 16, 2)
            {
                formatTag = (WaveFormatTag)Reader.ReadInt16(),
                nChannels = Reader.ReadInt16(),
                nSamplesPerSec = Reader.ReadInt32(),
                nAvgBytesPerSec = Reader.ReadInt32(),
                nBlockAlign = Reader.ReadInt16(),
                wBitsPerSample = Reader.ReadInt16()
            }; // initialize to any format

            // advance in the stream to skip the wave format block 
            len -= 16; // minimum format size
            while (len > 0)
            {
                Reader.ReadByte();
                len--;
            }

            // assume the data chunk is aligned
            while (m_Stream.Position < m_Stream.Length && ReadChunk(Reader) != "data") ;

            if (m_Stream.Position >= m_Stream.Length)
                throw new Exception("Invalid file format");

            m_Length = Reader.ReadInt32();
            m_DataPos = m_Stream.Position;

            Position = 0;
        }

        public WaveStream(Stream stream)
        {
            m_Stream = stream;
            ReadHeader();
        }

        public override bool CanRead => true;
        public override bool CanSeek => true;
        public override bool CanWrite => false;
        public override long Length => m_Length;
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
        { }

        public override void SetLength(long value)
        {
            throw new InvalidOperationException();
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            switch (origin)
            {
                case SeekOrigin.Begin:
                    m_Stream.Position = offset + m_DataPos;
                    break;
                case SeekOrigin.Current:
                    m_Stream.Seek(offset, SeekOrigin.Current);
                    break;
                case SeekOrigin.End:
                    m_Stream.Position = m_DataPos + m_Length - offset;
                    break;
            }
            return this.Position;
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            int toread = (int)Math.Min(count, m_Length - Position);
            return m_Stream.Read(buffer, offset, toread);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new InvalidOperationException();
        }
    }
}
