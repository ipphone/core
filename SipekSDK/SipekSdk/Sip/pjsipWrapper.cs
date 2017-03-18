/* 
 * Copyright (C) 2007 Sasa Coh <sasacoh@gmail.com>
 *
 * This program is free software; you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation; either version 2 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA 
 * 
 * @see http://sites.google.com/site/sipekvoip
 *  
 */

using System.Runtime.InteropServices;
using System;
using System.Text;
using ContactPoint.Common;
using Sipek.Common;
using System.ComponentModel;

namespace Sipek.Sip
{
    delegate int OnDtmfDigitCallback(int callId, int digit);
    delegate int OnMessageWaitingCallback(int mwi, string info);
    delegate int OnCallReplacedCallback(int oldid, int newid);

    /// <summary>
    /// Implementation of SIP interface using pjsip.org SIP stack.
    /// This proxy is used for sip stack initialization and shut down, registration, and 
    /// callback methods handling.
    /// </summary>
    public class pjsipStackProxy : IVoipProxy
    {
        #region Constructor

        private static pjsipStackProxy _instance = null;

        public static pjsipStackProxy Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new pjsipStackProxy();
                }
                return _instance;
            }
        }

        protected pjsipStackProxy()
        {
        }
        #endregion

        #region Properties

        private bool _initialized = false;
        public override bool IsInitialized
        {
            get { return _initialized; }
            set { 
                _initialized = value;

                if (value == true)
                    BaseIsInitializedChanged();
            }
        }

        #endregion

        #region Wrapper functions

#if LINUX
	  internal const string PJSIP_DLL = "libpjsipDll.so"; 
#elif MOBILE
		internal const string PJSIP_DLL = "pjsipdll_mobile.dll"; 
#elif TLS
		internal const string PJSIP_DLL = "pjsipdll_tls.dll"; 
#else
        internal const string PJSIP_DLL = "pjsipDll.dll";
