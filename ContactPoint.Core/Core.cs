using System;
using System.ComponentModel;
using ContactPoint.Common;
using ContactPoint.Common.Audio;
using ContactPoint.Common.PluginManager;
using ContactPoint.Common.SIP;
using ContactPoint.Common.SIP.Account;
using ContactPoint.Common.Contacts;
using ContactPoint.Contacts;

namespace ContactPoint.Core
{
    internal class Core : ICore
    {
        private readonly PluginManager.PluginManager _pluginManager;
        private readonly Settings.SettingsManager _settingsManager;
        private readonly ContactsManager _contactsManager;
        private readonly IAudio _audio;
        private readonly SIP.SIP _sip;

        public static event Action<string> PartLoading;

        public ICallManager CallManager { get; }
        public ISettingsManagerSection SettingsManager { get; }

        public ISip Sip => _sip;
        public IPluginManager PluginManager => _pluginManager;
        public IAudio Audio => _audio;
        internal Settings.SettingsManager SettingsManagerInternal => _settingsManager;
        internal ContactsManager ContactsManager => _contactsManager;

        IContactsManager ICore.ContactsManager => _contactsManager;

        internal Core(ISynchronizeInvoke syncInvoke)
        {
            var pluginLoadingHandler = new Action<string>(x => { RaisePartLoadingEvent($"Loading plugin {x}..."); });

            RaisePartLoadingEvent("Loading Settings...");
            _settingsManager = new Settings.SettingsManager(this);
            SettingsManager = _settingsManager.GetSection("Core");

            RaisePartLoadingEvent("Loading SIP...");
            _sip = new SIP.SIP(this, syncInvoke);

            RaisePartLoadingEvent("Loading Audio...");
            _audio = new Audio.Audio(this);

            RaisePartLoadingEvent("Initializing Audio...");
            _sip.InitializeAudio();

            // Load core classes
            RaisePartLoadingEvent("Loading Calls core...");
            CallManager = new CallManager.CallManager(this);

            RaisePartLoadingEvent("Loading Plugins...");
            _pluginManager = new PluginManager.PluginManager(this);

            RaisePartLoadingEvent("Loading Contacts...");
            _contactsManager = new ContactsManager(this, syncInvoke);

            _pluginManager.PluginLoading += pluginLoadingHandler;
            _pluginManager.LoadPluginsFromDirectory(System.IO.Path.GetFullPath("plugins"));
            _pluginManager.PluginLoading -= pluginLoadingHandler;
            _pluginManager.Start();

            RaisePartLoadingEvent("Core loaded successfully. Starting...");
        }

        ~Core()
        {
            Shutdown();
        }

        public void Shutdown()
        {
            SettingsManager?.Save();

            if (Sip?.Account?.PresenceStatus != null &&
                Sip.Account.PresenceStatus.Code != PresenceStatusCode.Offline &&
                Sip.Account.PresenceStatus.Code != PresenceStatusCode.Unknown)
                Sip.Account.UnRegister();

            _audio?.Dispose();
        }

        private static void RaisePartLoadingEvent(string partName)
        {
            PartLoading?.Invoke(partName);
        }

        #region IDisposable Members

        public void Dispose()
        {
            GC.SuppressFinalize(this);
            Shutdown();
        }

        #endregion
    }
}
