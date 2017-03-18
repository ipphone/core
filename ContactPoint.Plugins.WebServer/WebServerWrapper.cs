using System;
using System.Collections.Generic;
using System.Text;
using ContactPoint.Common;
using System.Net;
using ContactPoint.Plugins.WebServer.Handlers;

namespace ContactPoint.Plugins.WebServer
{
    internal class WebServerWrapper : HttpServer.HttpServer, IService
    {
        public static string PortParam = "WebServerPort";

        private PluginService _plugin;
        private HandlerManager _handlerManager;

        public int Port { get; set; }

        public WebServerWrapper(PluginService plugin)
        {
            _plugin = plugin;

            Port = plugin.PluginManager.Core.SettingsManager.GetValueOrSetDefault<int>(PortParam, 3323);
            IsStarted = false;

            _handlerManager = new HandlerManager();

            // TODO: make it in unified way
            _handlerManager.RegisterHandler("/call", new CallHandler(plugin));
            _handlerManager.RegisterHandler("/callstate", new CallStateHandler(plugin));

            ExceptionThrown += IncomingServer_ExceptionThrown;
        }

        ~WebServerWrapper()
        {
            ExceptionThrown -= IncomingServer_ExceptionThrown;
        }

        public void StopService()
        {
            StopService("Normal stop");
        }

        protected void StopService(string message)
        {
            Logger.LogNotice("Stopping WebServer wrapper");

            if (IsStarted)
                Stop();

            IsStarted = false;
            RaiseStopped(message);
        }

        protected override void HandleRequest(HttpServer.IHttpClientContext context, HttpServer.IHttpRequest request, HttpServer.IHttpResponse response, HttpServer.Sessions.IHttpSession session)
        {
            var handler = _handlerManager[request.Uri.LocalPath];

            if (handler != null)
            {
                try
                {
                    handler.Execute(context, request, response, session);
                }
                catch (Exception e)
                {
                    Logger.LogWarn(e);
                }
            }
            else
            {
                new FileHandler().Execute(context, request, response, session);
            }

            base.HandleRequest(context, request, response, session);
        }

        protected override void WriteLog(HttpServer.LogPrio prio, string message)
        {
            base.WriteLog(prio, message);

            switch (prio)
            {
                case HttpServer.LogPrio.Info: Logger.LogNotice(message); break;
                case HttpServer.LogPrio.Warning: Logger.LogWarn(message); break;
                case HttpServer.LogPrio.Error:
                case HttpServer.LogPrio.Fatal: Logger.LogError(message); break;
            }
        }

        private void IncomingServer_ExceptionThrown(object source, Exception exception)
        {
            Logger.LogWarn(exception);
            Logger.LogNotice("Critical error - web server restarting");

            Stop();
            Start();
        }

        private void RaiseStarted()
        {
            if (Started != null)
                Started(this);
        }

        private void RaiseStopped(string message)
        {
            if (Stopped != null)
                Stopped(this, message);
        }

        #region IService Members

        public event ServiceStartedDelegate Started;

        public event ServiceStoppedDelegate Stopped;

        public void Start()
        {
            Logger.LogNotice("Starting web server");

            try
            {
                Start(IPAddress.Loopback, Port);

                IsStarted = true;
            }
            catch (Exception ex)
            {
                Logger.LogWarn("web server could't start: " + ex.Message);

                StopService(ex.Message);
            }
        }

        public bool IsStarted { get; private set; }

        #endregion
    }
}
