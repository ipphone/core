using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using ContactPoint.Common;
using ContactPoint.Common.PluginManager;

namespace ContactPoint.Core.PluginManager
{
    internal class PluginManager : IPluginManager
    {
        private readonly Core _core;
        private readonly List<IPluginInformation> _plugins = new List<IPluginInformation>();
        private readonly List<IPluginInformationProvider> _pluginInformationProviders = new List<IPluginInformationProvider>();
        private readonly Dictionary<string, string> _pluginFiles = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);

        internal event Action<string> PluginLoading;

        public event ServiceStartedDelegate Started;
        public event ServiceStoppedDelegate Stopped;

        public ICore Core => _core;
        public IEnumerable<IPluginInformation> Plugins => _plugins;
        public bool IsStarted { get; private set; }

        internal PluginManager(Core core)
        {
            _core = core;

            AppDomain.CurrentDomain.ReflectionOnlyAssemblyResolve += OnReflectionOnlyAssemblyResolve;
            try
            {
                _pluginInformationProviders.Add(new ExternallyConfigurablePluginInformationProvider(_core));
            }
            catch (Exception e)
            {
                Logger.LogWarn(e, "Cannot load plugins external configuration");
            }

            try
            {
                _pluginInformationProviders.Add(new ReflectionPluginInformationProvider(_core));
            }
            catch (Exception e)
            {
                Logger.LogWarn(e, "Cannot load plugins configuration");
            }

            Logger.LogNotice("Plugin manager initialized");
        }

        public bool? ExecuteAction(Guid actionCode, Guid? pluginId = null, object data = null)
        {
            var query = ((pluginId.HasValue ? Plugins.Where(x => pluginId.Equals(x?.ID)) : Plugins) ?? Enumerable.Empty<IPluginInformation>())
                .SelectMany(x => x?.GetInstance(false)?.UIElements ?? Enumerable.Empty<IPluginUIElement>())
                .Where(x => actionCode.Equals(x?.ActionCode));

            bool? any = null;
            foreach (var el in query)
            {
                any = true;
                try
                {
                    Logger.LogNotice($"Executing action '{actionCode}' for element '{el.Text ?? el.Id.ToString()}', of plugin '{el.Plugin?.GetType()}'");
                    if (Ui.Current.InvokeRequired) Ui.Current.Invoke(new Action<IPluginManager, object>(el.Execute), new [] { this, data });
                    else el.Execute(this, data);
                }
                catch (Exception e)
                {
                    any = false;
                    Logger.LogWarn(e);
                }
            }

            return any;
        }

        internal void LoadPluginsFromDirectory(string path)
        {
            _pluginFiles.Clear();
            LoadAssembliesFromDirectory(path);
            //LoadAssembliesFromDirectory(Environment.CurrentDirectory, false);

            foreach (var file in _pluginFiles.Values)
            {
                try
                {
                    PluginLoading?.Invoke(Path.GetFileNameWithoutExtension(file));

                    var pluginPath = Path.GetFullPath(file);
                    if (!string.IsNullOrEmpty(pluginPath))
                    {
                        var targetAssembly = Assembly.ReflectionOnlyLoadFrom(pluginPath);
                        foreach (var assembly in targetAssembly.GetReferencedAssemblies())
                        {
                            try
                            {
                                Assembly.Load(assembly.FullName);
                                continue;
                            }
                            catch (Exception e)
                            {
                                Logger.LogWarn(e, $"Unable to load assembly {assembly.FullName}");
                            }

                            var assemblyPath = Path.Combine(path, assembly.Name + ".dll");
                            try
                            {
                                Assembly.LoadFrom(assemblyPath);
                                continue;
                            }
                            catch (Exception e)
                            {
                                Logger.LogWarn(e, $"Unable to load assembly '{assembly.FullName}' from '{assemblyPath}'");
                            }
                        }

                        RegisterPlugin(targetAssembly, pluginPath);
                    }
                }
                catch (ReflectionTypeLoadException typeLoadException)
                {
                    Logger.LogWarn(string.Format("Cannot load type using reflection from '{1}': {0}",
                        typeLoadException.Message, file));
                    if (typeLoadException.LoaderExceptions != null)
                    {
                        foreach (var ex in typeLoadException.LoaderExceptions)
                        {
                            Logger.LogError(ex);
                        }
                    }
                }
                catch (TypeLoadException typeLoadException)
                {
                    Logger.LogWarn(string.Format("Cannot load type '{0}' from '{2}': {1}", typeLoadException.TypeName,
                        typeLoadException.Message, file));
                }
                catch (Exception e)
                {
                    Logger.LogWarn(e);
                }
            }
        }

        private void LoadAssembliesFromDirectory(string path, bool recurse = true)
        {
            Logger.LogNotice($"Loading plugins from '{path}'");
            if (!Directory.Exists(path))
            {
                Logger.LogWarn($"Path doesn't exists '{path}'");
                return;
            }

            if (recurse)
            {
                foreach (var dir in Directory.GetDirectories(path))
                {
                    LoadAssembliesFromDirectory(dir);
                }
            }

            foreach (var file in Directory.GetFiles(path))
            {
                if (!file.EndsWith(".dll"))
                {
                    continue;
                }

                var assemblyName = Path.GetFileNameWithoutExtension(file);

                _pluginFiles[assemblyName] = file;
            }
        }

        private Assembly OnReflectionOnlyAssemblyResolve(object sender, ResolveEventArgs args)
        {
            Logger.LogNotice($"PluginManager '{args.RequestingAssembly.FullName}': attempting to load assembly '{args.Name}' only for reflection");
            if (args.Name.StartsWith("System") && args.Name.Contains("Version=4.0.0.0"))
            {
                Logger.LogNotice($"PluginManager '{args.RequestingAssembly.FullName}': apply version overwrite for '{args.Name}' to '4.0.0.0'");
                return Assembly.ReflectionOnlyLoad(args.Name.Replace("Version=4.0.0.0", "Version=4.0.0.0"));
            }

            return Assembly.ReflectionOnlyLoad(args.Name);
        }

        private void RegisterPlugin(Assembly assembly, string fileName)
        {
            _plugins.AddRange(
                _pluginInformationProviders.SelectMany(p => p.GetPluginInformations(assembly))
                    .GroupBy(x => x.ID, x => x)
                    .Select(x => x.OrderByDescending(o => o.Priority).First()));
        }

        private static void TryStartPlugin(IPluginInformation plugin)
        {
            try
            {
                plugin.Start();
            }
            catch (Exception e)
            {
                Logger.LogWarn(e);
            }
        }

        private void RestartPlugins()
        {
            foreach (var plugin in _plugins)
            {
                try
                {
                    var mustStart = plugin.IsStarted;
                    if (mustStart)
                    {
                        plugin.Stop();
                    }

                    if (mustStart || Core.SettingsManager.GetValueOrSetDefault(plugin.ID.ToString(), true))
                    {
                        TryStartPlugin(plugin);
                    }
                }
                catch (Exception e)
                {
                    Logger.LogWarn(e);
                }
            }
        }

        public void Start()
        {
            if (IsStarted) return;

            RestartPlugins();
            RaiseStartedEvent();
        }

        public void Stop()
        {
            IsStarted = false;
            Stopped?.Invoke(this, "Normal stop");
        }

        private void RaiseStartedEvent()
        {
            IsStarted = true;
            Started?.Invoke(this);
        }
    }
}
