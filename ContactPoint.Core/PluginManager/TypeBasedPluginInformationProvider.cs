using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ContactPoint.Common;
using ContactPoint.Common.PluginManager;

namespace ContactPoint.Core.PluginManager
{
    abstract class TypeBasedPluginInformationProvider : IPluginInformationProvider
    {
        private readonly ICore _core;

        protected TypeBasedPluginInformationProvider(ICore core)
        {
            _core = core;
        }

        public IEnumerable<IPluginInformation> GetPluginInformations(Assembly assembly)
        {
            var plugins = new List<IPluginInformation>();
            foreach (var pluginType in FindPluginTypes(assembly.GetTypes()))
            {
                plugins.AddRange(LoadPluginInformations(pluginType) ?? Enumerable.Empty<IPluginInformation>());
            }

            return plugins;
        }

        protected abstract IEnumerable<IPluginInformation> LoadPluginInformations(Type pluginType);

        protected virtual IPluginInformation CreatePluginInformation(Type pluginType, Guid pluginId, string pluginName,
            string pluginVersion, string info = null, bool haveSettingsForm = false, ISettingsManagerSection settings = null)
        {
            return new PluginInformation(
                            _core,
                            settings,
                            pluginId,
                            pluginName,
                            pluginVersion,
                            pluginType.Assembly.FullName)
            {
                PluginType = pluginType,
                TypeName = pluginType.FullName,
                FileName = pluginType.Assembly.Location,
                Info = info,
                HaveSettingsForm = haveSettingsForm
            };
        }

        private IEnumerable<Type> FindPluginTypes(Type[] types) => types.Where(type => type.GetInterface(typeof(IPlugin).FullName) != null);
    }
}
