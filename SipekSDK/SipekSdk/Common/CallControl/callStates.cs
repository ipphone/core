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

using System.Threading.Tasks;
using Sipek.Common;
using System.Collections.Generic;


namespace Sipek.Common.CallControl
{

    #region IdleState
    /// <summary>
    /// State Idle indicates the call is inactive
    /// </summary>
    internal class CIdleState : IAbstractState
    {
        public CIdleState(IStateMachine sm)
            : base(sm)
        {
            Id = EStateId.IDLE;
        }

        internal override void OnEntry()
        {
        }

        internal override void OnExit()
        {
        }

        public override bool endCall()
        {
            _smref.Destroy();
            CallProxy.endCall();
            return base.endCall();
        }

        /// <summary>
        /// Make call to a given number and accountId. Assign sessionId to call state machine got from VoIP part.
        /// </summary>
        /// <param name="dialedNo"></param>
        /// <param name="accountId"></param>
        /// <returns></returns>
        public override int makeCall(string dialedNo, Sipek.Sip.SipHeader[] headers)
        {
            _smref.CallingNumber = dialedNo;
            // make call and save sessionId
            _smref.ChangeState(EStateId.CONNECTING);
            _smref.Session = CallProxy.makeCall(dialedNo, headers);
            return _smref.Session;
        }

        /// <summary>
        /// Handle incoming call requests. Check for supplementary services flags.
        /// </summary>
        /// <param name="callingNo"></param>
        /// <param name="display"></param>
        public override void incomingCall(string callingNo, string display, IEnumerable<KeyValuePair<string, string>> headers)
        {
            // check supplementary services flags
            if ((_smref.Config.CFUFlag == true) && (_smref.Config.CFUNumber.Length > 0))
            {
                // do NOT notify about state changes 
                _smref.DisableStateNotifications = true;
                CallProxy.serviceRequest((int)EServiceCodes.SC_CFU, _smref.Config.CFUNumber);
                this.endCall();
                return;
            }
            else if (_smref.Config.DNDFlag == true)
            {
                // do NOT notify about state changes 
                _smref.DisableStateNotifications = true;
                CallProxy.serviceRequest((int)EServiceCodes.SC_DND, "");
                this.endCall();
                return;
            }

            _smref.CallingNumber = callingNo;
            _smref.CallingName = display;
            _smref.Headers = headers;
            _smref.ChangeState(EStateId.INCOMING);
        }

    }
    #endregion

    #region ConnectingState
    /// <summary>
    /// Connecting states indicates outgoing call has been initiated and waiting for a response.
    /// </summary>
    internal class CConnectingState : IAbstractState
    {
        public CConnectingState(CStateMachine sm)
            : base(sm)
        {
            Id = EStateId.CONNECTING;
        }

        internal override void OnEntry()
        {
            _smref.Type = ECallType.EDialed;
        }

        internal override void OnExit()
        {
        }

        public override void onReleased()
        {
            _smref.ChangeState(EStateId.RELEASED);
        }

        public override void onAlerting()
        {
            _smref.ChangeState(EStateId.ALERTING);
        }


        public override void onConnect()
        {
            _smref.ChangeState(EStateId.ACTIVE);
        }

        public override bool endCall()
        {
            _smref.ChangeState(EStateId.TERMINATED);
            CallProxy.endCall();
            return base.endCall();
        }

    }
    #endregion

    #region AlertingState
    /// <summary>
    /// Alerting state indicates other side accepts the call. Play ring back tone.
    /// </summary>
    internal class CAlertingState : IAbstractState
    {
        public CAlertingState(CStateMachine sm)
            : base(sm)
        {
            Id = EStateId.ALERTING;
        }

        internal override void OnEntry()
        {
            if (_smref.Config.AudioPlayOutgoing)
                Task.Factory.StartNew(() => MediaProxy.playTone(ETones.EToneRingback), TaskCreationOptions.PreferFairness);
        }

        internal override void OnExit()
        {
            Task.Factory.StartNew(() => MediaProxy.stopTone(), TaskCreationOptions.PreferFairness);
        }

        public override void onConnect()
        {
            _smref.Time = System.DateTime.Now;
            _smref.ChangeState(EStateId.ACTIVE);
        }

        public override void onReleased()
        {
            _smref.ChangeState(EStateId.RELEASED);
        }

        public override bool endCall()
        {
            _smref.ChangeState(EStateId.TERMINATED);
            CallProxy.endCall();
            return base.endCall();
        }
    }
    #endregion

