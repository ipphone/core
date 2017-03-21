using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ContactPoint.Common.Contacts;
using ContactPoint.Common.PluginManager;
using ContactPoint.Core.PluginManager;

namespace ContactPoint.Plugins.GoogleContacts
{
    [Plugin("{DFF2F859-4064-44E7-A165-33163C51B7D9}", "Google contacts", HaveSettingsForm = true)]
    public class GoogleContactsPlugin : Plugin
    {
        private bool _isStarted = false;
        private readonly GoogleAddressBook _googleAddressBook;

        public override IEnumerable<IPluginUIElement> UIElements
        {
            get { return null; }
        }

        public override Common.Contacts.IAddressBook AddressBook
        {
            get { return _googleAddressBook; }
        }

        public GoogleContactsPlugin(IPluginManager pluginManager)
            : base(pluginManager)
        {
            _googleAddressBook = new GoogleAddressBook(this);
        }

        public override void Start()
        {
            _isStarted = true;
            RaiseStartedEvent();
        }

        public override void Stop()
        {
            _isStarted = false;
            RaiseStoppedEvent(String.Empty);
        }

        public override bool IsStarted
        {
            get { return _isStarted; }
        }
    }
}
