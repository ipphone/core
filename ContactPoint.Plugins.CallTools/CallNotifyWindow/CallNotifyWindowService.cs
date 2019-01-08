using System;
using ContactPoint.BaseDesign;
using ContactPoint.Common;
using ContactPoint.Common.PluginManager;

namespace ContactPoint.Plugins.CallTools.CallNotifyWindow
{
    internal class CallNotifyWindowService : IService
    {
        private readonly IPlugin _plugin;

        public event ServiceStartedDelegate Started;
        public event ServiceStoppedDelegate Stopped;
        
        public bool NeedShowingNotification { get; private set; }
        public bool IsWindowPersistent { get; private set; }
        public int CloseWindowTimeout { get; private set; }

        public bool IsStarted { get; private set; } = false;

        public CallNotifyWindowService(IPlugin plugin)
        {
            _plugin = plugin;
        }

        public void Start()
        {
            if (IsStarted) { return; }

            NeedShowingNotification = _plugin.PluginManager.Core.SettingsManager.Get<bool>(CallToolsOptions.ShowIncomingCallWindowName);
            IsWindowPersistent = _plugin.PluginManager.Core.SettingsManager.Get<bool>(CallToolsOptions.NotHideCallWindowName);
            CloseWindowTimeout = IsWindowPersistent ? int.MaxValue : 0;

            _plugin.PluginManager.Core.CallManager.OnCallStateChanged += OnCallStateChanged;
            _plugin.PluginManager.Core.CallManager.OnIncomingCall += OnIncomingCall;

            IsStarted = true;
            Started?.Invoke(this);
        }

        public void Stop()
        {
            if (!IsStarted) { return; }

            _plugin.PluginManager.Core.CallManager.OnCallStateChanged -= OnCallStateChanged;
            _plugin.PluginManager.Core.CallManager.OnIncomingCall -= OnIncomingCall;

            IsStarted = false;
            Stopped?.Invoke(this, string.Empty);
        }

        private void OnIncomingCall(ICall call)
        {
            if (IsStarted && call.Headers.Contains("x-color") && !call.Tags.ContainsKey("color"))
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

            if (NeedShowingNotification && call.State == CallState.INCOMING && call.LastState == CallState.NULL && call.IsIncoming)
            {
                if (SyncUi.InvokeRequired)
                {
                    SyncUi.Invoke(new Action(() => NotifyManager.NotifyUser(new CallNotifyControl(call, IsWindowPersistent), CloseWindowTimeout)));
                }
                else
                {
                    NotifyManager.NotifyUser(new CallNotifyControl(call, IsWindowPersistent), CloseWindowTimeout);
                }
            }
        }
    }
}
