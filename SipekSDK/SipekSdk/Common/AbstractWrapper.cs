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
 * @see http://voipengine.googlepages.com/
 * 
 */

using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using Sipek.Sip;

namespace Sipek.Common
{
    public enum ESessionState : int
    {
        SESSION_STATE_NULL,	    /**< Before INVITE is sent or received  */
        SESSION_STATE_CALLING,	    /**< After INVITE is sent		    */
        SESSION_STATE_INCOMING,	    /**< After INVITE is received.	    */
        SESSION_STATE_EARLY,	    /**< After response with To tag.	    */
        SESSION_STATE_CONNECTING,	    /**< After 2xx is sent/received.	    */
        SESSION_STATE_CONFIRMED,	    /**< After ACK is sent/received.	    */
        SESSION_STATE_DISCONNECTED,   /**< Session is terminated.		    */
    }

    // event methods prototypes
    public delegate void DDtmfDigitReceived(int callId, int digit);
    public delegate void DMessageWaitingNotification(int mwi, string text);
    public delegate void DCallReplaced(int oldid, int newid);

    /// <summary>
    /// VoIP protocol stack interface. Defines some common events invoked by VoIP stack and 
    /// API methods invoked by user.
    /// </summary>
    public abstract class IVoipProxy
    {
        #region Events

        /// <summary>
        /// User Events. A protected virtual method makes possible to invoke events from derived classes. 
        /// </summary>

        /// <summary>
        /// DtmfDigitReceived event trigger by VoIP stack when DTMF is detected 
        /// </summary>
        public event DDtmfDigitReceived DtmfDigitReceived;
        protected void BaseDtmfDigitReceived(int callId, int digit)
        {
            if (null != DtmfDigitReceived) DtmfDigitReceived(callId, digit);
        }
        /// <summary>
        /// MessageWaitingIndication event trigger by VoIP stack when MWI indication arrived 
        /// </summary>
        public event DMessageWaitingNotification MessageWaitingIndication;
        protected void BaseMessageWaitingIndication(int mwi, string text)
        {
            if (null != MessageWaitingIndication) MessageWaitingIndication(mwi, text);
        }

        /// <summary>
        /// Notification that call is being replaced.
        /// </summary>
        public event DCallReplaced CallReplaced;
        protected void BaseCallReplacedCallback(int oldid, int newid)
        {
            if (null != CallReplaced) CallReplaced(oldid, newid);
        }

        /// <summary>
        /// Notification that proxy initialized
        /// </summary>
        public event EventHandler IsInitializedChanged;
        protected void BaseIsInitializedChanged()
        {
            if (null != IsInitializedChanged) IsInitializedChanged(this, null);
        }

        #endregion events

        #region Properties

        private IConfiguratorInterface _config = new NullConfigurator();
        public IConfiguratorInterface Config
        {
            set { _config = value; }
            get { return _config; }
        }

        /// <summary>
        /// Flag indicating stack initialization status
        /// </summary>
        public abstract bool IsInitialized
        {
            get;
            set;
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Initialize VoIP stack
        /// </summary>
        /// <returns></returns>
        public abstract int initialize();

        /// <summary>
        /// Shutdown VoIP stack
        /// </summary>
        /// <returns></returns>
        public virtual int shutdown()
        {
            DtmfDigitReceived = null;
            MessageWaitingIndication = null;
            return 1;
        }

        /// <summary>
        /// Set codec priority
        /// </summary>
        /// <param name="item">Codec Name</param>
        /// <param name="p">priority</param>
        public abstract void setCodecPriority(string item, int p);

        /// <summary>
        /// Get number of codecs in list
        /// </summary>
        /// <returns>Number of codecs</returns>
        public abstract int getNoOfCodecs();

        /// <summary>
        /// Get codec by index
        /// </summary>
        /// <param name="i">codec index</param>
        /// <returns>Codec Name</returns>
        public abstract string getCodec(int i);

        /// <summary>
        /// Creates an instance of call proxy 
        /// </summary>
        /// <returns></returns>
        public abstract ICallProxyInterface createCallProxy();

        #endregion
    }

