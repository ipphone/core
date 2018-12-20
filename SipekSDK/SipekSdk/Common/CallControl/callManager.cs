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
 */

/*! \mainpage Sipek Phone Project
 *
 * \section intro_sec Introduction
 *
 * SIPek is a small open source project that is intended to share common VoIP software design concepts 
 * and practices. It'd also like to become a simple and easy-to-use SIP phone with many useful features.
 * 
 * SIPek's telephony engine is based on common library used in Sipek project. The telephony part is powered 
 * by great SIP stack engine PJSIP (http://www.pjsip.org). The connection between pjsip code (C) 
 * and .Net GUI (C#) is handled by simple wrapper which is also suitable for mobile devices. Sipek use C# Audio library from http://www.codeproject.com/KB/graphics/AudioLib.aspx. 
 * The SIPek's simple software design enables efficient development, easy upgrading and 
 * user menus customizations.
 * 
 * Visit Sipek projects site at  http://sites.google.com/site/sipekvoip 
 *
 */


/*! \namespace CallControl
    \brief Module CallControl is a general Call Automaton engine controller. 

    Call control...
*/

using System.Collections;
using System.Collections.Generic;
using System;
using Sipek.Common;
using System.ComponentModel;
using Sipek.Sip;

namespace Sipek.Common.CallControl
{

    public delegate void DCallStateRefresh(int sessionId);
    public delegate void DIncomingCallNotification(int sessionId, string number, string info, IEnumerable<KeyValuePair<string, string>> headers);

    //////////////////////////////////////////////////////////////////////////
    /// <summary>
    /// CCallManager
    /// Main telephony class. Manages call instances. Handles user events and dispatches to a proper 
    /// call instance automaton. 
    /// </summary>
    public class CCallManager
    {
        #region Variables

        private static CCallManager _instance = null;
        private Dictionary<int, IStateMachine> _calls;  //!< Call table
        private IAbstractFactory _factory = new NullFactory();
        PendingAction _pendingAction;

        #endregion

        #region Properties

        public IAbstractFactory Factory
        {
            get { return _factory; }
            set { _factory = value; }
        }

        private IVoipProxy _stack = new NullVoipProxy();
        public IVoipProxy StackProxy
        {
            get { return _stack; }
            set { _stack = value; }
        }

        private IConfiguratorInterface _config = new NullConfigurator();
        public IConfiguratorInterface Config
        {
            get { return _config; }
            set { _config = value; }
        }

        /// <summary>
        /// Call indexer. 
        /// Retrieve a call instance (IStateMachine) from a call list. 
        /// </summary>
        /// <param name="sessionId">call/session identification</param>
        /// <returns>an instance of a call with a given sessionId</returns>
        public IStateMachine this[int sessionId]
        {
            get
            {
                if ((_calls.Count == 0) || (!_calls.ContainsKey(sessionId)))
                {
                    return new NullStateMachine();
                }
                return _calls[sessionId];
            }
        }

        /// <summary>
        /// Retrieve a complete list of calls (IStateMachines)
        /// </summary>
        public Dictionary<int, IStateMachine> CallList
        {
            get { return _calls; }
        }

        public int Count
        {
            get { return _calls.Count; }
        }

        public bool Is3Pty
        {
            get
            {
                return (this[EStateId.ACTIVE].Count == 2) ? true : false;
            }
        }

        private bool _initialized = false;
        public bool IsInitialized
        {
            get { return _initialized; }
        }

        public List<IStateMachine> this[EStateId stateId]
        {
            get
            {
                List<IStateMachine> calls = new List<IStateMachine>();
                foreach (KeyValuePair<int, IStateMachine> call in _calls)
                {
                    if (call.Value.State.Id == stateId)
                    {
                        calls.Add(call.Value);
                    }
                }
                return calls;
            }
        }

        #endregion Properties

        #region Constructor

