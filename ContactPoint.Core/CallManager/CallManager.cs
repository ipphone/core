using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Threading;
using ContactPoint.Common;
using ContactPoint.Common.SIP.Account;
using Sipek.Common;
using System.Threading;
using Sipek.Sip;
using System.Collections.Concurrent;

namespace ContactPoint.Core.CallManager
{
    internal delegate void RealCallDelegate(Call call);

    internal class CallManager : ICallManager
    {
        public static int LINES_MAXCOUNT = 5;

        private static int OPERATION_WAIT_TIMEOUT = 200;
        private static int OPERATION_WAIT_MAXTIMEOUT = 5000;

        private readonly AsyncCallback _raiseCallEventCallback;
        private readonly AsyncCallback _raiseCallRemovedEventCallback;
        private readonly AsyncCallback _deferredOperationCallback;
        private readonly Core _core;
        private readonly SIP.SIP _sip;
        private readonly System.Windows.Forms.Timer _durationTimer;
        private readonly System.Windows.Forms.Timer _internalTimer;
        private readonly List<Call> _calls = new List<Call>();
        private readonly Call[] _lines = new Call[LINES_MAXCOUNT + 1];
        private readonly ConcurrentQueue<CallOperation> _defferedOperations = new ConcurrentQueue<CallOperation>();
        private readonly Dispatcher _dispatcher;

        private bool _isConferenceActive;

        /// <summary>
        /// Core object
        /// </summary>
        public ICore Core => _core;

        /// <summary>
        /// Call indexer
        /// </summary>
        /// <param name="sessionId">Call session Id</param>
        /// <returns>Call object or null if not found</returns>
        public ICall this[int sessionId] => FindCallBySessionId(sessionId);

        /// <summary>
        /// Currently active call. Must be set by UI because this property indicating "currently used call in UI"! 
        /// </summary>
        public ICall ActiveCall { get; set; }

        /// <summary>
        /// Current count of calls
        /// </summary>
        public int Count => _sip.SipekResources.CallManager.CallList.Count;

        /// <summary>
        /// Occurs when incoming call
        /// </summary>
        public event CallDelegate OnIncomingCall;

        /// <summary>
        /// Occurs when call state is changed
        /// </summary>
        public event CallDelegate OnCallStateChanged;

        /// <summary>
        /// Occurs when information about call is changed
        /// </summary>
        public event CallDelegate OnCallInfoChanged;

        /// <summary>
        /// Occurs when call dropped
        /// </summary>
        public event CallRemovedDelegate OnCallRemoved;

        internal SIP.SIP Sip => _sip;

        /// <summary>
        /// Class to processing calls for UI.
        /// Create it in main thread!!!
        /// </summary>
        /// <param name="core">Core instance</param>
        internal CallManager(Core core)
        {
            _dispatcher = Dispatcher.CurrentDispatcher;
            _core = core;
            _sip = (SIP.SIP)core.Sip;
            _raiseCallEventCallback = RaiseCallEventCallback;
            _raiseCallRemovedEventCallback = RaiseCallRemovedEventCallback;
            _deferredOperationCallback = DeferredOperationCallback;

            _durationTimer = new System.Windows.Forms.Timer { Interval = 1000 };
            _durationTimer.Tick += DurationTimerTick;
            _durationTimer.Start();

            _internalTimer = new System.Windows.Forms.Timer { Interval = 100 };
            _internalTimer.Tick += InternalTimerTick;
            _internalTimer.Start();

            _sip.SipekResources.CallManager.IncomingCallNotification += OnIncomingCallNotification;
            _sip.SipekResources.CallManager.CallStateRefresh += OnCallStateRefresh;
            _sip.SipekResources.CallManager.CallMediaStateChanged += OnCallMediaStateChanged;
        }

        ~CallManager()
        {
            _durationTimer.Stop();
            _internalTimer.Stop();
        }

        #region Call control functions

