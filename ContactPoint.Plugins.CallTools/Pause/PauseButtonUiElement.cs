using ContactPoint.Common.PluginManager;
using ContactPoint.Common.SIP.Account;

namespace ContactPoint.Plugins.CallTools.Pause
{
    public class PauseButtonUiElement : AccountStateUiElement
    {
        public PauseButtonUiElement(IPlugin plugin) : base(plugin)
        { }

        protected override void ExecuteCheckedCommand(object sender, bool checkedValue, object data)
        {
            base.ExecuteCheckedCommand(sender, checkedValue, data);

            Checked = false;
            Plugin.PluginManager.Core.Sip.Account.PresenceStatus = new PresenceStatus(TargetState, data as string ?? Text, null);
        }

        protected override void AccountOnPresenceStatusChanged(ISipAccount account)
        { }

        protected override void AccountOnRegisterStateChanged(ISipAccount account)
        {
            if (account == null)
            {
                return;
            }

            SetEnabled(true);
            RaiseUIChangedEvent();
        }
    }
}
