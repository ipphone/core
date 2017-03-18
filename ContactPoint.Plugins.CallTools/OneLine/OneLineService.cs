using ContactPoint.Common;
using ContactPoint.Common.PluginManager;

namespace ContactPoint.Plugins.CallTools.OneLine {
    public class OneLineService {
        private bool _isOneLine = false;
        private bool _isStarted = false;
        private readonly IPluginManager _pluginManager;

        public bool IsOneLine {
            get { return this._isOneLine; }
            set { this._isOneLine = value; }
        }

        public void Start() {
            if (IsStarted) { return; }
            _pluginManager.Core.CallManager.OnCallStateChanged += CallManager_OnCallStateChanged;

            this._isStarted = true;
        }

        public void Stop() {
            if (!IsStarted) { return; }
            _pluginManager.Core.CallManager.OnCallStateChanged -= CallManager_OnCallStateChanged;

            this._isStarted = false;
        }

        public bool IsStarted {
            get { return this._isStarted; }
        }

        public OneLineService(IPlugin plugin) {
            _pluginManager = plugin.PluginManager;
        }

        void CallManager_OnCallStateChanged(ICall call) {
            lock (call) {
                if (call != null &&
                    !call.IsDisposed &&
                    this.IsOneLine &&
                    call.IsIncoming &&
                    _pluginManager.Core.CallManager.Count > 1)
                    call.Drop();
            }
        }
    }
}
