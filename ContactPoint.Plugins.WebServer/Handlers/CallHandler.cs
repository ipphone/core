using System;
using System.Collections.Generic;
using System.Text;
using ContactPoint.Common.PluginManager;

namespace ContactPoint.Plugins.WebServer.Handlers
{
    internal class CallHandler : IHandler
    {
        private IPlugin _plugin;

        public CallHandler(IPlugin plugin)
        {
            _plugin = plugin;
        }

        #region IHandler Members

        public void Execute(HttpServer.IHttpClientContext context, HttpServer.IHttpRequest request, HttpServer.IHttpResponse response, HttpServer.Sessions.IHttpSession session)
        {
            if (!request.Param.Contains("number")) return;

            var call = _plugin.PluginManager.Core.CallManager.MakeCall(request.Param["number"].Value);

            var content = TemplateTools.ProcessTemplate(
                "Call.html",
                new Dictionary<string, string>() 
                { 
                    { "number", call.Number }
                });

            response.Connection = HttpServer.ConnectionType.Close;
            response.ContentType = "text/html";
            response.ContentLength = content.Length;
            response.AddHeader("Content-type", "text/html");
            response.SendHeaders();

            response.SendBody(content);

            //response.Connection = HttpServer.ConnectionType.Close;
            //response.Redirect(String.Format("/callstate?call_id={0}", call.SessionId));
            //response.Send();
        }

        #endregion
    }
}