        /// <summary>
        /// CCallManager Singleton
        /// </summary>
        /// <returns></returns>
        public static CCallManager Instance
        {
            get
            {
                if (_instance == null) _instance = new CCallManager();
                return _instance;
            }
        }

        #endregion Constructor

        #region Events

        /// <summary>
        /// Notify about call state changed in automaton with given sessionId
        /// </summary>
        public event DCallStateRefresh CallStateRefresh;
        public event DIncomingCallNotification IncomingCallNotification;
        public event DCallMediaStateChanged CallMediaStateChanged;

        /// <summary>
        /// Action definitions for pending events.
        /// </summary>
        enum EPendingActions : int
        {
            EUserAnswer,
            ECreateSession,
            EUserHold
        };

        /// <summary>
        /// Internal mechanism to execute 2 stage actions. Some user events requires 
        /// two request to VoIP side. Depending on result the second action is executed.
        /// </summary>
        class PendingAction
        {
            delegate void DPendingAnswer(int sessionId); // for onUserAnswer
            delegate void DPendingCreateSession(string number, int accountId); // for CreateOutboudCall

            private EPendingActions _actionType;
            private int _sessionId;
            private int _accountId;
            private string _number;
            private Sipek.Sip.SipHeader[] _headers;

            public PendingAction(EPendingActions action, int sessionId)
            {
                _actionType = action;
                _sessionId = sessionId;
            }
            public PendingAction(EPendingActions action, string number, int accId, Sipek.Sip.SipHeader[] headers)
            {
                _actionType = action;
                _sessionId = -1;
                _number = number;
                _accountId = accId;
                _headers = headers;
            }

            public void Activate()
            {
                switch (_actionType)
                {
                    case EPendingActions.EUserAnswer:
                        CCallManager.Instance.OnUserAnswer(_sessionId);
                        break;
                    case EPendingActions.ECreateSession:
                        CCallManager.Instance.CreateSmartOutboundCall(_number, _accountId, _headers);
                        break;
                    case EPendingActions.EUserHold:
                        CCallManager.Instance.OnUserHoldRetrieve(_sessionId);
                        break;
                }
            }

        }

        /////////////////////////////////////////////////////////////////////////
        // Callback handlers
        /// <summary>
        /// Inform GUI to be refreshed 
        /// </summary>
        public void updateGui(int sessionId)
        {
            // check if call is in table (doesn't work in connecting state - session not inserted in call table)
            //if (!_calls.ContainsKey(sessionId)) return;

            if (null != CallStateRefresh) CallStateRefresh(sessionId);
        }

        #endregion Events

        #region Public methods

        /// <summary>
        /// Initialize telephony and VoIP stack. On success register accounts.
        /// </summary>
        /// <returns>initialiation status</returns>
        public int Initialize(IVoipProxy proxy)
        {
            _stack = proxy;

            int status = 0;
            ///
            if (!IsInitialized)
            {
                //// register to signaling proxy interface
                ICallProxyInterface.CallStateChanged += OnCallStateChanged;
                ICallProxyInterface.CallIncoming += OnIncomingCall;
                ICallProxyInterface.CallNotification += OnCallNotification;
                ICallProxyInterface.CallMediaStateChanged += OnCallMediaStateChanged;

                // Initialize call table
                _calls = new Dictionary<int, IStateMachine>();
            }

            // (re)initialize voip proxy
            if (!StackProxy.IsInitialized)
            {
                status = StackProxy.initialize();
                if (status != 0) return status;
            }

            // (re)register 
            _initialized = true;

            return status;
        }