        /// <summary>
        /// Drop specified call
        /// </summary>
        /// <param name="call">Call to drop</param>
        public void DropCall(ICall call)
        {
            if (call == null) return;

            if (Monitor.TryEnter(call, OPERATION_WAIT_TIMEOUT))
            {
                try { DropCallInternal(call); }
                finally { Monitor.Exit(call); }
            }
            else
                AddDeferredOperation(new Action<ICall>(DropCall), call);
        }

        private void DropCallInternal(ICall call)
        {
            Logger.LogNotice($"Dropping call {call.Number}.");

            Call callObj = (Call)call;
            callObj.LastUserAction = CallAction.Hangup;

            if (callObj.SessionId < 0)
                RemoveCallInternal(callObj, CallRemoveReason.NULL);
            else
                _sip.SipekResources.CallManager.OnUserRelease(callObj.SessionId);
        }

        /// <summary>
        /// Answer specified call
        /// </summary>
        /// <param name="call">Call to answer</param>
        public void AnswerCall(ICall call)
        {
            if (call == null) return;

            if (Monitor.TryEnter(call, OPERATION_WAIT_TIMEOUT))
            {
                try { AnswerCallInternal(call); }
                finally { Monitor.Exit(call); }
            }
            else
                AddDeferredOperation(new Action<ICall>(AnswerCall), call);
        }

        private void AnswerCallInternal(ICall call)
        {
            Logger.LogNotice($"Answering call {call.Number}.");

            Call callObj = (Call)call;
            callObj.LastUserAction = CallAction.Answer;

            _sip.SipekResources.CallManager.OnUserAnswer(callObj.SessionId);
        }

        /// <summary>
        /// Put call on hold
        /// </summary>
        /// <param name="call">Call to put on hold</param>
        public void HoldCall(ICall call)
        {
            if (call == null) return;

            if (Monitor.TryEnter(call, OPERATION_WAIT_TIMEOUT))
            {
                try { HoldCallInternal(call); }
                finally { Monitor.Exit(call); }
            }
            else
                AddDeferredOperation(new Action<ICall>(HoldCall), call);
        }

        private void HoldCallInternal(ICall call)
        {
            Logger.LogNotice($"Putting call {call.Number} on hold.");

            if (call.State != CallState.HOLDING)
                _sip.SipekResources.CallManager.OnUserHoldRetrieve(call.SessionId);
        }

        /// <summary>
        /// Retrive call from hold
        /// </summary>
        /// <param name="call">Call to retrive from hold</param>
        public void UnHoldCall(ICall call)
        {
            if (call == null) return;

            if (Monitor.TryEnter(call, OPERATION_WAIT_TIMEOUT))
            {
                try { UnHoldCallInternal(call); }
                finally { Monitor.Exit(call); }
            }
            else
                AddDeferredOperation(new Action<ICall>(UnHoldCall), call);
        }

        private void UnHoldCallInternal(ICall call)
        {
            Logger.LogNotice($"Taking call {call.Number} from hold.");

            if (call.State == CallState.HOLDING)
                _sip.SipekResources.CallManager.OnUserHoldRetrieve(call.SessionId);
        }

        /// <summary>
        /// Toggle hold state on call
        /// </summary>
        /// <param name="call">Call to hold or unhold</param>
        public void ToggleHoldCall(ICall call)
        {
            if (call == null) return;

            if (Monitor.TryEnter(call, OPERATION_WAIT_TIMEOUT))
            {
                try { ToggleHoldCallInternal(call); }
                finally { Monitor.Exit(call); }
            }
            else
                AddDeferredOperation(new Action<ICall>(ToggleHoldCall), call);
        }

        private void ToggleHoldCallInternal(ICall call)
        {
            Logger.LogNotice($"Toggle call {call.Number} hold.");

            _sip.SipekResources.CallManager.OnUserHoldRetrieve(call.SessionId);
        }

