/* 
 * Copyright (C) 2008 Sasa Coh <sasacoh@gmail.com>
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
 * 
 * @see http://sites.google.com/site/sipekvoip
 * 
 */

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using Sipek.Common;

namespace Sipek.Sip
{
    // callback delegates
    internal delegate int OnCallStateChanged(int callId, ESessionState stateId);
    internal delegate int OnCallIncoming(int callId, string number, string localNumber, string headers);
    internal delegate int OnCallHoldConfirm(int callId);
    internal delegate int OnCallMediaStateChanged(int callId, ECallMediaState mediaStateId);

    [StructLayout(LayoutKind.Sequential)]
    public struct SipHeader
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)] public string name;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 255)] public string value;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct SipHeaderCollection
    {
        [MarshalAs(UnmanagedType.LPArray)] public SipHeader[] headers;
    }

    /// <summary>
    /// Implementation of ICallProxyInterface interface use by call state machine. 
    /// Each call (session) contains an instance of a call proxy. 
    /// Current session is identified by SessionId property.
    /// pjsipCallProxy communicates with pjsip stack using API functions and callbacks.
    /// </summary>
    internal class pjsipCallProxy : ICallProxyInterface
    {
        #region DLL declarations

        [DllImport(Native.PJSIP_DLL, EntryPoint = "dll_makeCall", CallingConvention = CallingConvention.Cdecl)]
        private static extern int dll_makeCall(int accountId, string uri, [MarshalAs(UnmanagedType.LPArray)] SipHeader[] headers, int headersCount);

        [DllImport(Native.PJSIP_DLL, EntryPoint = "dll_releaseCall", CallingConvention = CallingConvention.Cdecl)]
        private static extern int dll_releaseCall(int callId);

        [DllImport(Native.PJSIP_DLL, EntryPoint = "dll_answerCall", CallingConvention = CallingConvention.Cdecl)]
        private static extern int dll_answerCall(int callId, int code);

        [DllImport(Native.PJSIP_DLL, EntryPoint = "dll_holdCall", CallingConvention = CallingConvention.Cdecl)]
        private static extern int dll_holdCall(int callId);

        [DllImport(Native.PJSIP_DLL, EntryPoint = "dll_retrieveCall", CallingConvention = CallingConvention.Cdecl)]
        private static extern int dll_retrieveCall(int callId);

        [DllImport(Native.PJSIP_DLL, EntryPoint = "dll_xferCall", CallingConvention = CallingConvention.Cdecl)]
        private static extern int dll_xferCall(int callId, string uri);

        [DllImport(Native.PJSIP_DLL, EntryPoint = "dll_xferCall", CallingConvention = CallingConvention.Cdecl)]
        private static extern int dll_xferCallWithHeaders(int callId, string uri, [MarshalAs(UnmanagedType.LPArray)] SipHeader[] headers, int headersCount);

        [DllImport(Native.PJSIP_DLL, EntryPoint = "dll_xferCallWithReplaces", CallingConvention = CallingConvention.Cdecl)]
        private static extern int dll_xferCallWithReplaces(int callId, int dstSession);

        [DllImport(Native.PJSIP_DLL, EntryPoint = "dll_xferCallWithReplacesAndHeaders", CallingConvention = CallingConvention.Cdecl)]
        private static extern int dll_xferCallWithReplacesAndHeaders(int callId, int dstSession, [MarshalAs(UnmanagedType.LPArray)] SipHeader[] headers, int headersCount);

        [DllImport(Native.PJSIP_DLL, EntryPoint = "dll_serviceReq", CallingConvention = CallingConvention.Cdecl)]
        private static extern int dll_serviceReq(int callId, int serviceCode, string destUri);

        [DllImport(Native.PJSIP_DLL, EntryPoint = "dll_dialDtmf", CallingConvention = CallingConvention.Cdecl)]
        private static extern int dll_dialDtmf(int callId, string digits, int mode);

        [DllImport(Native.PJSIP_DLL, EntryPoint = "dll_getCurrentCodec", CallingConvention = CallingConvention.Cdecl)]
        private static extern int dll_getCurrentCodec(int callId, StringBuilder codec);

        [DllImport(Native.PJSIP_DLL, EntryPoint = "dll_makeConference", CallingConvention = CallingConvention.Cdecl)]
        private static extern int dll_makeConference(int callId);

        [DllImport(Native.PJSIP_DLL, EntryPoint = "dll_sendCallMessage", CallingConvention = CallingConvention.Cdecl)]
        private static extern int dll_sendCallMessage(int callId, string message);

        [DllImport(Native.PJSIP_DLL, EntryPoint = "dll_isThreadRegistered", CallingConvention = CallingConvention.Cdecl)]
        private static extern bool dll_isThreadRegistered();

        [DllImport(Native.PJSIP_DLL, EntryPoint = "dll_threadRegister", CallingConvention = CallingConvention.Cdecl)]
        private static extern int dll_threadRegister(string name);

        [DllImport(Native.PJSIP_DLL, EntryPoint = "dll_connectCallsMedia", CallingConvention = CallingConvention.Cdecl)]
        private static extern int dll_connectCallsMedia(int call1Id, int call2Id);

        #endregion

        #region Callback Declarations

        // passing delegates to unmanaged code (.dll)
        [DllImport(Native.PJSIP_DLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern int onCallStateCallback(OnCallStateChanged cb);

        [DllImport(Native.PJSIP_DLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern int onCallIncoming(OnCallIncoming cb);

        [DllImport(Native.PJSIP_DLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern int onCallHoldConfirmCallback(OnCallHoldConfirm cb);

        [DllImport(Native.PJSIP_DLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern int onCallMediaStateChanged(OnCallMediaStateChanged cb);

        // Static declaration because of CallbackonCollectedDelegate exception!
        private static readonly OnCallStateChanged csDel = onCallStateChanged;
        private static readonly OnCallIncoming ciDel = onCallIncoming;
        private static readonly OnCallHoldConfirm chDel = onCallHoldConfirm;
        private static readonly OnCallMediaStateChanged cmDel = onCallMediaStateChanged;

        #endregion

        #region Properties

        private readonly IConfiguratorInterface _config = new NullConfigurator();

        private IConfiguratorInterface Config
        {
            get { return _config; }
        }

        public override int SessionId { get; set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor called by pjsipWrapper with Config parameter. 
        /// Make sure you set Config in pjsipWrapper before using pjsipCallProxy
        /// </summary>
        /// <param name="config"></param>
        internal pjsipCallProxy(IConfiguratorInterface config)
        {
            _config = config;
        }

        /// <summary>
        /// Static initializer. Call this method to set callbacks from SIP stack. 
        /// </summary>
        public static void initialize()
        {
            // assign callbacks
            onCallIncoming(ciDel);
            onCallStateCallback(csDel);
            onCallHoldConfirmCallback(chDel);
            onCallMediaStateChanged(cmDel);
        }

        #endregion Constructor

        #region Public Methods

        /// <summary>
        /// Method makeCall creates call session. Checks the 1st parameter 
        /// format is SIP URI, if not build one.  
        /// </summary>
        /// <param name="dialedNo"></param>
        /// <param name="accountId"></param>
        /// <returns>SessionId chosen by pjsip stack</returns>
        public override int makeCall(string dialedNo, SipHeader[] headers)
        {
            string sipuri = "";

            // check if call by URI
            if (dialedNo.IndexOf("sip:") == 0)
            {
                // do nothing...
                sipuri = dialedNo;
            }
            else
            {
                // prepare URI
                sipuri = "sip:" + dialedNo + "@" + Config.Account.HostName;
            }
            // Select configured transport for this account: udp, tcp, tls 
            sipuri = pjsipStackProxy.Instance.SetTransport(sipuri);

            // Don't forget to convert accontId here!!!
            // Store session identification for further requests

            SessionId = SafeInvoke(() => { return dll_makeCall(Config.Account.Index, sipuri, headers, headers.Length); });

            return SessionId;
        }

        /// <summary>
        /// End call for a given session
        /// </summary>
        /// <param name="sessionId"></param>
        /// <returns></returns>
        public override bool endCall()
        {
            SafeBeginInvoke(() => { dll_releaseCall(SessionId); });

            return true;
        }

        /// <summary>
        /// Signals sip stack that device is alerted (ringing)
        /// </summary>
        /// <param name="sessionId"></param>
        /// <returns></returns>
        public override bool alerted()
        {
            SafeBeginInvoke(() => { dll_answerCall(SessionId, 180); });

            return true;
        }

        /// <summary>
        /// Signals that user accepts the call (asnwer)
        /// </summary>
        /// <param name="sessionId"></param>
        /// <returns></returns>
        public override bool acceptCall()
        {
            SafeBeginInvoke(() => { dll_answerCall(SessionId, 200); });

            return true;
        }

        /// <summary>
        /// Hold request for a given session
        /// </summary>
        /// <param name="sessionId"></param>
        /// <returns></returns>
        public override bool holdCall()
        {
            SafeBeginInvoke(() => { dll_holdCall(SessionId); });

            return true;
        }

        /// <summary>
        /// Retrieve request for a given session
        /// </summary>
        /// <param name="sessionId"></param>
        /// <returns></returns>
        public override bool retrieveCall()
        {
            SafeBeginInvoke(() => { dll_retrieveCall(SessionId); });

            return true;
        }

        /// <summary>
        /// Trasfer call to number
        /// </summary>
        /// <param name="sessionId"></param>
        /// <param name="number"></param>
        /// <returns></returns>
        public override bool xferCall(string number)
        {
            return xferCall(number, new SipHeader[0]);
        }

        /// <summary>
        /// Trasfer call to number
        /// </summary>
        /// <param name="sessionId"></param>
        /// <param name="number"></param>
        /// <returns></returns>
        public override bool xferCall(string number, SipHeader[] headers)
        {
            string uri = "sip:" + number + "@" + Config.Account.HostName;
            if (headers == null) headers = new SipHeader[0];

            SafeBeginInvoke(() => { dll_xferCallWithHeaders(SessionId, uri, headers, headers?.Length ?? 0); });

            return true;
        }

        /// <summary>
        /// Transfer call to other session
        /// </summary>
        /// <param name="sessionId"></param>
        /// <param name="session"></param>
        /// <returns></returns>
        public override bool xferCallSession(int session, SipHeader[] headers)
        {
            SafeBeginInvoke(() => { dll_xferCallWithReplacesAndHeaders(SessionId, session, headers, headers?.Length ?? 0); });

            return true;
        }

        /// <summary>
        /// Make conference with given session
        /// </summary>
        /// <param name="sessionId"></param>
        /// <param name="session"></param>
        /// <returns></returns>
        public override bool threePtyCall(int session)
        {
            return SafeInvoke(() => { return dll_serviceReq(SessionId, (int) EServiceCodes.SC_3PTY, ""); }) == 1;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sessionId"></param>
        /// <param name="code"></param>
        /// <param name="dest"></param>
        /// <returns></returns>
        public override bool serviceRequest(int code, string dest)
        {
            string destUri = "<sip:" + dest + "@" + Config.Account.HostName + ">";

            return SafeInvoke(() => { return dll_serviceReq(SessionId, code, destUri); }) == 1;
        }

        /// <summary>
        /// Send dtmf digit
        /// </summary>
        /// <param name="sessionId"></param>
        /// <param name="digits"></param>
        /// <param name="mode"></param>
        /// <returns></returns>
        public override bool dialDtmf(string digits, EDtmfMode mode)
        {
            return SafeInvoke(() => { return dll_dialDtmf(SessionId, digits, (int) mode); }) == 1;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string getCurrentCodec()
        {
            var codec = new StringBuilder(256);

            SafeInvoke(() => { dll_getCurrentCodec(SessionId, codec); });

            return codec.ToString();
        }

        /// <summary>
        /// Make a conference call
        /// </summary>
        /// <returns></returns>
        public override bool connectCallMedia(int targetCallId)
        {
            return SafeInvoke(() => { return dll_connectCallsMedia(SessionId, targetCallId); }) == 0; // Zero on success here
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public override bool sendCallMessage(string message)
        {
            return SafeInvoke(() => { return dll_sendCallMessage(SessionId, message); }) == 1;
        }

        // Invokes
        private void SafeBeginInvoke(Action action)
        {
            CommonDelegates.SafeBeginInvoke(action);
        }

        private void SafeInvoke(Action action)
        {
            CommonDelegates.SafeInvoke(action);
        }

        private T SafeInvoke<T>(Func<T> func)
        {
            return CommonDelegates.SafeInvoke(func);
        }

        #endregion Methods

        #region Callbacks

        /// <summary>
        /// 
        /// </summary>
        /// <param name="callId"></param>
        /// <param name="callState"></param>
        /// <returns></returns>
        private static int onCallStateChanged(int callId, ESessionState callState)
        {
            BaseCallStateChanged(callId, callState, "");
            return 0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="callId"></param>
        /// <param name="sturi"></param>
        /// <returns></returns>
        private static int onCallIncoming(int callId, string sturi, string localNumber, string headers)
        {
            string uri = sturi;
            string display = "";
            string number = "";

            if (null != uri)
            {
                // get indices
                int startNum = uri.IndexOf("<sip:");
                int atPos = uri.IndexOf('@');
                // search for number
                if ((startNum >= 0) && (atPos > startNum))
                {
                    number = uri.Substring(startNum + 5, atPos - startNum - 5);
                }

                // extract display name if exists
                if (startNum >= 0)
                {
                    display = uri.Remove(startNum, uri.Length - startNum).Trim();
                }
                else
                {
                    int semiPos = display.IndexOf(';');
                    if (semiPos >= 0)
                    {
                        display = display.Remove(semiPos, display.Length - semiPos);
                    }
                    else
                    {
                        int colPos = display.IndexOf(':');
                        if (colPos >= 0)
                        {
                            display = display.Remove(colPos, display.Length - colPos);
                        }
                    }
                }
            }

            // parse headers
            var headersList = new List<KeyValuePair<string, string>>();
            if (!String.IsNullOrEmpty(headers))
            {
                string[] tempArray = headers.Split(new[] {"\r\n"}, StringSplitOptions.RemoveEmptyEntries);

                foreach (string item in tempArray)
                {
                    try
                    {
                        int splitterIndex = item.IndexOf(':');

                        if (splitterIndex < item.Length - 2)
                        {
                            headersList.Add(new KeyValuePair<string, string>(
                                                item.Substring(0, splitterIndex).Trim(),
                                                item.Substring(splitterIndex + 1).Trim()
                                                ));
                        }
                    }
                    catch
                    {
                    }
                }
            }

            // invoke callback
            BaseIncomingCall(callId, number, display, headersList);
            return 1;
        }

        private static int onCallHeaderReceived(int callId, string header)
        {
            return 1;
        }

        /// <summary>
        /// Not used
        /// </summary>
        /// <param name="callId"></param>
        /// <returns></returns>
        private static int onCallHoldConfirm(int callId)
        {
            //if (sm != null) sm.getState().onHoldConfirm();
            // TODO:::implement proper callback
            BaseCallNotification(callId, ECallNotification.CN_HOLDCONFIRM, "");
            return 1;
        }

        private static int onCallMediaStateChanged(int callId, ECallMediaState callMediaState)
        {
            BaseCallMediaStateChanged(callId, callMediaState);
            return 1;
        }

        #endregion
    }
}
