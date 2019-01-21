using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using ContactPoint.Common.PluginManager;
using ContactPoint.Core.PluginManager;
using Microsoft.Win32;

namespace ContactPoint.Plugins.WebBrowser
{
    [Plugin("{5D9C4631-B7AD-4501-A07B-4F130AA7A17F}", "External Web Browser popups", HaveSettingsForm = true)]
    public class WebBrowserPluginDefinition : Plugin
    {
        internal static IEnumerable<Browser> Browsers { get; private set; }

        private string _incomingCallUrl;
        private bool _isStarted = false;
        private string[] _triggerHeaders = new string[0];
        private Browser _browser;
        private WebBrowserTrigger _trigger;

        public override IEnumerable<IPluginUIElement> UIElements { get { return null; } }
        public override bool IsStarted { get { return _isStarted; } }

        internal string IncomingCallUrl
        {
            get { return _incomingCallUrl; }
            set
            {
                _incomingCallUrl = value;
                PluginManager.Core.SettingsManager.Set("IncomingCallUrl", value);
            }
        }

        internal string[] TriggerHeaders
        {
            get { return _triggerHeaders; }
            set
            {
                _triggerHeaders = value;
                PluginManager.Core.SettingsManager.Set("TriggerHeaders", String.Join(";", value));
            }
        }

        internal Browser Browser
        {
            get { return _browser; }
            set
            {
                _browser = value;
                PluginManager.Core.SettingsManager.Set("Browser", Browser.Serialize(value));
            }
        }

        public override void ShowSettingsDialog()
        {
            var form = new WebBrowserSettings(this);
            if (form.ShowDialog() == DialogResult.OK)
            {
                if (_isStarted)
                {
                    Stop();
                    Start();
                }
            }
        }

        static WebBrowserPluginDefinition()
        {
            LoadBrowsers();
        }

        public WebBrowserPluginDefinition(IPluginManager pluginManager)
            : base(pluginManager)
        {
            _incomingCallUrl = PluginManager.Core.SettingsManager.GetValueOrSetDefault("IncomingCallUrl", String.Empty);
            _triggerHeaders = PluginManager.Core.SettingsManager.GetValueOrSetDefault("TriggerHeaders", String.Empty).Split(';');
            _browser = WebBrowser.Browser.Create(PluginManager.Core.SettingsManager.GetValueOrSetDefault("Browser", String.Empty));

            Browsers = LoadBrowsers();
        }

        public override void Start()
        {
            if (Browser == null || string.IsNullOrEmpty(_incomingCallUrl) || _triggerHeaders.Length == 0)
                return;

            _trigger = new WebBrowserTrigger(PluginManager.Core.CallManager, Browser, _incomingCallUrl, _triggerHeaders);
            _isStarted = true;

            RaiseStartedEvent();
        }

        public override void Stop()
        {
            if (_trigger != null)
                _trigger.Dispose();

            _trigger = null;

            _isStarted = false;

            RaiseStoppedEvent("Normal stop");
        }

        private static IEnumerable<Browser> LoadBrowsers()
        {
            var browsers = new List<Browser>();
            const string browserListKey = @"SOFTWARE\Clients\StartMenuInternet";
            using (var clients = Registry.LocalMachine.OpenSubKey(browserListKey))
            {
                if (clients == null) return Enumerable.Empty<Browser>();

                foreach (var client in clients.GetSubKeyNames())
                {
                    using (var clientKey = clients.OpenSubKey(client))
                    {
                        if (clientKey == null) continue;

                        var name = (string)clientKey.GetValue(string.Empty);
                        using (var commandKey = clientKey.OpenSubKey(@"shell\open\command"))
                        {
                            if (commandKey == null) continue;

                            browsers.Add(new Browser(name, (string)commandKey.GetValue(string.Empty)));
                        }
                    }
                }
            }

            return browsers;
        }
    }
}
