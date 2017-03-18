using ContactPoint.Common;
using ContactPoint.Common.PluginManager;

namespace ContactPoint.Plugins.CallTools.AutoAnswer {
    class AutoAnswerService {
        private readonly IPluginManager _pluginManager;

        internal bool IsStarted { get; private set; }

        public void Start() {
            if (IsStarted) { return; }
            _pluginManager.Core.CallManager.OnCallStateChanged += CallManager_OnCallStateChanged;
            IsStarted = true;
        }

        public void Stop() {
            if (!IsStarted) { return; }
            _pluginManager.Core.CallManager.OnCallStateChanged -= CallManager_OnCallStateChanged;
            IsStarted = false;
        }

        public AutoAnswerService(IPlugin plugin) {
            _pluginManager = plugin.PluginManager;
        }

        void CallManager_OnCallStateChanged(ICall call) {
            if (call != null && !call.IsDisposed && call.IsIncoming && (call.State == CallState.ALERTING || call.State == CallState.INCOMING) && call.Headers.Contains("x-auto-answer"))
                call.Answer();
        }
    }
}