    #region CallProxy interface

    public delegate void DCallStateChanged(int callId, ESessionState callState, string info);
    public delegate void DCallIncoming(int callId, string number, string info, IEnumerable<KeyValuePair<string, string>> headers);
    public delegate void DCallNotification(int callId, ECallNotification notFlag, string text);
    public delegate void DCallMediaStateChanged(int callId, ECallMediaState mediaState);

    #endregion

    /// <summary>
    /// Call oriented interface. Offers basic session control API and events called from VoIP stack
    /// </summary>
    /// 
    public abstract class ICallProxyInterface
    {
        #region Properties
        /// <summary>
        /// Call/Session identification. All public methods refers to this identification
        /// </summary>
        public abstract int SessionId
        { get; set; }

        #endregion

        #region Events
        /// <summary>
        /// CallStateChanged event trigger by VoIP stack when call state changed
        /// </summary>
        public static event DCallStateChanged CallStateChanged;
        protected static void BaseCallStateChanged(int callId, ESessionState callState, string info)
        {
            if (null != CallStateChanged) CallStateChanged(callId, callState, info);
        }
        /// <summary>
        /// CallIncoming event triggered by VoIP stack when new incoming call arrived
        /// </summary>
        public static event DCallIncoming CallIncoming;
        protected static void BaseIncomingCall(int callId, string number, string info, IEnumerable<KeyValuePair<string, string>> headers)
        {
            BaseCallStateChanged(callId, ESessionState.SESSION_STATE_INCOMING, info);

            if (null != CallIncoming) CallIncoming(callId, number, info, headers);
        }
        /// <summary>
        /// CallNotification event trigger by VoIP stack when call notification arrived
        /// </summary>
        public static event DCallNotification CallNotification;
        protected static void BaseCallNotification(int callId, ECallNotification notifFlag, string text)
        {
            if (null != CallNotification) CallNotification(callId, notifFlag, text);
        }

        public static event DCallMediaStateChanged CallMediaStateChanged;
        protected static void BaseCallMediaStateChanged(int callId, ECallMediaState mediaState)
        {
            if (null != CallMediaStateChanged) CallMediaStateChanged(callId, mediaState);
        }

        #endregion

        #region Public Methods
        /// <summary>
        /// Make call request
        /// </summary>
        /// <param name="dialedNo">Calling Number</param>
        /// <param name="accountId">Account Id</param>
        /// <returns>Session Identification</returns>
        public abstract int makeCall(string dialedNo, Sipek.Sip.SipHeader[] headers);

        /// <summary>
        /// End call
        /// </summary>
        /// <returns></returns>
        public abstract bool endCall();

        /// <summary>
        /// Report that device is alerted
        /// </summary>
        /// <returns></returns>
        public abstract bool alerted();

        /// <summary>
        /// Report that call is accepted/answered
        /// </summary>
        /// <returns></returns>
        public abstract bool acceptCall();

        /// <summary>
        /// Request call hold
        /// </summary>
        /// <returns></returns>
        public abstract bool holdCall();

        /// <summary>
        /// Request retrieve call
        /// </summary>
        /// <returns></returns>
        public abstract bool retrieveCall();

        /// <summary>
        /// Tranfer call to a given number
        /// </summary>
        /// <param name="number">Number to transfer call to</param>
        /// <returns></returns>
        public abstract bool xferCall(string number);

        /// <summary>
        /// Tranfer call to a given number
        /// </summary>
        /// <param name="number">Number to transfer call to</param>
        /// <returns></returns>
        public abstract bool xferCall(string number, SipHeader[] headers);

        /// <summary>
        /// Transfer call to partner session
        /// </summary>
        /// <param name="partnersession">Session to transfer call to</param>
        /// <returns></returns>
        public abstract bool xferCallSession(int partnersession);