        /// <summary>
        /// Shutdown telephony and VoIP stack
        /// </summary>
        public void Shutdown()
        {
            IStateMachine[] callarr = new IStateMachine[CallList.Count];
            CallList.Values.CopyTo(callarr, 0);
            for (int i = 0; i < callarr.Length; i++)
            {
                callarr[i].Destroy();
            }

            this.CallList.Clear();

            StackProxy.shutdown();
            _initialized = false;

            CallStateRefresh = null;
            IncomingCallNotification = null;

            ICallProxyInterface.CallStateChanged -= OnCallStateChanged;
            ICallProxyInterface.CallIncoming -= OnIncomingCall;
            ICallProxyInterface.CallNotification -= OnCallNotification;
            ICallProxyInterface.CallMediaStateChanged -= OnCallMediaStateChanged;
            StackProxy.CallReplaced -= OnCallReplaced;
        }

        /// <summary>
        /// Create outgoing call using default accountId. 
        /// </summary>
        /// <param name="number">Number to call</param>
        public int CreateSimpleOutboundCall(string number, Sipek.Sip.SipHeader[] headers)
        {
            int accId = Config.DefaultAccountIndex;
            return this.CreateSimpleOutboundCall(number, accId, headers);
        }

        /// <summary>
        /// Try to create an outbound call. No automatics: make call only if no other call exists
        /// </summary>
        /// <param name="number"></param>
        /// <param name="accountId"></param>
        /// <returns>SessionId or -1 on error</returns>
        public int CreateSimpleOutboundCall(string number, int accountId, Sipek.Sip.SipHeader[] headers)
        {
            // if no calls in list
            // Why not?!
            if (/*(this.Count > 0) || */(!IsInitialized))
            {
                return -1;
            }
            else
            {
                // create state machine
                IStateMachine call = Factory.CreateStateMachine();
                // couldn't create new call instance (max calls?)
                if (call == null)
                {
                    return -1;
                }

                // make call request (stack provides new sessionId)
                int newsession = call.State.makeCall(number, headers);
                if (newsession == -1)
                {
                    return -1;
                }
                // update call table
                // catch argument exception (same key)!!!!
                try
                {
                    call.Session = newsession;
                    _calls.Add(newsession, call);
                }
                catch
                {
                    // previous call not released ()
                    // first release old one
                    _calls[newsession].Destroy();
                    // and then add new one
                    _calls.Add(newsession, call);
                }

                return call.Session;
            }
        }

        /// <summary>
        /// Create outgoing call to a number and default account.
        /// 
        /// If the other calls exist check if it is possible to create a new one.
        /// The logic below will automatically put the active call on hold, 
        /// return -2 and store a new call creation request. When hold confirmation 
        /// received create a call. 
        /// 
        /// Be aware: This is done asynchronously. 
        /// 
        /// </summary>
        /// <param name="number">Number to call</param>
        public int CreateSmartOutboundCall(string number, Sipek.Sip.SipHeader[] headers)
        {
            int accId = Config.DefaultAccountIndex;
            return this.CreateSmartOutboundCall(number, accId, headers);
        }

        /// <summary>
        /// Create outgoing call to a number and from a given account.
        /// 
        /// If the other calls exist check if it is possible to create a new one.
        /// The logic below will automatically put the active call on hold, 
        /// return -2 and store a new call creation request. When hold confirmation 
        /// received create a call. 
        /// 
        /// Be aware: This is done asynchronously. 
        /// 
        /// </summary>
        /// <param name="number">Number to call</param>
        /// <param name="accountId">Specified account Id </param>
        public int CreateSmartOutboundCall(string number, int accountId, Sipek.Sip.SipHeader[] headers)
        {
            if (!IsInitialized) return -1;

            // check if current call automatons allow session creation.
            if (this.GetNoCallsInStates((int)(EStateId.CONNECTING | EStateId.ALERTING)) > 0)
            {
                // new call not allowed!
                return -1;
            }
            // if at least 1 call connected try to put it on hold first
            if (this[EStateId.ACTIVE].Count == 0)
            {
                // create state machine
                IStateMachine call = Factory.CreateStateMachine();
                // couldn't create new call instance (max calls?)
                if (call == null)
                {
                    return -1;
                }

                // make call request (stack provides new sessionId)
                int newsession = call.State.makeCall(number, headers);
                if (newsession == -1)
                {
                    return -1;
                }
                // update call table
                // catch argument exception (same key)!!!!
                try
                {
                    call.Session = newsession;
                    _calls.Add(newsession, call);
                }
                catch
                {
                    // previous call not released ()
                    // first release old one
                    // if it exists
                    if (_calls.ContainsKey(newsession))
                        _calls[newsession].Destroy();
                    
                    // and then add new one
                    _calls.Add(newsession, call);
                }

                return call.Session;
            }
            else // we have at least one ACTIVE call
            {
                // put connected call on hold
                _pendingAction = new PendingAction(EPendingActions.ECreateSession, number, accountId, headers);

                List<IStateMachine> calls = this[EStateId.ACTIVE];
                if (calls.Count > 0)
                {
                    calls[0].State.holdCall();
                }
                // indicates that new call is pending...
                // At this point we don't know yet if the call will be created or not
                // The call will be created when current call is put on hold (confirmation)!
                return -2;
            }

        }