        /// <summary>
        /// Send DTMF to call with default DTMF mode
        /// </summary>
        /// <param name="call">Call to send DTMF</param>
        /// <param name="digits">Digits to send</param>
        public void SendDTMF(ICall call, string digits)
        {
            if (Monitor.TryEnter(call, OPERATION_WAIT_TIMEOUT))
            {
                Logger.LogNotice(String.Format("Sending DTMF {1} on call {0}", call.Number, digits));

                try { _sip.SipekResources.CallManager.OnUserDialDigit(call.SessionId, digits, _sip.SipekResources.Configurator.DtmfMode); }
                finally { Monitor.Exit(call); }
            }
            else
                AddDeferredOperation(new Action<ICall, string>(SendDTMF), call, digits);
        }

        public ICall MakeCall(string number)
        {
            return MakeCall(number, null);
        }

        /// <summary>
        /// Create new call to specified number
        /// </summary>
        /// <param name="number">Number to call</param>
        /// <param name="headers">Headers to pass with INVITE</param>
        /// <returns>New call</returns>
        public ICall MakeCall(string number, IEnumerable<KeyValuePair<string, string>> headers)
        {
            if (string.IsNullOrWhiteSpace(number))
                throw new ArgumentException("Phone number is empty or null. Skipping call request.", "number");

            // Remove all non-numeric symbols from number
            number = number.ToLower();

            number = number.Replace("sip:", ""); // it can be added while calling from browser

            while (number.IndexOf(" ", StringComparison.Ordinal) >= 0) number = number.Replace(" ", "");
            while (number.IndexOf("(", StringComparison.Ordinal) >= 0) number = number.Replace("(", "");
            while (number.IndexOf(")", StringComparison.Ordinal) >= 0) number = number.Replace(")", "");
            while (number.IndexOf("-", StringComparison.Ordinal) >= 0) number = number.Replace("-", "");
            while (number.IndexOf("_", StringComparison.Ordinal) >= 0) number = number.Replace("_", "");

            Logger.LogNotice("CallManager: Trying to call " + number);

            // Check if current status is NA then make user Available
            //if (this._sip.Account.PresenceStatus.Code == CallService.Common.SIP.Account.PresenceStatusCode.NotAvailable)
            //    this._sip.Account.PresenceStatus = new CallService.Common.SIP.Account.PresenceStatus(CallService.Common.SIP.Account.PresenceStatusCode.Available);

            // Lock calls collection to ensure that while we check all, we don't receive any new calls
            lock (_calls)
            {
                // We only support 5 parallel lines
                if (_calls.Count < LINES_MAXCOUNT)
                {
                    // Put active calls on hold
                    foreach (var call in _calls)
                    {
                        if (call.State == CallState.ACTIVE)
                            call.Hold();
                    }

                    var newCall = new CallWrapper(this, number)
                    {
                        LastUserAction = CallAction.Make,
                        State = CallState.CONNECTING
                    };

                    var headersCollection = new List<SipHeader>();
                    if (headers != null)
                        headersCollection.AddRange(headers.Select(header => new SipHeader() { name = header.Key, value = header.Value }));

                    if (!AssignLineForCall(newCall))
                    {
                        Logger.LogNotice($"Can't assign line for new call {newCall.Number}. Lines count exceeds.");
                        throw new InvalidOperationException("Lines count exceeds.");
                    }

                    _calls.Add(newCall);

                    RaiseCallStateChanged(newCall);

                    _dispatcher.BeginInvoke(new Action(() => DoMakeCallAsync(newCall, number, headersCollection.ToArray())));

                    return newCall;
                }
            }

            return null;
        }

