using System;
using ContactPoint.Common.PluginManager;
using ContactPoint.Common.SIP.Account;

namespace ContactPoint.Plugins.CallTools.Pause
{
    public class AccountStateUiElement : PauseUiElement
    {
        public SipAccountState TargetRegisterState { get; } = SipAccountState.Offline;

        public AccountStateUiElement(IPlugin plugin) : base(plugin)
        {
            var settings = plugin.PluginManager.Core.SettingsManager;
            if (settings.Get<string>("PauseButton.TargetRegisterState") != null) TargetRegisterState = (SipAccountState)Enum.Parse(typeof(SipAccountState), settings.Get<string>("PauseButton.TargetRegisterState"));
        }

        protected override void ExecuteCheckedCommand(object sender, bool checkedValue, object data)
        {
            if (!Enabled)
            {
                return;
            }

            if (Plugin.PluginManager.Core.Sip.Account.RegisterState != TargetRegisterState)
            {
                if (TargetRegisterState == SipAccountState.Online)
                {
                    Plugin.PluginManager.Core.Sip.Account.Register();
                }
                else if (TargetRegisterState == SipAccountState.Offline)
                {
                    Plugin.PluginManager.Core.Sip.Account.UnRegister();
                }
                else
                {
                    if (Plugin.PluginManager.Core.Sip.Account.RegisterState == SipAccountState.Online)
                    {
                        Plugin.PluginManager.Core.Sip.Account.UnRegister();
                    }

                    if (Plugin.PluginManager.Core.Sip.Account.RegisterState == SipAccountState.Offline)
                    {
                        Plugin.PluginManager.Core.Sip.Account.Register();
                    }
                }
            }
        }

        protected override void AccountOnRegisterStateChanged(ISipAccount account)
        {
            Checked = account?.PresenceStatus?.Code == TargetState;

            base.AccountOnRegisterStateChanged(account);
        }
    }
}
