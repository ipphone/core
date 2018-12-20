using System;
using System.Collections.Generic;
using ContactPoint.Common;
using ContactPoint.Common.PluginManager;
using ContactPoint.Core.Helpers;
using ContactPoint.Core.Settings;

namespace ContactPoint.Core.PluginManager
{
    internal class PluginManagerWrapper : IPluginManager
    {
        private readonly ICore _core;

        #pragma warning disable 0067
        public event ServiceStartedDelegate Started;
        public event ServiceStoppedDelegate Stopped;
        #pragma warning restore 0067

        public ICore Core { get; }
        public IEnumerable<IPluginInformation> Plugins { get; }
        public bool IsStarted => true;

        public PluginManagerWrapper(IPluginInformation pluginInformation, ICore core, ISettingsManagerSection settingsManager)
        {
            _core = core;

            Plugins = new [] { pluginInformation };

            Core = core.CreateChildCore(new CoreSettings {
                PluginManager = this,
                SettingsManager = settingsManager,
            });
        }

        public bool? ExecuteAction(Guid actionId, Guid? pluginId = null, object data = null)
        {
            return _core.PluginManager.ExecuteAction(actionId, pluginId, data);
        }

        public void Start()
        {
            // Do nothing on attempting to start PluginManager from Plugin
        }

        public void Stop()
        {
            // Do nothing on attempting to stop PluginManager from Plugin
        }
    }
}
