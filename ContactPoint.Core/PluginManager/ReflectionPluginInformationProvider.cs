using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ContactPoint.Common;
using ContactPoint.Common.PluginManager;

namespace ContactPoint.Core.PluginManager
{
    class ReflectionPluginInformationProvider : TypeBasedPluginInformationProvider
    {
        public ReflectionPluginInformationProvider(ICore core) : base(core)
        { }

        private string GetNamedArgumentValue(CustomAttributeData attr, string name)
        {
            return attr?.NamedArguments?
                .Where(x => x.MemberInfo?.Name == name)
                .Select(x => x.TypedValue.Value?.ToString())
                .FirstOrDefault();
        }

        protected override IEnumerable<IPluginInformation> LoadPluginInformations(Type pluginType)
        {
            var results = new List<IPluginInformation>();
            foreach (var attr in CustomAttributeData.GetCustomAttributes(pluginType))
            {
                if (attr.Constructor.DeclaringType?.FullName == typeof(PluginAttribute).FullName)
                {
                    try
                    {
                        var settingsForm = false;
                        bool.TryParse(GetNamedArgumentValue(attr, "HaveSettingsForm"), out settingsForm);

                        var pluginInformation = CreatePluginInformation(
                            pluginType,
                            Guid.Parse(attr.ConstructorArguments[0].Value.ToString()),
                            attr.ConstructorArguments[1].Value.ToString(),
                            (pluginType.Assembly.ReflectionOnly ? null : pluginType.Assembly.GetCustomAttribute<AssemblyFileVersionAttribute>()?.Version) ?? pluginType.Assembly.GetName().Version.ToString(4),
                            GetNamedArgumentValue(attr, "Info"),
                            settingsForm);

                        Logger.LogNotice($"Plugin information loaded: {pluginInformation}");

                        results.Add(pluginInformation);
                    }
                    catch (Exception e)
                    {
                        Logger.LogWarn(e, $"Can't load plugin configuration using reflection for plugin '{pluginType}'");
                    }
                }
            }

            return results;
        }
    }
}
