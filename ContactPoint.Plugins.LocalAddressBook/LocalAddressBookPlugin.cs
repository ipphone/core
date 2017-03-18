using ContactPoint.Common.PluginManager;
using ContactPoint.Core.PluginManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ContactPoint.Plugins.LocalAddressBook
{
    [Plugin("{D878BAF7-4336-4487-A5A4-F3A2ED300B7A}", "Local address book")]
    public class LocalAddressBookPlugin : Plugin
    {
        private bool _isStarted = false;
        private readonly LocalAddressBook _addressBook;

        public override bool IsStarted
        {
            get { return _isStarted; }
        }

        public override IEnumerable<IPluginUIElement> UIElements
        {
            get { return null; }
        }

        public override Common.Contacts.IAddressBook AddressBook
        {
            get { return _addressBook; }
        }

        public LocalAddressBookPlugin(IPluginManager pluginManager)
            : base(pluginManager)
        {
            _addressBook = new LocalAddressBook(this);
        }

        public override void Start()
        {
            _isStarted = true;

            RaiseStartedEvent();
        }

        public override void Stop()
        {
            _isStarted = false;

            RaiseStoppedEvent("Normal stop");
        }
    }
}
