using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using ContactPoint.Common;
using ContactPoint.Common.CallManager;
using ContactPoint.Common.Contacts;

namespace ContactPoint.Core.CallManager
{
    internal class Call : ICall
    {
        private readonly AsyncCallback _emptyDelegateAsyncCallback;
        private readonly AsyncCallback _callRemovedAsyncCallback;
        private readonly HeaderCollection _headers;
        private readonly ConcurrentDictionary<string, object> _tags = new ConcurrentDictionary<string, object>();
        private readonly LinkedList<ICallStateInfo> _stateHistory = new LinkedList<ICallStateInfo>();
        private string _name = String.Empty;
        private TimeSpan _activeStartDuration = TimeSpan.Zero;
        private CallState _currentState;
        private IContact _contact;

        protected readonly CallManager CallManager;

        public Guid Id { get; }
        public int SessionId { get; protected set; }
        public string Number { get; protected internal set; }
        public string Info { get; internal set; }
        public bool IsIncoming { get; }
        public CallAction LastUserAction { get; internal set; } = CallAction.NULL;
        public bool IsDisposed { get; internal set; }
        public IHeaderCollection Headers => _headers;
        public IDictionary<string, object> Tags => _tags;
        public IEnumerable<ICallStateInfo> States => _stateHistory;
        public ICallStateInfo LastStateInfo => _stateHistory.Count > 0 ? _stateHistory.Last.Value : CallStateInfo.Default;
        public CallState LastState => LastStateInfo.State;
        public TimeSpan ActiveDuration => _activeStartDuration != TimeSpan.Zero ? Duration - _activeStartDuration : TimeSpan.Zero;

        public IContact Contact
        {
            get { return _contact; }
            internal set
            {
                _contact = value;

                if (!string.IsNullOrWhiteSpace(_contact?.ShowedName))
                {
                    _name = _contact.ShowedName;
                }

                CallManager.RaiseCallInfoChanged(this);
            }
        }

        public CallState State
        {
            get
            {
                return _currentState;
            }
            internal set
            {
                var duration = Duration;

                // Set call state but don't raise event. Event raises by CallManager
                _stateHistory.AddLast(new CallStateInfo
                {
                    State = _currentState,
                    Duration = duration - _stateHistory.Select(x => x.Duration).LastOrDefault()
                });

                _currentState = value;

                if (_activeStartDuration == TimeSpan.Zero && value == CallState.ACTIVE)
                {
                    _activeStartDuration = duration;
                }
            }
        }

        public string Codec
        {
            get
            {
                if (CallManager.Sip.SipekResources.CallManager.CallList.ContainsKey(SessionId))
                    return CallManager.Sip.SipekResources.CallManager[SessionId].Codec;

                return String.Empty;
            }
        }

        public string Name
        {
            get { return _name; }
            set
            { 
                _name = value;
                CallManager.RaiseCallInfoChanged(this);
            }
        }

        private int _line = -1;
        public int Line
        {
            get { return _line; }
            set
            {
                if (_line == -1)
                {
                    _line = value;
                    CallManager.RaiseCallInfoChanged(this);
                }
            }
        }


        private TimeSpan _duration = TimeSpan.FromSeconds(0);
        public TimeSpan Duration
        {
            get { return _duration; }
            internal set
            {
                _duration = value;
                CallManager.RaiseCallDurationChanged(this);
            }
        }

        public bool IsInConference
        {
            get
            {
                if (CallManager.Sip.SipekResources.CallManager.CallList.ContainsKey(SessionId))
                    return CallManager.Sip.SipekResources.CallManager[SessionId].IsConference;

                return false;
            }
            set
            {
                if (CallManager.Sip.SipekResources.CallManager.CallList.ContainsKey(SessionId))
                    CallManager.Sip.SipekResources.CallManager[SessionId].IsConference = value;
            }
        }

        /// <summary>
        /// Occurs when call state is changed
        /// </summary>
        public event EmptyDelegate OnStateChanged;
        
        /// <summary>
        /// Occurs when information about call is changed
        /// </summary>
        public event EmptyDelegate OnInfoChanged;

        /// <summary>
        /// Occurs when call dropped
        /// </summary>
        public event Action<CallRemoveReason> OnRemoved;

        /// <summary>
        /// Duration increased
        /// </summary>
        public event EmptyDelegate OnDurationChanged;

        internal Call(CallManager callManager, int sessionId)
            : this(callManager, sessionId, null)
        { }

        internal Call(CallManager callManager, int sessionId, string number)
            : this(callManager, sessionId, number, null)
        { }

        internal Call(CallManager callManager, int sessionId, string number, string info)
            : this(callManager, sessionId, number, info, new List<KeyValuePair<string, string>>())
        { }

