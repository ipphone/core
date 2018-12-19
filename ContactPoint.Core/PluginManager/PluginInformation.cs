using System;
using ContactPoint.Common.PluginManager;
using ContactPoint.Common;

namespace ContactPoint.Core.PluginManager
{
    internal class PluginInformation : IPluginInformation
    {
        private readonly object _lockObj = new object();

        public event ServiceStartedDelegate Started;
        public event ServiceStoppedDelegate Stopped;

        private readonly ISettingsManagerSection _settings;
        private readonly ICore _core;
        private IPlugin _instance;

        public Guid ID { get; }
        public string Name { get; }
        public string Version { get; }
        public string AssemblyName { get; }
        public bool HaveSettingsForm { get; set; }
        public int Priority { get; set; }

        public string Info { get; set; }
        public string TypeName { get; set; }
        public string FileName { get; set; }
        public Type PluginType { get; set; }
        public bool IsStarted => _instance != null && _instance.IsStarted;

        public PluginInformation(ICore core, ISettingsManagerSection settings, Guid id, string name, string version, string assemblyName)
        {
            _core = core;
            _settings = settings ?? (core as Core)?.SettingsManagerInternal.GetSection(id.ToString()) ?? core.SettingsManager;

            ID = id;
            Name = name;
            Version = version;
            AssemblyName = assemblyName;
        }

        public void ShowSettingsDialog()
        {
            if (!HaveSettingsForm) return;
            if (_instance == null && CreateInstance() == null) return;

            _instance?.ShowSettingsDialog();
        }

        public void Start()
        {
            lock (_lockObj)
            {
                if (_instance == null && CreateInstance() == null) return;
                if (_instance == null) return;
                if (_instance.IsStarted) return;
            }

            _instance.Start();
        }

        public void Stop()
        {
            lock (_lockObj)
            {
                if (_instance == null) return;
                if (!_instance.IsStarted) return;
            }

            _instance.Stop();
        }

        public IPlugin GetInstance(bool create)
        {
            if (_instance == null && create)
            {
                lock (_lockObj)
                {
                    if (_instance == null)
                    {
                        CreateInstance();
                    }
                }
            }

            return _instance;
        }

        private object CreateInstance()
        {
            if (_instance != null)
            {
                return _instance;
            }

            try
            {
                IPluginManager wrappedPluginManager = new PluginManagerWrapper(this, _core, _settings);

                var assembly = AppDomain.CurrentDomain.Load(PluginType.Assembly.GetName());
                var pluginType = assembly.GetType(TypeName, true, false);

                _instance = (IPlugin) Activator.CreateInstance(pluginType, wrappedPluginManager);
                _instance.Started += OnInstanceStarted;
                _instance.Stopped += OnInstanceStopped;

                if (_instance.AddressBook != null)
                {
                    (_core as Core)?.ContactsManager.RegisterAddressBook(_instance.AddressBook);
                }

                return _instance;
            }
            catch (Exception e)
            {
                Logger.LogWarn(e, $"Cannot create instance of plugin '{TypeName}'");
            }

            return null;
        }

        private void OnInstanceStarted(object sender)
        {
            Started?.Invoke(sender);
        }

        private void OnInstanceStopped(object sender, string message)
        {
            Stopped?.Invoke(sender, message);
        }

        public override string ToString() => $"Plugin: {Name}; Id: {ID}; Assembly: {AssemblyName}; Type: {TypeName}; Location: {FileName}; Instance: {_instance}";
    }
}
