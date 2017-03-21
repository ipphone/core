using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ContactPoint.Common.PluginManager;
using ContactPoint.Core.PluginManager;

namespace ContactPoint.Plugins.MySqlAddressBook
{
    [Plugin("{15BECC65-D1B0-4829-B67D-3AC44F4255E6}", "MySQL address book", HaveSettingsForm = true)]
    public class MySqlAddressBookPlugin : Plugin
    {
        private bool _isStarted;
        private readonly SqlConnectionManager _connectionManager;

        public override IEnumerable<IPluginUIElement> UIElements
        {
            get { return null; }
        }

        public override bool IsStarted
        {
            get { return _isStarted; }
        }

        internal SqlConnectionManager ConnectionManager
        {
            get { return _connectionManager; }
        }

        public MySqlAddressBookPlugin(IPluginManager pluginManager)
            : base(pluginManager)
        {
            _connectionManager = new SqlConnectionManager(pluginManager.Core.SettingsManager.GetValueOrSetDefault("ConnectionString", String.Empty));
        }

        public override void Start()
        {
            _isStarted = true;
            RaiseStartedEvent();
        }

        public override void Stop()
        {
            _isStarted = false;
            RaiseStoppedEvent(String.Empty);
        }
    }
}
