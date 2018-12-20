using ContactPoint.Common;
using ContactPoint.Common.PluginManager;

namespace ContactPoint.Plugins.CallTools.OneLine
{
    public class OneLineService
    {
        private readonly IPluginManager _pluginManager;

        public bool IsOneLine { get; set; } = false;
        public bool IsStarted { get; private set; } = false;
        
        public OneLineService(IPlugin plugin)
        {
            _pluginManager = plugin.PluginManager;
        }

        public void Start()
        {
            if (IsStarted) { return; }
            _pluginManager.Core.CallManager.OnCallStateChanged += CallManager_OnCallStateChanged;

            this.IsStarted = true;
        }

        public void Stop()
        {
            if (!IsStarted) { return; }
            _pluginManager.Core.CallManager.OnCallStateChanged -= CallManager_OnCallStateChanged;

            this.IsStarted = false;
        }

        void CallManager_OnCallStateChanged(ICall call)
        {
            if (!IsOneLine)
            {
                return;
            }

            if (call == null || call.IsDisposed)
            {
                return;
            }

            if (call.IsIncoming && _pluginManager.Core.CallManager.Count > 1)
            {
                call.Drop();
            }
        }
    }
}