        /// <summary>
        /// User triggers a call release for a given session
        /// </summary>
        /// <param name="session">session identification</param>
        public void OnUserRelease(int session)
        {
            this[session].State.endCall();
        }

        /// <summary>
        /// User accepts call for a given session
        /// In case of multi call put current active call to Hold
        /// </summary>
        /// <param name="session">session identification</param>
        public void OnUserAnswer(int session)
        {
            HoldActiveCalls(new PendingAction(EPendingActions.EUserAnswer, session));

            this[session].State.acceptCall();
        }

        /// <summary>
        /// User put call on hold or retrieve 
        /// </summary>
        /// <param name="session">session identification</param>
        public void OnUserHoldRetrieve(int session)
        {
            // check Hold or Retrieve
            IAbstractState state = this[session].State;
            if (state.Id == EStateId.ACTIVE)
            {
                this[session].State.holdCall();
            }
            else if (state.Id == EStateId.HOLDING)
            {
                // execute retrieve
                // check if any ACTIVE calls
                HoldActiveCalls(new PendingAction(EPendingActions.EUserHold, session));

                this[session].State.retrieveCall();
            }
            else
            {
                // illegal
            }
        }

        /// <summary>
        /// User starts a call transfer
        /// </summary>
        /// <param name="session">session identification</param>
        /// <param name="number">number to transfer</param>
        public void OnUserTransfer(int session, string number)
        {
            this[session].State.xferCall(number);
        }

        /// <summary>
        /// User starts a call transfer
        /// </summary>
        /// <param name="session">session identification</param>
        /// <param name="number">number to transfer</param>
        public void OnUserTransfer(int session, string number, SipHeader[] headers)
        {
            this[session].State.xferCall(number, headers);
        }

