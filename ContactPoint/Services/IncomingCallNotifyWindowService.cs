using System;
using ContactPoint.BaseDesign;
using ContactPoint.BaseDesign.BaseNotifyControls;
using ContactPoint.Common;

namespace ContactPoint.Services
{
    public class IncomingCallNotifyWindowService
    {
        public IncomingCallNotifyWindowService(ICallManager callManager)
        {
            callManager.OnCallStateChanged += OnCallStateChanged;
            callManager.OnIncomingCall += OnIncomingCall;
        }

        private void OnIncomingCall(ICall call)
        {
            if (call.Headers.Contains("x-color") && !call.Tags.ContainsKey("color"))
            {
                call.Tags.Add("color", call.Headers["x-color"].Value);
            }
        }

        private void OnCallStateChanged(ICall call)
        {
            if (call == null || call.IsDisposed)
            {
                return;
            }

            if (call.State == CallState.INCOMING && call.LastState == CallState.NULL && call.IsIncoming)
            {
                if (SyncUi.InvokeRequired)
                {
                    SyncUi.Invoke(new Action<ICall>(NotifyUser), call);
                }
                else
                {
                    NotifyUser(call);
                }
            }
        }

        private void NotifyUser(ICall call)
        {
            var notifyControl = NotifyControlFactory.CreateNotifyControl(Notifications.IncomingCall, call);
            if (notifyControl != null)
            {
                NotifyManager.NotifyUser(notifyControl);
            }
        }
    }
}