    #region ActiveState
    /// <summary>
    /// Active state indicates conversation. 
    /// </summary>
    internal class CActiveState : IAbstractState
    {
        public CActiveState(CStateMachine sm)
            : base(sm)
        {
            Id = EStateId.ACTIVE;
        }

        internal override void OnEntry()
        {
            _smref.Counting = true;

            foreach (var callItem in CCallManager.Instance[EStateId.INCOMING])
            {
                var call = callItem;

                if (call != null)
                    Task.Factory.StartNew(() => call.MediaProxy.stopTone(), TaskCreationOptions.PreferFairness);
            }

            Task.Factory.StartNew(() => MediaProxy.stopTone(), TaskCreationOptions.PreferFairness);
        }

        internal override void OnExit()
        {
        }

        public override bool endCall()
        {
            _smref.ChangeState(EStateId.TERMINATED);
            CallProxy.endCall();
            return base.endCall();
        }

        public override bool holdCall()
        {
            _smref.HoldRequested = true;
            return CallProxy.holdCall();
        }

        public override bool xferCall(string number)
        {
            return CallProxy.xferCall(number);
        }

        public override bool xferCall(string number, Sip.SipHeader[] headers)
        {
            return CallProxy.xferCall(number, headers);
        }

        public override bool xferCallSession(int partnersession)
        {
            return CallProxy.xferCallSession(partnersession);
        }

        public override void onHoldConfirm()
        {
            // check if Hold requested
            if (_smref.HoldRequested)
            {
                _smref.ChangeState(EStateId.HOLDING);
                // activate pending action if any
                _smref.ActivatePendingAction();
            }
            _smref.HoldRequested = false;
        }

        /// <summary>
        /// 
        /// </summary>
        public override void onReleased()
        {
            _smref.ChangeState(EStateId.RELEASED);
        }

        public override bool connectCallMedia(int targetCallId)
        {
            _smref.IsConference = CallProxy.connectCallMedia(targetCallId);

            return _smref.IsConference;
        }
    }
    #endregion

    #region ReleasedState
    /// <summary>
    /// Released State indicates call has been released from other party (stack) 
    /// and now waiting for destruction. The state machine will be release on 
    /// endCall user request or released timer timeout.
    /// </summary>
    internal class CReleasedState : IAbstractState
    {
        public CReleasedState(CStateMachine sm)
            : base(sm)
        {
            Id = EStateId.RELEASED;
        }

        /// <summary>
        /// Enter release state. If release timer not implemented release call imediately
        /// </summary>
        internal override void OnEntry()
        {
            // Client's don't whant to hear this tone. Simply end the call
            // MediaProxy.playTone(ETones.EToneCongestion);
            //bool success = _smref.StartTimer(ETimerType.ERELEASED);
            //if (!success) _smref.Destroy();
            _smref.Destroy();
        }

        internal override void OnExit()
        {
            //MediaProxy.stopTone();
            //_smref.StopAllTimers();
        }

        public override bool endCall()
        {
            // try once more (might not be needed)!
            CallProxy.endCall();
            // destroy it!
            _smref.Destroy();
            return true;
        }

        public override bool releasedTimerExpired(int sessionId)
        {
            _smref.Destroy();
            return true;
        }

        /// <summary>
        /// If by any chance this lost event comes here: destroy State machine
        /// </summary>
        public override void onReleased()
        {
            _smref.Destroy();
        }
    }
    #endregion

    #region TerminatedState
    /// <summary>
    /// Released State indicates call has been released by user and waiting for destruction.
    /// The state machine will be destroyed on onReleased event from stack or on released timer 
    /// timeout
    /// </summary>
    internal class CTerminatedState : IAbstractState
    {
        public CTerminatedState(CStateMachine sm)
            : base(sm)
        {
            Id = EStateId.TERMINATED;
        }

        /// <summary>
        /// Enter release state. If release timer not implemented release call imediately
        /// </summary>
        internal override void OnEntry()
        {
            bool success = _smref.StartTimer(ETimerType.ERELEASED);
            if (!success) _smref.Destroy();
        }

        internal override void OnExit()
        {
            _smref.StopAllTimers();
        }

        public override bool endCall()
        {
            CallProxy.endCall();
            return true;
        }

        public override bool releasedTimerExpired(int sessionId)
        {
            _smref.Destroy();
            return true;
        }

        public override void onAlerting()
        {
            CallProxy.endCall();
        }

        public override void onConnect()
        {
            CallProxy.endCall();
        }