        /// <summary>
        /// Attendant transfer
        /// </summary>
        /// <param name="session"></param>
        /// <param name="number"></param>
        public void OnUserTransferAttendant(int session, int partnerSession)
        {
            this[session].State.xferCallSession(partnerSession);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="session"></param>
        /// <param name="digits"></param>
        /// <param name="mode"></param>
        public void OnUserDialDigit(int session, string digits, EDtmfMode mode)
        {
            this[session].State.dialDtmf(digits, mode);
        }

        public void OnUserConference(int sessionId, int targetSession)
        {
            this[targetSession].IsConference = this[sessionId].State.connectCallMedia(targetSession);
        }

        /// <summary>
        /// Send message inside call dialog
        /// </summary>
        /// <param name="sessionId"></param>
        /// <returns></returns>
        public bool OnUserSendCallMessage(int sessionId, string message)
        {
            return this[sessionId].State.sendCallMessage(message);
        }

        #endregion  // public methods

        #region Internal & Private Methods

        /// <summary>
        /// Destroy call 
        /// </summary>
        /// <param name="session">session identification</param>
        internal void DestroySession(int session)
        {
            bool notify = true;
            if (this[session].DisableStateNotifications)
            {
                notify = false;
            }
            _calls.Remove(session);
            // Warning: this call no longer exists
            if (notify) updateGui(session);
        }

        /// <summary>
        /// 
        /// </summary>
        internal void activatePendingAction()
        {
            if (null != _pendingAction) _pendingAction.Activate();
            _pendingAction = null;
        }

        /// <summary>
        /// Hold calls in active state, but not in conference
        /// </summary>
        /// <param name="pendingAction">Action to make after the call putted on hold</param>
        private void HoldActiveCalls(PendingAction pendingAction)
        {
            if (this[EStateId.ACTIVE].Count > 0)
            {
                foreach (var sm in this[EStateId.ACTIVE])
                {
                    if (!sm.IsConference)
                    {
                        // get 1st and put it on hold
                        if (!sm.IsNull) sm.State.holdCall();

                        // set Retrieve event pending for HoldConfirm
                        _pendingAction = pendingAction;
                        return;
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="callId"></param>
        /// <param name="callState"></param>
        private void OnCallStateChanged(int callId, ESessionState callState, string info)
        {
            if (callState == ESessionState.SESSION_STATE_INCOMING)
            {
                IStateMachine incall = Factory.CreateStateMachine();
                // couldn't create new call instance (max calls?)
                if (incall.IsNull)
                {
                    // check if CFB, activate redirection
                    if (Config.CFBFlag == true)
                    {
                        // get stack proxy
                        ICallProxyInterface proxy = StackProxy.createCallProxy();
                        // assign callid to the proxy...
                        //proxy.SessionId = callId;
                        proxy.serviceRequest((int)EServiceCodes.SC_CFB, Config.CFBNumber);
                        return;
                    }
                }
                // save session parameters
                incall.Session = callId;

                // check if callID already exists!!!
                if (CallList.ContainsKey(callId))
                {
                    // shouldn't be here
                    // release the call
                    CallList[callId].State.endCall();
                    return;
                }
                // add call to call table
                _calls.Add(callId, incall);

                return;
            }

            IStateMachine call = this[callId];
            if (call.IsNull) return;

            switch (callState)
            {
                case ESessionState.SESSION_STATE_CALLING:
                    //sm.getState().onCalling();
                    break;
                case ESessionState.SESSION_STATE_EARLY:
                    call.State.onAlerting();
                    break;
                case ESessionState.SESSION_STATE_CONNECTING:
                    call.State.onConnect();
                    break;
                case ESessionState.SESSION_STATE_DISCONNECTED:
                    call.State.onReleased();
                    break;
            }
        }

        /// <summary>
        /// Create session for incoming call.
        /// </summary>
        /// <param name="sessionId">session identification</param>
        /// <param name="number">number from calling party</param>
        /// <param name="info">additional info of calling party</param>
        private void OnIncomingCall(int sessionId, string number, string info, IEnumerable<KeyValuePair<string, string>> headers)
        {
            IStateMachine call = this[sessionId];

            if (call.IsNull) return;

            // inform automaton for incoming call
            call.State.incomingCall(number, info, new List<KeyValuePair<string, string>>());

            // call callback 
            if ((IncomingCallNotification != null) && (call.DisableStateNotifications == false)) IncomingCallNotification(sessionId, number, info, headers);
        }

        private void OnCallNotification(int callId, ECallNotification notFlag, string text)
        {
            if (notFlag == ECallNotification.CN_HOLDCONFIRM)
            {
                IStateMachine sm = this[callId];
                if (!sm.IsNull) sm.State.onHoldConfirm();
            }
        }

        private void OnCallMediaStateChanged(int callId, ECallMediaState mediaState)
        {
            IStateMachine sm = this[callId];
            if (!sm.IsNull) sm.State.onMediaStateChanged(mediaState);

            if (CallMediaStateChanged != null) CallMediaStateChanged(callId, mediaState);
        }

        /// <summary>
        /// Replace call ids
        /// </summary>
        /// <param name="newid"></param>
        /// <param name="oldid"></param>
        private void OnCallReplaced(int oldid, int newid)
        {
            IStateMachine call = CallList[oldid];
            _calls.Remove(oldid);
            call.Session = newid;
            CallList.Add(newid, call);
        }

        /// <summary>
        /// Gets the number of calls in given states (bitmask)
        /// 
        /// example: 
        ///   getNoCallsInStates(EStateId.CONNECTING | EStateId.ALERTING)
        /// 
        /// </summary>
        /// <param name="states">a bit mask of states</param>
        /// <returns></returns>
        private int GetNoCallsInStates(int states)
        {
            int cnt = 0;
            foreach (KeyValuePair<int, IStateMachine> kvp in _calls)
            {
                if ((states & (int)kvp.Value.State.Id) == (int)kvp.Value.State.Id)
                {
                    cnt++;
                }
            }
            return cnt;
        }

        #endregion Methods

        #region Obsolete Methods
        // Thanks to Ian Kemp

        [Obsolete("Use the CreateSmartOutboundCall(string number) method instead.")]
        public IStateMachine createOutboundCall(string number, Sipek.Sip.SipHeader[] headers)
        {
            return this[CreateSmartOutboundCall(number, Config.DefaultAccountIndex, headers)];
        }

        [Obsolete("Use the CreateSmartOutboundCall(string number, int accountId) method instead")]
        public IStateMachine createOutboundCall(string number, int accountId, Sipek.Sip.SipHeader[] headers)
        {
            return this[CreateSmartOutboundCall(number, accountId, headers)];
        }

        [Obsolete("Use the CallManager call indexer property instead")]
        public IStateMachine getCall(int session)
        {
            return this[session];
        }

        [Obsolete("Use the CallManager StateId indexer property instead")]
        public IStateMachine getCallInState(EStateId stateId)
        {
            List<IStateMachine> calls = this[stateId];
            if (calls.Count == 0)
            {
                return new NullStateMachine();
            }
            return calls[0];
        }

        [Obsolete("Use the CallManager StateId indexer property instead")]
        public ICollection<IStateMachine> enumCallsInState(EStateId stateId)
        {
            return this[stateId];
        }

        [Obsolete("Use the CallManager StateId indexer property instead")]
        public int getNoCallsInState(EStateId stateId)
        {
            return this[stateId].Count;
        }

        [Obsolete("Use the OnUserRelease(int session) method instead")]
        public void onUserRelease(int session)
        {
            this.OnUserRelease(session);
        }

        [Obsolete("Use the OnUserAnswer(int session) method instead")]
        public void onUserAnswer(int session)
        {
            this.OnUserAnswer(session);
        }

        [Obsolete("Use the OnUserHoldRetrieve(int session) method instead")]
        public void onUserHoldRetrieve(int session)
        {
            this.OnUserHoldRetrieve(session);
        }

        [Obsolete("Use the onUserTransfer(int session, string number) method instead")]
        public void onUserTransfer(int session, string number)
        {
            this.OnUserTransfer(session, number);
        }

        [Obsolete("Use the OnUserDialDigit(int session, string digits, EDtmfMode mode) method instead")]
        public void onUserDialDigit(int session, string digits, EDtmfMode mode)
        {
            this.OnUserDialDigit(session, digits, mode);
        }

        [Obsolete("Use the OnUserConference(int session) method instead")]
        public void onUserConference(int session1, int session2)
        {
            this.OnUserConference(session1, session2);
        }

        [Obsolete("Use the OnUserSendCallMessage(int session, string message) method instead")]
        public bool onUserSendCallMessage(int sessionId, string message)
        {
            return this.OnUserSendCallMessage(sessionId, message);
        }

        [Obsolete("Use the Initialize(IVoipProxy stack) method")]
        public int Initialize()
        {
            return this.Initialize(_stack);
        }


        #endregion
    }

} // namespace Sipek