        /// <summary>
        /// Transfer call to number
        /// </summary>
        /// <param name="call">Call to transfer</param>
        /// <param name="number">Destination number</param>
        public void TransferCall(ICall call, string number)
        {
            if (call == null)
            {
                return;
            }

            var thisCall = (Call)call;
            if (Monitor.TryEnter(call, OPERATION_WAIT_TIMEOUT))
            {
                try
                {
                    Logger.LogNotice($"Transfering call {call.Number} on to {number}.");
                    thisCall.LastUserAction = CallAction.Transfer;

                    var headers = new List<SipHeader>();
                    foreach (var header in thisCall.Headers.Where(x => x.Name.StartsWith("x-", StringComparison.OrdinalIgnoreCase)))
                    {
                        headers.Add(new SipHeader { name = header.Name, value = header.Value });
                    }

                    _sip.SipekResources.CallManager.OnUserTransfer(call.SessionId, number, headers.ToArray());
                }
                finally
                {
                    Monitor.Exit(call);
                }
            }
            else
            {
                AddDeferredOperation(new Action<ICall, string>(TransferCall), call, number);
            }
        }

        /// <summary>
        /// Transfer call to active call
        /// </summary>
        /// <param name="call">Call to transfer</param>
        /// <param name="destCall">Destination call</param>
        public void TransferAttendedCall(ICall call, ICall destCall)
        {
            if (call == null)
            {
                return;
            }

            var currentCall = (Call)call;
            if (Monitor.TryEnter(call, OPERATION_WAIT_TIMEOUT))
            {
                try
                {
                    Logger.LogNotice($"Attendant transfering call {call.Number} on to {destCall.Number}.");

                    currentCall.LastUserAction = CallAction.Transfer;

                    _sip.SipekResources.CallManager.OnUserTransferAttendant(call.SessionId, destCall.SessionId);
                }
                finally
                {
                    Monitor.Exit(call);
                }
            }
            else
            {
                AddDeferredOperation(new Action<ICall, ICall>(TransferAttendedCall), call, destCall);
            }
        }

        /// <summary>
        /// Make conference between 2 calls
        /// </summary>
        /// <param name="call1">First call</param>
        /// <param name="call2">Second call</param>
        public void DoMakeConference(ICall call1, ICall call2)
        {
            if (call1 == null || call2 == null) return;

            if (Monitor.TryEnter(call1, OPERATION_WAIT_TIMEOUT) && Monitor.TryEnter(call2, OPERATION_WAIT_TIMEOUT))
            {
                try
                {
                    Logger.LogNotice($"Connecting media of {call1.Number} and {call2.Number}");

                    _sip.SipekResources.CallManager.OnUserConference(call1.SessionId, call2.SessionId);
                }
                finally
                {
                    Monitor.Exit(call1);
                    Monitor.Exit(call2);
                }

                // Raise event after lock releasing
                RaiseCallInfoChanged(call1 as Call);
                RaiseCallInfoChanged(call2 as Call);
            }
            else
                AddDeferredOperation(new Action<ICall, ICall>(DoMakeConference), call1, call2);
        }

        /// <summary>
        /// Make conference between all calls that in holding and active states
        /// </summary>
        public void DoMakeConference()
        {
            Logger.LogNotice(String.Format("Making conference."));

            lock (_calls)
            {
                if (_calls.Count(x => x.State == CallState.ACTIVE || x.State == CallState.HOLDING) < 2) return;

                _isConferenceActive = true;

                foreach (var call in _calls.Where(x => x.State == CallState.ACTIVE).ToArray())
                    call.IsInConference = true;

                var calls = _calls.Where(x => x.State == CallState.HOLDING).ToArray();

                if (calls.Length == 0) DoMakeConferenceInternal();
                else
                {
                    foreach (var call in calls)
                    {
                        call.IsInConference = true;

                        UnHoldCall(call);
                    }
                }
            }
        }

        private void DoMakeConferenceInternal()
        {
            Call[] calls;
            lock (_calls)
            {
                calls = _calls.Where(x => x.State == CallState.ACTIVE).ToArray();
            }

            // Interconnect all calls between themselves
            for (var i = 0; i < calls.Length; i++)
            {
                for (var j = i + 1; j < calls.Length; j++)
                {
                    AddDeferredOperation(new Action<ICall, ICall>(DoMakeConference), calls[i], calls[j]);
                }
            }
        }

