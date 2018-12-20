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

using System;
using System.Threading.Tasks;
using Sipek.Common;
using System.Collections.Generic;
using System.ComponentModel;

namespace Sipek.Common.CallControl
{

    /// <summary>
    /// CStateMachine class is a telephony data container for signle call. It maintains call state, 
    /// communicates with signaling via proxy and informs about events from signaling.
    /// A Finite State Machine is implemented in State design pattern!
    /// </summary>
    public class CStateMachine : IStateMachine
    {
        #region Variables

        private IAbstractState _state;
        // State instances....
        private CIdleState _stateIdle;
        private CConnectingState _stateCalling;
        private CAlertingState _stateAlerting;
        private CActiveState _stateActive;
        private CReleasedState _stateReleased;
        private CIncomingState _stateIncoming;
        private CHoldingState _stateHolding;
        private CTerminatedState _stateTerminated;
        // call properties
        private ECallType _callType;
        private TimeSpan _duration;
        private DateTime _timestamp;
        private CCallManager _manager;
        // Timers
        private ITimer _noreplyTimer;
        private ITimer _noresponseTimer;

        private int _session = -1;
        private ICallProxyInterface _sigProxy;
        private IMediaProxyInterface _mediaProxy;
        private string _callingNumber = "";
        private string _callingName = "";
        private bool _incoming = false;
        private bool _counting = false; // if duration counter is started
        private bool _holdRequested = false;
        private bool _retrieveRequested = false;
        private bool _disableStateNotifications = false;
        private bool _isConference = false;

        #endregion Variables

        #region Properties
        /// <summary>
        /// A reference to CCallManager instance
        /// </summary>
        public CCallManager Manager
        {
            get { return _manager; }
        }

        /// <summary>
        /// Call/Session identification
        /// </summary>
        public override int Session
        {
            get { return _session; }
            set
            {
                _session = value;
                // don't forget to set proxy sessionId in case of incoming call!
                this.CallProxy.SessionId = value;
                this._mediaProxy.SessionId = value;
            }
        }

        /// <summary>
        /// Calling number property
        /// </summary>
        public override string CallingNumber
        {
            get { return _callingNumber; }
            set { _callingNumber = value; }
        }

        /// <summary>
        /// Calling name property
        /// </summary>
        public override string CallingName
        {
            get { return _callingName; }
            set { _callingName = value; }
        }

