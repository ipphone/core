using System;
using System.Runtime.InteropServices;
using System.Threading;
using AudioLibrary.WMME.Native;

namespace AudioLibrary.WMME
{
    internal class WaveOutBuffer : IDisposable
    {
        private readonly object _lockObj = new object();

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
            lock (_lockObj)
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
}