        /// <summary>
        /// Destroy state machine 
        /// </summary>
        public override void onReleased()
        {
            _smref.Destroy();
        }
    }
    #endregion

    #region IncomingState
    /// <summary>
    /// Incoming state indicates incoming call. Check CFx and DND features. Start ringer. 
    /// </summary>
    internal class CIncomingState : IAbstractState
    {
        public CIncomingState(CStateMachine sm)
            : base(sm)
        {
            Id = EStateId.INCOMING;
        }

        internal override void OnEntry()
        {
            // set incoming call flags
            _smref.Incoming = true;

            int sessionId = SessionId;

            // drop call if pause flag
            if (_smref.Config.PauseFlag)
            {
                this.endCall();

                return;
            }

            // Start no response timer
            _smref.StartTimer(ETimerType.ENORESPONSE);

            CallProxy.alerted();
            _smref.Type = ECallType.EMissed;
            
            // Don't play any sounds while active calls persist
            if (_smref.Config.AudioPlayOnIncoming && (this._smref.NumberOfCalls == 1 || _smref.Config.AudioPlayOnIncomingAndActive))
                Task.Factory.StartNew(() => MediaProxy.playTone(ETones.EToneRing), TaskCreationOptions.PreferFairness);

            // if CFNR active start timer
            if (_smref.Config.CFNRFlag)
            {
                _smref.StartTimer(ETimerType.ENOREPLY);
            }

            // auto answer call (if single call)
            if ((_smref.Config.AAFlag == true) && (_smref.NumberOfCalls == 1))
            {
                this.acceptCall();
            }
        }

        internal override void OnExit()
        {
            Task.Factory.StartNew(() => MediaProxy.stopTone(), TaskCreationOptions.PreferFairness);
            _smref.StopAllTimers();
        }

        public override bool acceptCall()
        {
            _smref.Type = ECallType.EReceived;
            _smref.Time = System.DateTime.Now;

            CallProxy.acceptCall();
            _smref.ChangeState(EStateId.ACTIVE);
            return true;
        }

        public override void onReleased()
        {
            _smref.ChangeState(EStateId.RELEASED);
        }

        public override bool xferCall(string number)
        {
            // In fact this is not Tranfser. It's Deflect => redirect...
            return CallProxy.serviceRequest((int)EServiceCodes.SC_CD, number);
        }

        public override bool endCall()
        {
            _smref.ChangeState(EStateId.TERMINATED);
            CallProxy.endCall();
            return base.endCall();
        }

        /// <summary>
        /// No reply timer expired. Send service code to call proxy.
        /// </summary>
        /// <param name="sessionId"></param>
        /// <returns></returns>
        public override bool noReplyTimerExpired(int sessionId)
        {
            CallProxy.serviceRequest((int)EServiceCodes.SC_CFNR, _smref.Config.CFUNumber);
            return true;
        }

        /// <summary>
        /// Response timer expired. Releasing the call...
        /// </summary>
        /// <param name="sessionId"></param>
        /// <returns></returns>
        public override bool noResponseTimerExpired(int sessionId)
        {
            _smref.ChangeState(EStateId.TERMINATED);
            CallProxy.endCall();
            return true;
        }
    }
    #endregion

    #region HoldingState
    /// <summary>
    /// Holding state indicates call is hodling.
    /// </summary>
    internal class CHoldingState : IAbstractState
    {
        public CHoldingState(CStateMachine sm)
            : base(sm)
        {
            Id = EStateId.HOLDING;
        }

        internal override void OnEntry()
        {
        }

        internal override void OnExit()
        {
        }

        public override bool retrieveCall()
        {
            _smref.RetrieveRequested = true;
            CallProxy.retrieveCall();
            _smref.ChangeState(EStateId.ACTIVE);
            return true;
        }

        // TODO implement in stack interface
        //public override onRetrieveConfirm()
        //{
        //  if (_smref.RetrieveRequested)
        //  {
        //    _smref.changeState(EStateId.ACTIVE);
        //  }
        //  _smref.RetrieveRequested = false;
        //}

        public override void onReleased()
        {
            _smref.ChangeState(EStateId.RELEASED);
        }

        public override bool endCall()
        {
            CallProxy.endCall();
            _smref.ChangeState(EStateId.TERMINATED);
            return base.endCall();
        }

        public override bool connectCallMedia(int targetCallId)
        {
            retrieveCall();

            _smref.IsConference = CallProxy.connectCallMedia(targetCallId);
            return _smref.IsConference;
        }
    }
    #endregion

} // namespace Sipek
