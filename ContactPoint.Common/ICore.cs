using System;
using ContactPoint.Common.Audio;
using ContactPoint.Common.PluginManager;
using ContactPoint.Common.SIP;
using ContactPoint.Common.Contacts;

namespace ContactPoint.Common
{
    /// <summary>
    /// Main object interface in program
    /// </summary>
    public interface ICore : IDisposable
    {
        /// <summary>
        /// SIP control object
        /// </summary>
        ISip Sip { get; }

        /// <summary>
        /// Call manager. Contains information about all client calls
        /// </summary>
        ICallManager CallManager { get; }

        /// <summary>
        /// Plugin manager. Contains information about all plugins
        /// </summary>
        IPluginManager PluginManager { get; }

        /// <summary>
        /// Settings manager. Store program settings
        /// </summary>
        ISettingsManagerSection SettingsManager { get; }

        /// <summary>
        /// Contacts manager
        /// </summary>
        IContactsManager ContactsManager { get; }

        /// <summary>
        /// Audio manager. Controls audio in program
        /// </summary>
        IAudio Audio { get; }

        /// <summary>
        /// Shutdown core
        /// </summary>
        void Shutdown();
    }
}
