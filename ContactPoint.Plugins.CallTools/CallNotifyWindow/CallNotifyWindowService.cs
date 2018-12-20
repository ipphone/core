using System;
using ContactPoint.BaseDesign;
using ContactPoint.Common;
using ContactPoint.Common.PluginManager;

namespace ContactPoint.Plugins.CallTools.CallNotifyWindow
{
    internal class CallNotifyWindowService : IService
    {
        private readonly IPlugin _plugin;
        private readonly CallDelegate _callStatChanged;

        public event ServiceStartedDelegate Started;
        public event ServiceStoppedDelegate Stopped;
        
        public bool NeedShowingNotification => _plugin.PluginManager.Core.SettingsManager.Get<bool>(CallToolsOptions.ShowIncomingCallWindowName);
        public bool IsNotificationWindowPersistent => _plugin.PluginManager.Core.SettingsManager.Get<bool>(CallToolsOptions.NotHideCallWindowName);

        public bool IsStarted { get; private set; } = false;

        public CallNotifyWindowService(IPlugin plugin)
        {
            _plugin = plugin;

            _callStatChanged = CallManager_OnCallStateChanged;
        }

        public void Start()
        {
            if (IsStarted) { return; }

            _plugin.PluginManager.Core.CallManager.OnCallStateChanged += _callStatChanged;
            _plugin.PluginManager.Core.CallManager.OnIncomingCall += CallManager_OnIncomingCall;

            IsStarted = true;
            Started?.Invoke(this);
        }

        public void Stop()
        {
            if (!IsStarted) { return; }

            _plugin.PluginManager.Core.CallManager.OnCallStateChanged -= _callStatChanged;
            _plugin.PluginManager.Core.CallManager.OnIncomingCall -= CallManager_OnIncomingCall;

            IsStarted = false;
            Stopped?.Invoke(this, string.Empty);
        }

        void CallManager_OnIncomingCall(ICall call)
        {
            if (IsStarted && call.Headers.Contains("x-color") && !call.Tags.ContainsKey("color"))
                call.Tags.Add("color", call.Headers["x-color"].Value);
        }

        void CallManager_OnCallStateChanged(ICall call)
        {
            if (call == null || call.IsDisposed)
            {
                return;
            }

            if (NeedShowingNotification&& call.State == CallState.INCOMING && call.LastState == CallState.NULL && call.IsIncoming)
            {
                bool isWindowPersistent = IsNotificationWindowPersistent;
                int closeWindowTimeout = isWindowPersistent ? int.MaxValue : 0;

                if (SyncUi.InvokeRequired)
                {
                    SyncUi.Invoke(new Action(() => NotifyManager.NotifyUser(new CallNotifyControl(call, isWindowPersistent), closeWindowTimeout)));
                }
                else
                {
                    NotifyManager.NotifyUser(new CallNotifyControl(call, isWindowPersistent), closeWindowTimeout);
                }
            }
        }
    }
}