        /// <summary>
        /// Incoming call flag
        /// </summary>
        public override bool Incoming
        {
            get { return _incoming; }
            set { _incoming = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public override EStateId StateId
        {
            get { return _state.Id; }
        }


        /// <summary>
        /// ???
        /// </summary>
        internal override bool Counting
        {
            get { return _counting; }
            set { _counting = value; }
        }

        /// <summary>
        /// Duration of a call
        /// </summary>
        public override TimeSpan Duration
        {
            set { _duration = value; }
            get { return _duration; }
        }

        /// <summary>
        /// Calculate call duration
        /// </summary>
        public override TimeSpan RuntimeDuration
        {
            get
            {
                if (true == Counting)
                {
                    return DateTime.Now.Subtract(Time);
                }
                else
                {
                    return Duration;
                }
            }
        }

        public override bool IsConference
        {
            get { return _isConference; }
            set { _isConference = value; }
        }

        /// <summary>
        /// Current State of the state machine
        /// </summary>
        internal override IAbstractState State
        {
            get { return _state; }
        }

        /// <summary>
        /// Check for null state machine
        /// </summary>
        public override bool IsNull
        {
            get { return false; }
        }

        private IEnumerable<KeyValuePair<string, string>> _headers;
        public override IEnumerable<KeyValuePair<string, string>> Headers
        {
            get { return _headers; }
            set { _headers = value; }
        }

        /// <summary>
        /// Signaling proxy instance (separatelly created for each call)
        /// </summary>
        internal override ICallProxyInterface CallProxy
        {
            get { return _sigProxy; }
        }

        /// <summary>
        /// Media proxy instance getter for handling tones
        /// </summary>
        internal override IMediaProxyInterface MediaProxy
        {
            get { return _mediaProxy; }
        }

        /// <summary>
        /// Call type property for Call log
        /// </summary>
        internal override ECallType Type
        {
            get { return _callType; }
            set { _callType = value; }
        }

        /// <summary>
        /// Timestamp of a call
        /// </summary>
        internal override DateTime Time
        {
            set { _timestamp = value; }
            get { return _timestamp; }
        }

        /// <summary>
        /// Has been call hold requested
        /// </summary>
        internal override bool HoldRequested
        {
            get { return _holdRequested; }
            set { _holdRequested = value; }
        }

        /// <summary>
        /// Has been call retrieve requested
        /// </summary>
        internal override bool RetrieveRequested
        {
            get { return _retrieveRequested; }
            set { _retrieveRequested = value; }
        }

        /// <summary>
        /// Data access instance
        /// </summary>
        internal override IConfiguratorInterface Config
        {
            get { return _manager.Config; }
        }

        /// <summary>
        /// Get Current codec for this call
        /// </summary>
        /// <returns>Codec name</returns>
        public override string Codec
        {
            get { return _sigProxy.getCurrentCodec(); }
        }

        internal override bool DisableStateNotifications
        {
            get { return _disableStateNotifications; }
            set { _disableStateNotifications = value; }
        }

        internal override int NumberOfCalls
        {
            get { return Manager.Count; }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Call/Session constructor. Initializes call states, creates signaling proxy, initialize time,
        /// initialize timers.
        /// </summary>
        public CStateMachine()
        {
            // store manager reference...
            _manager = CCallManager.Instance;

            // create call proxy
            _sigProxy = _manager.StackProxy.createCallProxy();

            // create media proxy for this call
            _mediaProxy = new Sipek.Sip.pjsipMediaPlayerProxy();

            // initialize call states
            _stateIdle = new CIdleState(this);
            _stateAlerting = new CAlertingState(this);
            _stateActive = new CActiveState(this);
            _stateCalling = new CConnectingState(this);
            _stateReleased = new CReleasedState(this);
            _stateIncoming = new CIncomingState(this);
            _stateHolding = new CHoldingState(this);
            _stateTerminated = new CTerminatedState(this);
            // change state
            _state = _stateIdle;

            // initialize data
            Time = DateTime.Now;
            Duration = TimeSpan.Zero;

            // Initialize timers
            if (null != _manager)
            {
                _noreplyTimer = _manager.Factory.CreateTimer();
                _noreplyTimer.Interval = 60000; // hardcoded to 60s
                _noreplyTimer.Elapsed = new TimerExpiredCallback(_noreplyTimer_Elapsed);

                //_releasedTimer = _manager.Factory.createTimer();
                //_releasedTimer.Interval = 1000; // hardcoded to 1s
                //_releasedTimer.Elapsed = new TimerExpiredCallback(_releasedTimer_Elapsed);

                _noresponseTimer = _manager.Factory.CreateTimer();
                _noresponseTimer.Interval = 60000; // hardcoded to 60s
                _noresponseTimer.Elapsed = new TimerExpiredCallback(_noresponseTimer_Elapsed);
            }
        }

        #endregion Constructor

        #region Private Methods
        /// <summary>
        /// Change state
        /// </summary>
        /// <param name="state">instance of state to change to</param>
        private void ChangeState(IAbstractState state)
        {
            _state.OnExit();
            _state = state;
            _state.OnEntry();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _noreplyTimer_Elapsed(object sender, EventArgs e)
        {
            // Disable noreply functionality - users don't whant to see hangups
            //State.noReplyTimerExpired(this.Session);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _releasedTimer_Elapsed(object sender, EventArgs e)
        {
            //State.releasedTimerExpired(this.Session);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _noresponseTimer_Elapsed(object sender, EventArgs e)
        {
            State.noResponseTimerExpired(this.Session);
        }

        ///////////////////////////////////////////////////////////////////////////////////
        // Timers
        /// <summary>
        /// Start timer by timer type
        /// </summary>
        /// <param name="ttype">timer type</param>
        internal override bool StartTimer(ETimerType ttype)
        {
            bool success = false;
            switch (ttype)
            {
                case ETimerType.ENOREPLY:
                    success = _noreplyTimer.Start();
                    break;
                //case ETimerType.ERELEASED:
                //    success = _releasedTimer.Start();
                //    break;
                case ETimerType.ENORESPONSE:
                    success = _noresponseTimer.Start();
                    break;
            }
            return success;
        }

        /// <summary>
        /// Stop timer by timer type
        /// </summary>
        /// <param name="ttype">timer type</param>
        internal override bool StopTimer(ETimerType ttype)
        {
            bool success = false;
            switch (ttype)
            {
                case ETimerType.ENOREPLY:
                    success = _noreplyTimer.Stop();
                    break;
                //case ETimerType.ERELEASED:
                //    success = _releasedTimer.Stop();
                //    break;
                case ETimerType.ENORESPONSE:
                    success = _noresponseTimer.Stop();
                    break;
            }
            return success;
        }

        /// <summary>
        /// Stop all timers...
        /// </summary>
        internal override void StopAllTimers()
        {
            _noreplyTimer.Stop();
            _noresponseTimer.Stop();
        }

        /// <summary>
        /// Run queued requests
        /// </summary>
        internal override void ActivatePendingAction()
        {
            Manager.activatePendingAction();
        }

        /// <summary>
        /// Change state by state id
        /// </summary>
        /// <param name="stateId">state id</param>
        internal override void ChangeState(EStateId stateId)
        {
            switch (stateId)
            {
                case EStateId.IDLE: ChangeState(_stateIdle); break;
                case EStateId.CONNECTING: ChangeState(_stateCalling); break;
                case EStateId.ALERTING: ChangeState(_stateAlerting); break;
                case EStateId.ACTIVE: ChangeState(_stateActive); break;
                case EStateId.RELEASED: ChangeState(_stateReleased); break;
                case EStateId.INCOMING: ChangeState(_stateIncoming); break;
                case EStateId.HOLDING: ChangeState(_stateHolding); break;
                case EStateId.TERMINATED: ChangeState(_stateTerminated); break;
            }
            // inform manager 
            if ((null != _manager) && (Session != -1) && (DisableStateNotifications == false)) _manager.updateGui(this.Session);
        }
        #endregion

        #region Public Methods

        /// <summary>
        /// Destroy call. Calculate call duration time, edit call log, destroy session.
        /// </summary>
        public override void Destroy()
        {
            // stop tones & timers
            StopAllTimers();

            Task.Factory.StartNew(() => MediaProxy.stopTone(), TaskCreationOptions.PreferFairness);

            // Calculate timing
            if (true == Counting)
            {
                Duration = DateTime.Now.Subtract(Time);
            }
            Counting = false;
            // reset data
            CallingNumber = "";
            Incoming = false;
            ChangeState(EStateId.IDLE);
            if (null != _manager) _manager.DestroySession(Session);
        }

        #endregion Methods

        #region Obsolete Methods

        [Obsolete("Use Destroy() method instead")]
        public override void destroy()
        {
            this.Destroy();
        }
        #endregion

    }

} // namespace Sipek.Common.CallControl
