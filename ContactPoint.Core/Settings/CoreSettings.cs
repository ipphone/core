using ContactPoint.Common;
using ContactPoint.Common.Audio;
using ContactPoint.Common.Contacts;
using ContactPoint.Common.PluginManager;
using ContactPoint.Common.SIP;

namespace ContactPoint.Core.Settings
{
    class CoreSettings
    {
        public ISip Sip { get; set; }
        public ICallManager CallManager { get; set; }
        public IPluginManager PluginManager { get; set; }
        public ISettingsManagerSection SettingsManager { get; set; }
        public IContactsManager ContactsManager { get; set; }
        public IAudio Audio { get; set; }
    }
}
