using ContactPoint.BaseDesign;
using ContactPoint.BaseDesign.BaseNotifyControls;
using ContactPoint.Common;
using ContactPoint.Common.PluginManager;

namespace ContactPoint.Plugins.CallTools.CallNotifyWindow
{
    internal class CallNotifyWindowService
    {
        private readonly IPlugin _plugin;

        public bool IsWindowPersistent => _plugin.PluginManager.Core.SettingsManager.Get<bool>(CallToolsOptions.NotHideCallWindowName);

        public CallNotifyWindowService(IPlugin plugin)
        {
            _plugin = plugin;
        }

        public void Start()
        {
            NotifyControlFactory.RegisterNotifyControlFactory(Notifications.IncomingCall, CreateCallNotifyControl);
        }

        public void Stop()
        {
            NotifyControlFactory.UnregisterNotifyControlFactory(Notifications.IncomingCall, CreateCallNotifyControl);
        }

        private NotifyControl CreateCallNotifyControl(object param)
        {
            return param is ICall call ? new CallNotifyControl(call, IsWindowPersistent) : null;
        }
    }
}
