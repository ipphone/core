using System;
using System.Collections.Generic;
using System.Drawing;
using ContactPoint.Common.PluginManager;
using ContactPoint.Common.SIP.Account;
using ContactPoint.Core.PluginManager;
using ContactPoint.Plugins.CallTools.Properties;

namespace ContactPoint.Plugins.CallTools.Pause
{
    public class PauseUiElement : PluginCheckedUIElementBase
    {
        public PresenceStatusCode TargetState { get; } = PresenceStatusCode.DND;
        public PresenceStatusCode DefaultState { get; } = PresenceStatusCode.Available;

        private bool _enabled = true;

        public SipAccountState? ValidRegisterState { get; }

        public override Bitmap Image { get; } = Resources.pause;
        public override string Text { get; } = Resources.PauseText;
        public override Guid ActionCode { get; } = Guid.Parse("{475B69A4-00DD-4046-AC15-B752BC9D2970}");
        public override bool ShowInMenu { get; } = true;
        public override bool ShowInToolBar { get; } = true;
        public override bool ShowInNotifyMenu { get; } = true;
        public override IEnumerable<IPluginUIElement> Childrens { get; } = null;
        public override bool Enabled => _enabled;

        public PauseUiElement(IPlugin plugin) : base(plugin)
        {
            var settings = plugin.PluginManager.Core.SettingsManager;

            if (settings.Get<string>("PauseButton.Image") != null) Image = (Bitmap)Resources.ResourceManager.GetObject(settings.Get<string>("PauseButton.Image"));
            if (settings.Get<string>("PauseButton.Text") != null) Text = settings.Get<string>("PauseButton.Text");
            if (settings.Get<string>("PauseButton.ActionCode") != null) ActionCode = Guid.Parse(settings.Get<string>("PauseButton.ActionCode"));
            if (settings.Get<string>("PauseButton.ShowInMenu") != null) ShowInMenu = bool.Parse(settings.Get<string>("PauseButton.ShowInMenu"));
            if (settings.Get<string>("PauseButton.ShowInToolBar") != null) ShowInToolBar = bool.Parse(settings.Get<string>("PauseButton.ShowInToolBar"));
            if (settings.Get<string>("PauseButton.ShowInNotifyMenu") != null) ShowInNotifyMenu = bool.Parse(settings.Get<string>("PauseButton.ShowInNotifyMenu"));
            if (settings.Get<string>("PauseButton.DefaultState") != null) DefaultState = (PresenceStatusCode) Enum.Parse(typeof(PresenceStatusCode), settings.Get<string>("PauseButton.DefaultState"));
            if (settings.Get<string>("PauseButton.TargetState") != null) TargetState = (PresenceStatusCode) Enum.Parse(typeof(PresenceStatusCode), settings.Get<string>("PauseButton.TargetState"));
            if (settings.Get<string>("PauseButton.ValidRegisterState") != null) ValidRegisterState = (SipAccountState)Enum.Parse(typeof(SipAccountState), settings.Get<string>("PauseButton.ValidRegisterState"));

            Plugin.PluginManager.Core.Sip.Account.PresenceStatusChanged += AccountOnPresenceStatusChanged;
            Plugin.PluginManager.Core.Sip.Account.RegisterStateChanged += AccountOnRegisterStateChanged;
        }

        protected override void ExecuteCheckedCommand(object sender, bool checkedValue, object data)
        {
            if (Enabled)
            {
                Plugin.PluginManager.Core.Sip.Account.PresenceStatus = new PresenceStatus(
                    checkedValue ? TargetState : DefaultState, 
                    data as string ?? Text, 
                    checkedValue ? ImageChecked : null);
            }
        }

        protected virtual void AccountOnPresenceStatusChanged(ISipAccount account)
        {
            Checked = account?.PresenceStatus?.Code == TargetState;
        }

        protected virtual void AccountOnRegisterStateChanged(ISipAccount account)
        {
            if (account == null)
            {
                return;
            }

            var enabled = _enabled;
            _enabled = ValidRegisterState?.Equals(account.RegisterState) ?? true;

            if (enabled == _enabled)
            {
                return;
            }

            RaiseUIChangedEvent();

            if (!_enabled)
            {
                Checked = false;
            }
            else
            {
                var target = account.PresenceStatus?.Code == TargetState;
                if (Checked != target)
                {
                    Checked = target;
                }
            }
        }

        protected override void Dispose(bool disposing)
        {
            Plugin.PluginManager.Core.Sip.Account.PresenceStatusChanged -= AccountOnPresenceStatusChanged;
            Plugin.PluginManager.Core.Sip.Account.RegisterStateChanged -= AccountOnRegisterStateChanged;

            base.Dispose(disposing);
        }

        protected void SetEnabled(bool value)
        {
            _enabled = value;
        }
    }
}