        #endregion

        private void DoMakeCallAsync(CallWrapper call, string number, SipHeader[] headers)
        {
            try
            {
                int sessionId = _sip.SipekResources.CallManager.CreateSimpleOutboundCall(number, headers);

                if (sessionId < 0)
                    DropCall(call);
                else
                {
                    call.SetSession(sessionId);

                    RefreshCallState(call, sessionId);
                }
            }
            catch (Exception e)
            {
                Logger.LogWarn(e, "Can't acquire new session for call. Number: {0}. Call: {1}.", number, call);
            }
        }

        private void OnIncomingCallNotification(int sessionId, string number, string info, IEnumerable<KeyValuePair<string, string>> headers)
        {
            Logger.LogNotice($"Incoming call received. Number: {number}, SessionId: {sessionId}, Info: {info}.");

            // Incoming call raises after CallStateRefresh! So we have our call in collection
            Call call = FindCallBySessionId(sessionId);
            if (call == null) return;

            if (Monitor.TryEnter(call, OPERATION_WAIT_TIMEOUT))
            {
                Logger.LogNotice($"Local call found! Id: {call.Id}. Assigning values.");

                try
                {
                    if (call.Line < 0 && !AssignLineForCall(call))
                    {
                        DropCallInternal(call);

                        return;
                    }

                    call.Number = number;
                    call.Info = info;
                    if (call.Contact == null)
                        call.Contact = Core.ContactsManager.GetContactByPhoneNumber(number);

                    call.AddHeaders(headers);
                }
                finally { Monitor.Exit(call); }

                RaiseCallIncoming(call);
            }
            else
                AddDeferredOperation(new Action<int, string, string, IEnumerable<KeyValuePair<string, string>>>(OnIncomingCallNotification), sessionId, number, info, headers);
        }

        private void OnCallStateRefresh(int sessionId)
        {
            Logger.LogNotice($"Call state refresh for {sessionId}.");
            Call call = FindCallBySessionId(sessionId);

            if (call == null) // Call not found in our collection, but it may be exists! Let's add it
            {
                Logger.LogNotice($"Call not found for {sessionId}.");
                IStateMachine sipekCall = _sip.SipekResources.CallManager[sessionId];

                // Add call to own collection only if we in first incoming/outgoing states
                if (sipekCall != null &&
                    (sipekCall.StateId == EStateId.ALERTING || // Outgoing call
                     sipekCall.StateId == EStateId.INCOMING)) // Incoming call
                {
                    call = new Call(this, sessionId);

                    if (Monitor.TryEnter(_calls, OPERATION_WAIT_MAXTIMEOUT) && Monitor.TryEnter(call, OPERATION_WAIT_MAXTIMEOUT))
                    {
                        Logger.LogNotice($"Creating new call for {sessionId}. Id: {call.Id}");

                        try
                        {
                            if (call.Line < 0 && !AssignLineForCall(call))
                            {
                                DropCallInternal(call);

                                return;
                            }

                            RefreshCallState(call, sessionId);

                            _calls.Add(call);

                            // First state when outgoing call
                            if (call.State == CallState.ALERTING)
                                call.LastUserAction = CallAction.Make;

                            // Check presence code
                            if (_core.Sip.Account.PresenceStatus.Code == PresenceStatusCode.NotAvailable)
                            {
                                DropCallInternal(call);

                                return;
                            }
                        }
                        finally
                        {
                            Monitor.Exit(call);
                            Monitor.Exit(_calls);
                        }

                        RaiseCallStateChanged(call);
                    }
                }
            }
            else
            {
                if (Monitor.TryEnter(call, OPERATION_WAIT_MAXTIMEOUT))
                {
                    try { RefreshCallState(call, sessionId); }
                    finally { Monitor.Exit(call); }
                }
                else
                    return;

                // Call found - raising event about state changed
                RaiseCallStateChanged(call);

                // If it is new call.
                // Check if there are Conferences and if so then add it to conference automatically.
                if (IsConferenceActive && call.State == CallState.ACTIVE && !_calls.Any(x => x.State == CallState.HOLDING && x.IsInConference)) DoMakeConferenceInternal();
            }
        }

