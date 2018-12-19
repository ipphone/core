using System.Threading.Tasks;
using Sipek.Common;
using System.Runtime.InteropServices;

namespace Sipek.Sip
{
    internal class pjsipMediaPlayerProxy : IMediaProxyInterface
    {
        private readonly object _lockObj = new object();

#if LINUX
		internal const string PJSIP_DLL = "libpjsipDll.so"; 
#elif MOBILE
		internal const string PJSIP_DLL = "pjsipdll_mobile.dll"; 
#elif TLS
		internal const string PJSIP_DLL = "pjsipdll_tls.dll"; 
#else
        internal const string PJSIP_DLL = "pjsipDll.dll";
#endif

        [DllImport(PJSIP_DLL, EntryPoint = "dll_playWav", CallingConvention = CallingConvention.Cdecl)]
        private static extern int dll_playWav(string wavFile, bool loopFile, int callId);
        [DllImport(PJSIP_DLL, EntryPoint = "dll_releaseWav", CallingConvention = CallingConvention.Cdecl)]
        private static extern bool dll_releaseWav(int playerId);

        private volatile bool _isPlaying = false;
        private int _sessionId = -1;
        private int _playerId = -1;

        public int SessionId
        {
            get { return _sessionId; }
            set 
            {
                lock (_lockObj)
                {
                    if (_isPlaying) Task.Factory.StartNew(() => stopTone(), TaskCreationOptions.PreferFairness);

                    _sessionId = value;
                }
            }
        }

        public int playTone(ETones toneId)
        {
            lock (_lockObj)
            {
                if (_isPlaying)
                {
                    //Task.Factory.StartNew(() => stopTone(), TaskCreationOptions.PreferFairness);
                    _isPlaying = false;

                    CommonDelegates.SafeInvoke(() => dll_releaseWav(_playerId));
                }

                if (_sessionId >= 0)
                {
                    _playerId = CommonDelegates.SafeInvoke(() => dll_playWav(GetToneFile(toneId), true, _sessionId));

                    if (_playerId >= 0)
                        _isPlaying = true;
                }

                return _playerId;
            }
        }

        public int stopTone()
        {
            lock (_lockObj)
            {
                if (!_isPlaying) return 0;

                _isPlaying = false;

                CommonDelegates.SafeInvoke(() => dll_releaseWav(_playerId));
            }

            return 0;
        }

        private string GetToneFile(ETones toneId)
        {
            switch (toneId)
            {
                case ETones.EToneDial:
                    return "Sounds/dial.wav";
                case ETones.EToneCongestion:
                    return "Sounds/congestion.wav";
                case ETones.EToneRingback:
                    return "Sounds/ringback.wav";
                case ETones.EToneRing:
                    return "Sounds/ring.wav";
                default:
                    return "";
            }
        }
    }
}
