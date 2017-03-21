using ContactPoint.Common;
using ContactPoint.Common.Audio;
using ContactPoint.Common.Contacts;
using ContactPoint.Common.PluginManager;
using ContactPoint.Common.SIP;
using ContactPoint.Core.Settings;

namespace ContactPoint.Core.Helpers
{
    static class CoreWrapperExtensions
    {
        public static ICore CreateChildCore(this ICore core, CoreSettings settings)
        {
            return new CoreWrapper(core, settings);
        }

        private class CoreWrapper : ICore
        {
            public ISip Sip { get; }
            public ICallManager CallManager { get; }
            public IPluginManager PluginManager { get; }
            public ISettingsManagerSection SettingsManager { get; }
            public IContactsManager ContactsManager { get; }
            public IAudio Audio { get; }

            public CoreWrapper(ICore core, CoreSettings coreSettings)
            {
                Sip = coreSettings.Sip ?? core.Sip;
                CallManager = coreSettings.CallManager ?? core.CallManager;
                PluginManager = coreSettings.PluginManager ?? core.PluginManager;
                SettingsManager = coreSettings.SettingsManager ?? core.SettingsManager;
                ContactsManager = coreSettings.ContactsManager ?? core.ContactsManager;
                Audio = coreSettings.Audio ?? core.Audio;
            }

            public void Shutdown()
            { }

            public void Dispose()
            { }
        }
    }
}