        void OnCallMediaStateChanged(int callId, ECallMediaState mediaState)
        {
            if (IsConferenceActive) AddDeferredOperation(new Action(DoMakeConferenceInternal));
        }

        private bool IsConferenceActive
        {
            get { return _isConferenceActive; }
        }

        private void RefreshCallState(Call call, int sessionId)
        {
            // Set call state. If call not found set state to null
            if (_sip.SipekResources.CallManager.CallList.ContainsKey(sessionId))
                call.State = (CallState)_sip.SipekResources.CallManager[sessionId].StateId;
            else
                call.State = CallState.NULL;
        }

        private void DurationTimerTick(object sender, EventArgs e)
        {
            RemoveNullCallsAndIncrementDuration();
        }

        private void InternalTimerTick(object sender, EventArgs e)
        {
            ProcessDeferredOperations();
        }

        private void RemoveNullCallsAndIncrementDuration()
        {
            IEnumerable<Call> calls;
            lock (_calls)
            {
                calls = _calls.ToArray();
            }

            foreach (var call in calls)
            {
                try
                {
                    if (call != null && call.State != CallState.NULL)
                    {
                        call.Duration += TimeSpan.FromSeconds(1);
                    }
                    else
                    {
                        RemoveCallInternal(call, CallRemoveReason.NULL);
                    }
                }
                catch (Exception ex)
                {
                    Logger.LogWarn(ex);
                }
            }
        }

        private Call FindCallBySessionId(int sessionId)
        {
            lock (_calls)
            {
                return _calls.FirstOrDefault(call => call.SessionId == sessionId);
            }
        }

        private CallRemoveReason GetCallRemoveReason(Call call)
        {
            if (!call.IsIncoming && call.LastState == CallState.NULL) return CallRemoveReason.InvalidData;
            if (!call.IsIncoming && call.LastUserAction == CallAction.Make && call.LastState == CallState.CONNECTING) return CallRemoveReason.InvalidData;
            if (call.LastUserAction == CallAction.Hangup) return CallRemoveReason.UserHangup;
            if (call.LastUserAction == CallAction.Transfer) return CallRemoveReason.Transfer;
            if (call.States.Any(x => x.State == CallState.ACTIVE) || call.States.Any(x => x.State == CallState.HOLDING)) return CallRemoveReason.RemoteHangup;
            if (!call.IsIncoming && call.States.Any(x => x.State == CallState.ALERTING)) return CallRemoveReason.Busy;
            return CallRemoveReason.NULL;
        }

        private void RemoveCallInternal(Call call, CallRemoveReason reason)
        {
            lock (_calls)
            {
                _isConferenceActive = _calls.Count(x => x.IsInConference) > 1;
                _calls.Remove(call);
            }

            call.IsDisposed = true;
            call.RaiseRemoved(reason);

            SafeRaiseEvent(OnCallRemoved, call, reason);
        }

        private bool AssignLineForCall(Call call)
        {
            lock (_lines)
            {
                int targetLine = -1;
                int maxAllowedCalls = LINES_MAXCOUNT;

                if (call.IsIncoming) maxAllowedCalls--;
                maxAllowedCalls -= _calls.Count(x => x != call);

                if (maxAllowedCalls <= 0) return false;

                for (int i = 0; i < LINES_MAXCOUNT; i++)
                {
                    if (_lines[i] == null)
                    {
                        targetLine = i;
                        break;
                    }

                    var line = _lines[i];
                    if (line.IsDisposed || line.State == CallState.NULL)
                    {
                        targetLine = i;
                        break;
                    }
                }

                if (targetLine >= 0 && targetLine < LINES_MAXCOUNT)
                {
                    _lines[targetLine] = call;

                    call.Line = targetLine;

                    return true;
                }
            }

            return false;
        }

