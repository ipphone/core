using System;
using System.Collections.Generic;
using System.Text;
using ContactPoint.Common.PluginManager;
using ContactPoint.Common.SIP.Account;
using ContactPoint.Core.PluginManager;
using ContactPoint.Plugins.Pause.Properties;

namespace ContactPoint.Plugins.Pause
{
    internal class PauseUiElement : PluginCheckedUIElement
    {
        public override System.Drawing.Bitmap Image
        {
            get { return Resources.pause; }
        }

        public override string Text
        {
            get { return Resources.PauseText; }
        }

        public override List<IPluginUIElement> Childrens
        {
            get { return null; }
        }

        public override Guid ActionCode
        {
            get { return new Guid("{475B69A4-00DD-4046-AC15-B752BC9D296B}"); }
        }

        public override bool ShowInMenu
        {
            get { return true; }
        }

        public override bool ShowInToolBar
        {
            get { return true; }
        }

        public override bool ShowInNotifyMenu
        {
            get { return true; }
        }

        public PauseUiElement(IPlugin plugin)
            : base(plugin)
        {
            Plugin.PluginManager.Core.Sip.Account.PresenceStatusChanged += Account_PresenceStatusChanged;
        }

        ~PauseUiElement()
        {
            Plugin.PluginManager.Core.Sip.Account.PresenceStatusChanged -= Account_PresenceStatusChanged;
        }

        void Account_PresenceStatusChanged(ISipAccount obj)
        {
            if (obj.PresenceStatus.Code == PresenceStatusCode.NotAvailable) Checked = true;
            else Checked = false;
        }

        protected override void InternalExecute(object sender)
        {
            if (Checked) Plugin.PluginManager.Core.Sip.Account.PresenceStatus = new PresenceStatus(PresenceStatusCode.NotAvailable);
            else Plugin.PluginManager.Core.Sip.Account.PresenceStatus = new PresenceStatus(PresenceStatusCode.Available);
        }
    }
}
