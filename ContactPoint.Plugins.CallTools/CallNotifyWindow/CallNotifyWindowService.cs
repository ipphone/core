using System;
using ContactPoint.BaseDesign;
using ContactPoint.Common;
using ContactPoint.Common.PluginManager;

namespace ContactPoint.Plugins.CallTools.CallNotifyWindow {
    internal class CallNotifyWindowService : IService {
        private readonly IPlugin _plugin;
        private readonly CallDelegate _callStatChanged;
        private bool _isStarted = false;
        private int _showingWindowTimeout = 0;

        public event ServiceStartedDelegate Started;
        public event ServiceStoppedDelegate Stopped;

        public bool IsStarted {
            get { return _isStarted; }
        }

        public CallNotifyWindowService(IPlugin plugin) {
            _plugin = plugin;

            _callStatChanged = CallManager_OnCallStateChanged;
        }

        public void Start() {
            if (IsStarted) { return; }
            _plugin.PluginManager.Core.CallManager.OnCallStateChanged += _callStatChanged;
            _plugin.PluginManager.Core.CallManager.OnIncomingCall += CallManager_OnIncomingCall;

            _isStarted = true;
        }

        public void Stop() {
            if (!IsStarted) { return; }
            _plugin.PluginManager.Core.CallManager.OnCallStateChanged -= _callStatChanged;
            _plugin.PluginManager.Core.CallManager.OnIncomingCall -= CallManager_OnIncomingCall;

            _isStarted = false;
        }

        void CallManager_OnIncomingCall(ICall call) {
            if (IsStarted && call.Headers.Contains("x-color") && !call.Tags.ContainsKey("color"))
                call.Tags.Add("color", call.Headers["x-color"].Value);
        }

        void CallManager_OnCallStateChanged(ICall call) {
            bool notifyUser = false;

            if (call == null)
                return;

            lock (call) {
                if (call.IsDisposed)
                    return;

                if (call.State == CallState.INCOMING && call.LastState == CallState.NULL && call.IsIncoming)
                    notifyUser = true;
            }

            if (notifyUser && NeedShowingNotification()) {
                bool isWindowPersistent = IsNotificationWindowPersistent();
                int closeWindowTimeout = isWindowPersistent ? int.MaxValue : 0;
                if (SyncUi.InvokeRequired) {
                    SyncUi.Invoke(new Action(() => NotifyManager.NotifyUser(new CallNotifyControl(call, isWindowPersistent), closeWindowTimeout)));
                } else {
                    NotifyManager.NotifyUser(new CallNotifyControl(call, isWindowPersistent), closeWindowTimeout);
                }
            }
        }

        private bool NeedShowingNotification() {
            return _plugin.PluginManager.Core.SettingsManager.Get<bool>(CallToolsOptions.ShowIncomingCallWindowName);
        }

        private bool IsNotificationWindowPersistent() {
            return _plugin.PluginManager.Core.SettingsManager.Get<bool>(CallToolsOptions.NotHideCallWindowName);
        }
    }
}
