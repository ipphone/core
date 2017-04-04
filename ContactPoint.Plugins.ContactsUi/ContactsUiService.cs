using System.Collections.Generic;
using ContactPoint.Common.PluginManager;
using ContactPoint.Core.PluginManager;

namespace ContactPoint.Plugins.ContactsUi
{
    [Plugin("{9B75A7BA-5B95-494A-833A-2E258675A108}", "AddressBook UI", HaveSettingsForm = false)]
    public class ContactsUiService : Plugin
    {
        private readonly IPluginUIElement[] _uiElements;

        public ContactsUiService(IPluginManager pluginManager)
            : base(pluginManager)
        {
            _uiElements = new IPluginUIElement[] { new ContactsCommand(this) };
        }

        public override IEnumerable<IPluginUIElement> UIElements => _uiElements;
        public override bool IsStarted => true;

        public override void Start()
        { }

        public override void Stop()
        { }
    }
}
