using System;
using System.Collections.Generic;
using System.Text;
using ContactPoint.Core.PluginManager;
using ContactPoint.Common.PluginManager;
using ContactPoint.Common;

namespace ContactPoint.Plugins.WebServer
{
    [Plugin("{6BE60EE8-06F2-4984-9ED0-4486FCE3210C}", "Web server")]
    public class PluginService : Plugin
    {
        private WebServerWrapper _webServer;
        private bool _startingState = false;

        internal WebServerWrapper WebServer
        {
            get { return _webServer; }
        }

        public override IEnumerable<ContactPoint.Common.PluginManager.IPluginUIElement> UIElements
        {
            get { return null; }
        }

        public override bool IsStarted
        {
            get { return _webServer.IsStarted; }
        }

        public PluginService(IPluginManager pluginManager)
            : base(pluginManager)
        {
            Logger.LogNotice("Web server initializing...");

            TemplateTools.InitializePath(pluginManager.GetPluginPath(this));

            _webServer = new WebServerWrapper(this);

            _webServer.Started += InternalServiceStarted;
            _webServer.Stopped += InternalServiceStopped;
        }

        ~PluginService()
        {
            if (_webServer == null) return;

            Stop();

            _webServer.Started -= InternalServiceStarted;
            _webServer.Stopped -= InternalServiceStopped;
        }

        public override void Start()
        {
            Logger.LogNotice("Starting WebServer integration service");

            _startingState = true;
            TryStart();
        }

        public override void Stop()
        {
            Logger.LogNotice("Stopping WebServer integration service");

            _startingState = false;
            if (IsStarted)
            {
                _webServer.StopService();
            }

            RaiseStoppedEvent("Normal stop");
        }

        private void TryStart()
        {
            if (!IsStarted)
            {
                _webServer.Start();
            }
        }

        void InternalServiceStarted(object sender)
        {
            if (IsStarted && _startingState)
            {
                _startingState = false;

                RaiseStartedEvent();
            }
        }

        void InternalServiceStopped(object sender, string message)
        {
            if (_startingState)
                Stop();
            else if (!IsStarted)
                RaiseStoppedEvent(message);
        }
    }
}
