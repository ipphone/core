using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using ContactPoint.Common.PluginManager;
using ContactPoint.Core.PluginManager;
using ContactPoint.Plugins.ContactsUi.Forms;

namespace ContactPoint.Plugins.ContactsUi
{
    [Plugin("{9B75A7BA-5B95-494A-833A-2E258675A108}", "AddressBook UI", HaveSettingsForm = false)]
    public class ContactsUiService : Plugin
    {
        private readonly IPluginUIElement[] _uiElements;

        public ContactsUiService(IPluginManager pluginManager)
            : base(pluginManager)
        {
            _uiElements = new IPluginUIElement[] {new ContactsCommand(this)};
        }

        public override IEnumerable<IPluginUIElement> UIElements
        {
            get { return _uiElements; }
        }

        public override void Start()
        {
        }

        public override void Stop()
        {
        }

        public override bool IsStarted
        {
            get { return true; }
        }
    }

    internal class ContactsCommand : PluginUIElement
    {
        private readonly Guid _actionCode = new Guid("{F024D728-831F-4F9B-BD13-E156B3CF484A}");
        private static ContactsListForm _formInstance = null;

        public ContactsCommand(IPlugin plugin) : base(plugin)
        { }

        public override Bitmap Image
        {
            get { return Properties.Resources.contacts1; }
        }

        public override string Text
        {
            get { return "Contacts"; }
        }

        public override IEnumerable<IPluginUIElement> Childrens
        {
            get { return null; }
        }

        public override Guid ActionCode
        {
            get { return _actionCode; }
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

        protected override void InternalExecute(object sender)
        {
            lock (this)
            {
                if (_formInstance == null)
                {
                    _formInstance = new ContactsListForm(Plugin.PluginManager.Core.ContactsManager);
                    _formInstance.Closing += FormInstanceClosing;
                }
            }

            _formInstance.Show();
        }

        void FormInstanceClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            lock (this)
            {
                _formInstance = null;
            }
        }
    }
}
