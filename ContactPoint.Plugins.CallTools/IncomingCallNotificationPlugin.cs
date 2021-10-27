using ContactPoint.Common.PluginManager;
using ContactPoint.Core.PluginManager;
using ContactPoint.Plugins.CallTools.CallNotifyWindow;

namespace ContactPoint.Plugins.CallTools
{
    [Plugin("{718a93ed-2355-436c-979d-e2ca8b40f977}", "Incoming call notifications plugin", HaveSettingsForm = false)]
    public class IncomingCallNotificationPlugin : Plugin
    {
        private readonly CallNotifyWindowService _callNotifyWindowService;
        private bool _isStarted;

        public override bool IsStarted => _isStarted;

        public IncomingCallNotificationPlugin(IPluginManager pluginManager)
            : base(pluginManager)
        { 
            _callNotifyWindowService = new CallNotifyWindowService(this);
        }

        public override void Start()
        {
            _isStarted = true;
            _callNotifyWindowService.Start();
        }

        public override void Stop()
        {
            _isStarted = false;
            _callNotifyWindowService.Stop();
        }
    }
}
