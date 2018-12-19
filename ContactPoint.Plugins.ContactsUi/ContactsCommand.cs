using System;
using System.Collections.Generic;
using System.Drawing;
using ContactPoint.Common.PluginManager;
using ContactPoint.Core.PluginManager;
using ContactPoint.Plugins.ContactsUi.Forms;

namespace ContactPoint.Plugins.ContactsUi
{
    internal class ContactsCommand : PluginUIElementBase
    {
        private static ContactsListForm _formInstance;

        private readonly object _lockObj = new object();

        public ContactsCommand(IPlugin plugin) : base(plugin)
        { }

        public override Guid ActionCode { get; } = new Guid("{F024D728-831F-4F9B-BD13-E156B3CF484A}");

        public override Bitmap Image => Properties.Resources.contacts1;
        public override string Text => "Contacts";
        public override IEnumerable<IPluginUIElement> Childrens => null;
        public override bool ShowInMenu => true;
        public override bool ShowInToolBar => true;
        public override bool ShowInNotifyMenu => true;

        protected override void ExecuteCommand(object sender, object data)
        {
            lock (_lockObj)
            {
                if (_formInstance == null)
                {
                    _formInstance = new ContactsListForm(Plugin.PluginManager.Core.ContactsManager);
                    _formInstance.Closing += FormInstanceClosing;
                }

                _formInstance.Show();
            }
        }

        private void FormInstanceClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            lock (_lockObj)
            {
                _formInstance.Closing -= FormInstanceClosing;
                _formInstance = null;
            }
        }
    }
}
