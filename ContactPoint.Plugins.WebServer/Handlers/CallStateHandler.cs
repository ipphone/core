using System;
using System.Collections.Generic;
using System.Text;
using ContactPoint.Common.PluginManager;
using ContactPoint.Common.CallManager;
using ContactPoint.Common;

namespace ContactPoint.Plugins.WebServer.Handlers
{
    internal class CallStateHandler : IHandler
    {
        private IPlugin _plugin;

        public CallStateHandler(IPlugin plugin)
        {
            _plugin = plugin;
        }

        public void Execute(HttpServer.IHttpClientContext context, HttpServer.IHttpRequest request, HttpServer.IHttpResponse response, HttpServer.Sessions.IHttpSession session)
        {
            if (!request.Param.Contains("call_id")) return;

            var call = _plugin.PluginManager.Core.CallManager[Int32.Parse(request.Param["call_id"].Value)];

            var content = TemplateTools.ProcessTemplate(
                "CallState.html", 
                new Dictionary<string, string>() 
                { 
                    { "number", call.Number },
                    { "state", ConvertState(call.State) }
                });

            response.Connection = HttpServer.ConnectionType.Close;
            response.ContentType = "text/html";
            response.ContentLength = content.Length;
            response.AddHeader("Content-type", "text/html");
            response.SendHeaders();

            response.SendBody(content);
        }

        private string ConvertState(CallState state)
        {
            switch (state)
            {
                case CallState.ACTIVE: return "Active";
                case CallState.ALERTING:
                case CallState.CONNECTING: return "Calling";
                case CallState.INCOMING: return "Incoming";
                case CallState.HOLDING: return "Holding";
                default: return "Ended";
            }
        }
    }
}