        internal Call(CallManager callManager, int sessionId, string number, string info, IEnumerable<KeyValuePair<string, string>> headers)
        {
            Id = Guid.NewGuid();

            _emptyDelegateAsyncCallback = EmptyDelegateAsyncCallback;
            _callRemovedAsyncCallback = CallRemovedAsyncCallback;

            _headers = new HeaderCollection(headers);

            CallManager = callManager;
            SessionId = sessionId;

            if (SessionId >= 0)
            {
                try
                {
                    if (number == null &&
                        ((SIP.SIP) CallManager.Core.Sip).SipekResources.CallManager[sessionId] != null)
                    {
                        Number = ((SIP.SIP) CallManager.Core.Sip).SipekResources.CallManager[sessionId].CallingNumber;
                        IsIncoming = ((SIP.SIP) CallManager.Core.Sip).SipekResources.CallManager[SessionId].Incoming;
                        _contact = CallManager.Core.ContactsManager.GetContactByPhoneNumber(Number);
                    }
                    else
                    {
                        Number = "";
                        IsIncoming = false;
                    }
                }
                catch (Exception e)
                {
                    Logger.LogWarn(e, "Invalid flow of call. SessionId is invalid");
                }
            }

            Info = info ?? "";

            UpdateCallerName();
        }

        /// <summary>
        /// Drop this call
        /// </summary>
        public virtual void Drop()
        {
            CallManager.DropCall(this);
        }

        /// <summary>
        /// Answer this call
        /// </summary>
        public virtual void Answer()
        {
            CallManager.AnswerCall(this);
        }

        /// <summary>
        /// Hold this call
        /// </summary>
        public virtual void Hold()
        {
            CallManager.HoldCall(this);
        }

        /// <summary>
        /// UnHold this call
        /// </summary>
        public virtual void UnHold()
        {
            CallManager.UnHoldCall(this);
        }

        /// <summary>
        /// Toggle hold state on call
        /// </summary>
        public virtual void ToggleHold()
        {
            CallManager.ToggleHoldCall(this);
        }

        /// <summary>
        /// Send DTMF to this call
        /// </summary>
        /// <param name="digits">Digits to send</param>
        public virtual void SendDTMF(string digits)
        {
            CallManager.SendDTMF(this, digits);
        }

        /// <summary>
        /// Transfer this call
        /// </summary>
        /// <param name="number">Destination number</param>
        public virtual void Transfer(string number)
        {
            CallManager.TransferCall(this, number);
        }

        /// <summary>
        /// Transfer this call to another active call
        /// </summary>
        /// <param name="destCall">Destination call</param>
        public virtual void TransferAttended(ICall destCall)
        {
            CallManager.TransferAttendedCall(this, destCall);
        }

        /// <summary>
        /// Add call headers
        /// </summary>
        /// <param name="values">Header collection</param>
        public void AddHeaders(IEnumerable<KeyValuePair<string, string>> values)
        {
            _headers.AddRange(values);

            UpdateCallerName();
        }

        /// <summary>
        /// Raise OnStateChanged event
        /// </summary>
        public void RaiseStateChanged()
        {
            SafeRaiseEvent(OnStateChanged);
        }

        /// <summary>
        /// Raise OnInfoChanged event
        /// </summary>
        public void RaiseInfoChanged()
        {
            SafeRaiseEvent(OnInfoChanged);
        }

        /// <summary>
        /// Raise OnRemoved event
        /// </summary>
        /// <param name="reason">Remove reason</param>
        public void RaiseRemoved(CallRemoveReason reason)
        {
            SafeRaiseEvent(OnRemoved, reason);
        }

        /// <summary>
        /// Raise OnDurationChanged event
        /// </summary>
        public void RaiseDurationChanged()
        {
            SafeRaiseEvent(OnDurationChanged);
        }

        public override string ToString()
        {
            return String.Format("{0} ({1})", Number, SessionId);
        }

        private void UpdateCallerName()
        {
            if (string.IsNullOrEmpty(_name))
            {
                var from = _headers["From"];

                if (from != null)
                {
                    try
                    {
                        var match = Regex.Match(from.Value, "^\"(.*)\"");

                        if (match.Captures.Count > 0)
                            Name = match.Groups[1].Value;
                    }
                    catch (Exception e)
                    {
                        Logger.LogWarn(e, "Can't parse callerid name for {0}", from.Value);
                    }
                }
            }
        }

        private void SafeRaiseEvent(EmptyDelegate del)
        {
            if (del == null) return;

            var invocationList = del.GetInvocationList();

            foreach (var current in invocationList)
                ((EmptyDelegate)current).BeginInvoke(_emptyDelegateAsyncCallback, current);
        }

        private void SafeRaiseEvent(Action<CallRemoveReason> del, CallRemoveReason reason)
        {
            if (del == null) return;

            var invocationList = del.GetInvocationList();

            foreach (var current in invocationList)
                ((Action<CallRemoveReason>)current).BeginInvoke(reason, _callRemovedAsyncCallback, current);
        }

        private void EmptyDelegateAsyncCallback(IAsyncResult result)
        {
            try
            {
                ((EmptyDelegate)result.AsyncState).EndInvoke(result);
            }
            catch (Exception ex)
            {
                Logger.LogWarn(ex);
            }
        }

        private void CallRemovedAsyncCallback(IAsyncResult result)
        {
            try
            {
                ((Action<CallRemoveReason>)result.AsyncState).EndInvoke(result);
            }
            catch (Exception ex)
            {
                Logger.LogWarn(ex);
            }
        }
    }
}
