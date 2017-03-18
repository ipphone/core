using System;
using System.Collections.Generic;
using ContactPoint.Common;

namespace ContactPoint.Core.CallManager
{
    internal class CallWrapper : Call
    {
        private readonly List<Action> _actions = new List<Action>();

        public CallWrapper(CallManager callManager, string number)
            : base(callManager, -1)
        {
            Number = number;
        }

        public void SetSession(int sessionId)
        {
            lock (_actions)
            {
                try
                {
                    Number = ((SIP.SIP)CallManager.Core.Sip).SipekResources.CallManager[sessionId] != null ? ((SIP.SIP)CallManager.Core.Sip).SipekResources.CallManager[sessionId].CallingNumber : "";
                    SessionId = sessionId;
                }
                catch (Exception e)
                {
                    Logger.LogWarn(e, "Invalid flow of call. SessionId is invalid");

                    return;
                }

                foreach (var action in _actions)
                    try
                    {
                        action.Invoke();
                    }
                    catch (Exception e)
                    {
                        Logger.LogWarn(e);
                    }

                _actions.Clear();
            }
        }

        public override void Answer()
        {
            lock (_actions)
                if (SessionId < 0)
                {
                    _actions.Add(() => base.Answer());

                    return;
                }

            base.Answer();
        }

        public override void Drop()
        {
            lock (_actions)
                if (SessionId < 0 && State != CallState.CONNECTING && State != CallState.NULL)
                {
                    _actions.Add(() => base.Drop());

                    return;
                }

            base.Drop();
        }

        public override void Hold()
        {
            lock (_actions)
                if (SessionId < 0)
                {
                    _actions.Add(() => base.Hold());

                    return;
                }

            base.Hold();
        }

        public override void SendDTMF(string digits)
        {
            lock (_actions)
                if (SessionId < 0)
                {
                    _actions.Add(() => base.SendDTMF(digits));

                    return;
                }

            base.SendDTMF(digits);
        }

        public override void ToggleHold()
        {
            lock (_actions)
                if (SessionId < 0)
                {
                    _actions.Add(() => base.ToggleHold());

                    return;
                }

            base.ToggleHold();
        }

        public override void Transfer(string number)
        {
            lock (_actions)
                if (SessionId < 0)
                {
                    _actions.Add(() => base.Transfer(number));

                    return;
                }
            base.Transfer(number);
        }

        public override void TransferAttended(ICall destCall)
        {
            lock (_actions)
                if (SessionId < 0)
                {
                    _actions.Add(() => base.TransferAttended(destCall));

                    return;
                }

            base.TransferAttended(destCall);
        }

        public override void UnHold()
        {
            lock (_actions)
                if (SessionId < 0)
                {
                    _actions.Add(() => base.UnHold());

                    return;
                }

            base.UnHold();
        }
    }
}
