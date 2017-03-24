using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Reflection;
using ContactPoint.Common;
using ContactPoint.Common.PluginManager;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ContactPoint.Core.PluginManager
{
    class ExternallyConfigurablePluginInformationProvider : TypeBasedPluginInformationProvider
    {
        private readonly NameValueCollection _config;

        public ExternallyConfigurablePluginInformationProvider(ICore core) : base(core)
        {
            _config = new NameValueCollection(StringComparer.InvariantCultureIgnoreCase);

            var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "ContactPoint", "plugins.json");

            try
            {
                Logger.LogNotice($"Load plugins external configuration from '{path}'");

                var jRoot = JsonConvert.DeserializeObject<JObject>(File.ReadAllText(path));
                foreach (var item in jRoot.Properties())
                {
                    if (item.Value.Type == JTokenType.Object)
                    {
                        _config.Add(item.Name, item.Value.ToString(Formatting.Indented));
                    }
                    else if (item.Value.Type == JTokenType.Array)
                    {
                        foreach (var subItem in item.Value.Children<JObject>())
                        {
                            _config.Add(item.Name, subItem.ToString(Formatting.Indented));
                        }
                    }
                }

                Logger.LogNotice("Successfully loaded plugins external configurations");
            }
            catch (Exception e)
            {
                Logger.LogWarn(e, "Plugins external configuration file couldn't be loaded from '{0}'", path);
            }
        }

        protected override IEnumerable<IPluginInformation> LoadPluginInformations(Type pluginType)
        {
            var results = new List<IPluginInformation>();
            foreach (var data in _config.GetValues(pluginType.FullName) ?? new string[0])
            {
                try
                {
                    var config = JsonConvert.DeserializeObject<JObject>(data, new JsonSerializerSettings { EqualityComparer = StringComparer.InvariantCultureIgnoreCase });
                    var version = config.Value<string>("Version") ?? 
                        pluginType.Assembly
                            .GetCustomAttributesData()
                            .Where(x => x.Constructor.DeclaringType == typeof(AssemblyFileVersionAttribute))
                            .Select(x => x.ConstructorArguments[0].Value?.ToString())
                            .FirstOrDefault() ?? "1.0.0.0";

                    bool enabled;
                    if (bool.TryParse(config.GetValue("Enabled")?.ToString(), out enabled) && !enabled)
                    {
                        continue;
                    }

                    bool haveSettingsForm;
                    bool.TryParse(config.GetValue("HaveSettingsForm")?.ToString(), out haveSettingsForm);

                    JToken pluginIdToken;
                    var pluginId = config.TryGetValue("Id", out pluginIdToken) ? Guid.Parse(pluginIdToken.Value<string>("Id")) : Guid.NewGuid();
                    var name = config.GetValue("Name")?.ToString() ?? pluginType.Name;

                    results.Add(CreatePluginInformation(pluginType, 
                        pluginId,
                        name,
                        version, 
                        config.GetValue("Info")?.ToString() ?? name,
                        haveSettingsForm,
                        new JObjectReadOnlySettingsManagerSection(config)));
                }
                catch (Exception e)
                {
                    Logger.LogWarn(e, $"Can't load plugin configuration using external configuration for plugin '{pluginType}'");
                }
            }

            return results;
        }
    }
}
