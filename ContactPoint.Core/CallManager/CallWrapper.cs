using System;
using System.Collections.Concurrent;
using ContactPoint.Common;

namespace ContactPoint.Core.CallManager
{
    internal class CallWrapper : Call
    {
        private readonly ConcurrentQueue<Action> _actions = new ConcurrentQueue<Action>();

        public CallWrapper(CallManager callManager, string number)
            : base(callManager, -1)
        {
            Number = number;
        }

        public void SetSession(int sessionId)
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

            while (_actions.TryDequeue(out Action action))
            {
                try
                {
                    action.Invoke();
                }
                catch (Exception e)
                {
                    Logger.LogWarn(e);
                }
            }
        }

        public override void Answer()
        {
            if (SessionId < 0)
            {
                _actions.Enqueue(() => base.Answer());
                return;
            }

            base.Answer();
        }

        public override void Drop()
        {
            if (SessionId < 0 && State != CallState.CONNECTING && State != CallState.NULL)
            {
                _actions.Enqueue(() => base.Drop());
                return;
            }

            base.Drop();
        }

        public override void Hold()
        {
            if (SessionId < 0)
            {
                _actions.Enqueue(() => base.Hold());
                return;
            }

            base.Hold();
        }

        public override void SendDTMF(string digits)
        {
            if (SessionId < 0)
            {
                _actions.Enqueue(() => base.SendDTMF(digits));
                return;
            }

            base.SendDTMF(digits);
        }

        public override void ToggleHold()
        {
            if (SessionId < 0)
            {
                _actions.Enqueue(() => base.ToggleHold());
                return;
            }

            base.ToggleHold();
        }

        public override void Transfer(string number)
        {
            if (SessionId < 0)
            {
                _actions.Enqueue(() => base.Transfer(number));
                return;
            }

            base.Transfer(number);
        }

        public override void TransferAttended(ICall destCall)
        {
            if (SessionId < 0)
            {
                _actions.Enqueue(() => base.TransferAttended(destCall));
                return;
            }

            base.TransferAttended(destCall);
        }

        public override void UnHold()
        {
            if (SessionId < 0)
            {
                _actions.Enqueue(() => base.UnHold());
                return;
            }

            base.UnHold();
        }
    }
}