        #region Events raising

        public void RaiseCallStateChanged(Call call)
        {
            if (call.State == CallState.NULL) // Call must be removed early
            {
                if (call.LastState == CallState.NULL) // It is not normal!!! May be raise removed event
                    RemoveCallInternal(call, CallRemoveReason.NULL);

                return;
            }

            if (call.State == CallState.RELEASED ||
                call.State == CallState.TERMINATED ||
                call.State == CallState.IDLE)
            {
                // Check it for raise Removed event only once
                if (call.LastState != CallState.RELEASED &&
                    call.LastState != CallState.TERMINATED &&
                    call.LastState != CallState.IDLE)

                    RemoveCallInternal(call, GetCallRemoveReason(call)); // Call removed in normal way
            }
            else
            {
                // Call state is changed
                call.RaiseStateChanged();

                SafeRaiseEvent(OnCallStateChanged, call);
            }
        }

        public void RaiseCallIncoming(Call call)
        {
            SafeRaiseEvent(OnIncomingCall, call);
        }

        public void RaiseCallInfoChanged(Call call)
        {
            call.RaiseInfoChanged();

            SafeRaiseEvent(OnCallInfoChanged, call);
        }

        public void RaiseCallDurationChanged(Call call)
        {
            call.RaiseDurationChanged();
        }

        private void SafeRaiseEvent(CallDelegate del, Call call)
        {
            if (del == null) return;

            var invocationList = del.GetInvocationList();

            foreach (var current in invocationList)
                ((CallDelegate)current).BeginInvoke(call, _raiseCallEventCallback, current);
        }

        private void SafeRaiseEvent(CallRemovedDelegate del, Call call, CallRemoveReason reason)
        {
            if (del == null) return;

            var invocationList = del.GetInvocationList();

            foreach (var current in invocationList)
                ((CallRemovedDelegate)current).BeginInvoke(call, reason, _raiseCallRemovedEventCallback, current);
        }

        private void RaiseCallEventCallback(IAsyncResult result)
        {
            try
            {
                ((CallDelegate)result.AsyncState).EndInvoke(result);
            }
            catch (Exception ex)
            {
                Logger.LogWarn(ex);
            }
        }

        private void RaiseCallRemovedEventCallback(IAsyncResult result)
        {
            try
            {
                ((CallRemovedDelegate)result.AsyncState).EndInvoke(result);
            }
            catch (Exception ex)
            {
                Logger.LogWarn(ex);
            }
        }

        #endregion

        #region Deferred operations

        private void AddDeferredOperation(Delegate del, params object[] p)
        {
            _defferedOperations.Enqueue(new CallOperation(del, p));
        }

        private void ProcessDeferredOperations()
        {
            CallOperation operation = null;
            while (_defferedOperations.TryDequeue(out operation))
            {
                var localOperation = operation;
                var del = new Action(localOperation.Execute);
                del.BeginInvoke(_deferredOperationCallback, del);
            }
        }

        private static void DeferredOperationCallback(IAsyncResult result)
        {
            try
            {
                var del = result.AsyncState as Action;
                if (del != null)
                    del.EndInvoke(result);
            }
            catch (Exception e)
            {
                Logger.LogWarn(e);
            }
        }

        private class CallOperation
        {
            Delegate OperationDelegate;
            readonly object[] Parameters;

            public CallOperation(Delegate operationDelegate, params object[] parameters)
            {
                OperationDelegate = operationDelegate;
                Parameters = parameters;
            }

            public void Execute()
            {
                OperationDelegate.DynamicInvoke(Parameters);
            }
        }

        #endregion

        #region IEnumerable<ICall>

        public IEnumerator<ICall> GetEnumerator()
        {
            IEnumerable<Call> calls;
            lock (_calls)
            {
                calls = _calls.ToArray();
            }

            return calls.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion
    }
}