        /// <summary>
        /// Request three party conference
        /// </summary>
        /// <param name="partnersession">Partner session for conference with</param>
        /// <returns></returns>
        public abstract bool threePtyCall(int partnersession);

        /// <summary>
        /// Request service (TODO)
        /// </summary>
        /// <param name="code"></param>
        /// <param name="dest"></param>
        /// <returns></returns>
        public abstract bool serviceRequest(int code, string dest);

        /// <summary>
        /// Dial digit by DTMF
        /// </summary>
        /// <param name="digits">digit string</param>
        /// <param name="mode">digit mode (TODO)</param>
        /// <returns></returns>
        public abstract bool dialDtmf(string digits, EDtmfMode mode);

        /// <summary>
        /// Retrieve codec information for this call
        /// </summary>
        /// <returns>codec name</returns>
        public abstract string getCurrentCodec();

        /// <summary>
        /// Send a message inside a call
        /// </summary>
        /// <returns></returns>
        public abstract bool sendCallMessage(string message);

        /// <summary>
        /// Connect media of another call to this call
        /// </summary>
        /// <param name="targetCallId">Target call id</param>
        /// <returns>True if success</returns>
        public abstract bool connectCallMedia(int targetCallId);

        #endregion
    }


    #region Null Pattern

    /// <summary>
    /// 
    /// </summary>
    internal class NullCallProxy : ICallProxyInterface
    {
        #region ICallProxyInterface Members

        public override int makeCall(string dialedNo, Sipek.Sip.SipHeader[] headers)
        {
            return 1;
        }

        public int makeCallByUri(string uri)
        {
            return 1;
        }

        public override bool endCall()
        {
            return false;
        }

        public override bool alerted()
        {
            return false;
        }

        public override bool acceptCall()
        {
            return false;
        }

        public override bool holdCall()
        {
            return false;
        }

        public override bool retrieveCall()
        {
            return false;
        }

        public override bool xferCall(string number)
        {
            return false;
        }

        public override bool xferCall(string number, SipHeader[] headers)
        {
            return false;
        }

        public override bool xferCallSession(int session)
        {
            return false;
        }

        public override bool threePtyCall(int session)
        {
            return false;
        }

        public override bool serviceRequest(int code, string dest)
        {
            return false;
        }

        public override bool dialDtmf(string digits, EDtmfMode mode)
        {
            return false;
        }

        public override string getCurrentCodec()
        {
            return "PCMA";
        }

        public override int SessionId
        {
            get { return 0; }
            set { ; }
        }

        public override bool connectCallMedia(int targetCallId)
        {
            return false;
        }

        public override bool sendCallMessage(string message)
        {
            return false;
        }
        #endregion
    }


    /// <summary>
    /// 
    /// </summary>
    public class NullVoipProxy : IVoipProxy
    {
        #region ICommonProxyInterface Members

        public override int initialize()
        {
            return 1;
        }

        public override int shutdown()
        {
            return 1;
        }

        //public override int registerAccounts()
        //{
        //  return 1;
        //}

        //public override int addBuddy(string ident, bool presence)
        //{
        //  return 1;
        //}

        //public override int delBuddy(int buddyId)
        //{
        //  return 1;
        //}

        //public override int sendMessage(string dest, string message)
        //{
        //  return 1;
        //}

        //public override int setStatus(int accId, EUserStatus presence_state)
        //{
        //  return 1;
        //}

        public override void setCodecPriority(string item, int p)
        {
        }
        public override int getNoOfCodecs() { return 0; }

        public override string getCodec(int i) { return ""; }

        public override bool IsInitialized
        {
            get
            {
                return false;
            }
            set
            {
                ;
            }
        }

        public override ICallProxyInterface createCallProxy()
        {
            return new NullCallProxy();
        }
        #endregion

    }
    #endregion  Null Pattern

}