#endif

        [DllImport(PJSIP_DLL, EntryPoint = "dll_init", CallingConvention = CallingConvention.Cdecl)]
        private static extern int dll_init();
        [DllImport(PJSIP_DLL, EntryPoint = "dll_main", CallingConvention = CallingConvention.Cdecl)]
        private static extern int dll_main();
        [DllImport(PJSIP_DLL, EntryPoint = "dll_shutdown", CallingConvention = CallingConvention.Cdecl)]
        private static extern int dll_shutdown();
        [DllImport(PJSIP_DLL, EntryPoint = "dll_setSipConfig", CallingConvention = CallingConvention.Cdecl)]
        private static extern void dll_setSipConfig(SipConfigStruct config);

        [DllImport(PJSIP_DLL, EntryPoint = "dll_getCodec", CallingConvention = CallingConvention.Cdecl)]
        private static extern int dll_getCodec(int index, StringBuilder codec);
        [DllImport(PJSIP_DLL, EntryPoint = "dll_getNumOfCodecs", CallingConvention = CallingConvention.Cdecl)]
        private static extern int dll_getNumOfCodecs();
        [DllImport(PJSIP_DLL, EntryPoint = "dll_setCodecPriority", CallingConvention = CallingConvention.Cdecl)]
        private static extern int dll_setCodecPriority(string name, int prio);
        [DllImport(PJSIP_DLL, EntryPoint = "dll_setSoundDevice", CallingConvention = CallingConvention.Cdecl)]
        private static extern int dll_setSoundDevice(string playbackDeviceId, string recordingDeviceId);

        #endregion Wrapper functions

        #region Callback declarations

        [DllImport(PJSIP_DLL, EntryPoint = "onDtmfDigitCallback", CallingConvention = CallingConvention.Cdecl)]
        private static extern int onDtmfDigitCallback(OnDtmfDigitCallback cb);
        [DllImport(PJSIP_DLL, EntryPoint = "onMessageWaitingCallback", CallingConvention = CallingConvention.Cdecl)]
        private static extern int onMessageWaitingCallback(OnMessageWaitingCallback cb);
        [DllImport(PJSIP_DLL, EntryPoint = "onCallReplaced", CallingConvention = CallingConvention.Cdecl)]
        private static extern int onCallReplacedCallback(OnCallReplacedCallback cb);

        static OnDtmfDigitCallback dtdel = new OnDtmfDigitCallback(onDtmfDigitCallback);
        static OnMessageWaitingCallback mwidel = new OnMessageWaitingCallback(onMessageWaitingCallback);
        static OnCallReplacedCallback crepdel = new OnCallReplacedCallback(onCallReplacedCallback);

        #endregion

        #region Variables

        // config structure (used for special configuration options)
        public SipConfigStruct ConfigMore = SipConfigStruct.Instance;

        #endregion Variables

        #region Private Methods
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private int start()
        {
            // prepare configuration struct
            // read data from Config interface. If null read all values directly from SipConfigMore
            if (!Config.IsNull)
            {
                ConfigMore.listenPort = Config.SIPPort;
                ConfigMore.rtpPort = Config.RTPPort;
            }

            CommonDelegates.SafeInvoke(() => dll_setSipConfig(ConfigMore));

            int status = CommonDelegates.SafeInvoke(() => { return dll_init(); });
            if (status != 0) return status;

            status = CommonDelegates.SafeInvoke(() => dll_main());
            return status;
        }
        #endregion

        #region Public Methods

        /// <summary>
        /// Initialize pjsip stack
        /// </summary>
        /// <returns></returns>
        public override int initialize()
        {
            // shutdown if started already
            shutdown();

            // register callbacks (delegates)
            onDtmfDigitCallback(dtdel);
            onMessageWaitingCallback(mwidel);
            onCallReplacedCallback(crepdel);

            // init call proxy (callbacks)
            pjsipCallProxy.initialize();

            // Initialize pjsip...
            int status = start();
            // set initialized flag
            IsInitialized = (status == 0) ? true : false;

            return status;
        }

        /// <summary>
        /// Shutdown pjsip stack
        /// </summary>
        /// <returns></returns>
        public override int shutdown()
        {
            if (!IsInitialized) return -1;

            return CommonDelegates.SafeInvoke(() => dll_shutdown());
        }

        /// <summary>
        /// Get codec by index
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public override string getCodec(int index)
        {
            if (!IsInitialized) return "";

            StringBuilder codec = new StringBuilder(256);
            CommonDelegates.SafeInvoke(() => dll_getCodec(index, codec));
            return (codec.ToString());
        }

        /// <summary>
        /// Get number of all codecs
        /// </summary>
        /// <returns></returns>
        public override int getNoOfCodecs()
        {
            if (!IsInitialized) return 0;

            return CommonDelegates.SafeInvoke(() => dll_getNumOfCodecs());
        }

        /// <summary>
        /// Set codec priority
        /// </summary>
        /// <param name="codecname"></param>
        /// <param name="priority"></param>
        public override void setCodecPriority(string codecname, int priority)
        {
            if (!IsInitialized) return;

            CommonDelegates.SafeInvoke(() => dll_setCodecPriority(codecname, priority));
        }

        /// <summary>
        /// Call proxy factory method
        /// </summary>
        /// <returns></returns>
        public override ICallProxyInterface createCallProxy()
        {
            return new pjsipCallProxy(Config);
        }

        /// <summary>
        /// Set sound device for playback and recording
        /// </summary>
        /// <param name="deviceId"></param>
        public void setSoundDevice(string playbackDeviceId, string recordingDeviceId)
        {
            CommonDelegates.SafeInvoke(() => dll_setSoundDevice(playbackDeviceId, recordingDeviceId));
        }


        #endregion Methods

        #region Callbacks


        //////////////////////////////////////////////////////////////////////////////////

        private static int onDtmfDigitCallback(int callId, int digit)
        {
            Instance.BaseDtmfDigitReceived(callId, digit);
            return 1;
        }

        private static int onMessageWaitingCallback(int mwi, string info)
        {
            if (null == info) return -1;
            Instance.BaseMessageWaitingIndication(mwi, info.ToString());
            return 1;
        }

        private static int onCallReplacedCallback(int oldid, int newid)
        {
            Instance.BaseCallReplacedCallback(oldid, newid);
            return 1;
        }

        #endregion Callbacks

        #region Utility Methods
        internal string SetTransport(string sipuri)
        {
            string temp = sipuri;

            try
            {
                // set transport mode
                switch (Config.Account.TransportMode)
                {
                    case ETransportMode.TM_TCP:
                        temp = sipuri + ";transport=tcp";
                        break;
                    case ETransportMode.TM_TLS:
                        temp = sipuri + ";transport=tls";
                        break;
                }
            }
            catch (ArgumentOutOfRangeException ex)
            {
                Logger.LogWarn(ex);
            }
            return temp;
        }

        private T SafeInvoke<T>(Delegate del)
        {
            return CommonDelegates.SafeInvoke<T>(del);
        }
        #endregion
    }

} // namespace PjsipWrapper
