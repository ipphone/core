using System;
using System.Collections.Generic;
using System.Text;

namespace ContactPoint.Plugins.WebServer
{
    internal interface IHandler
    {
        void Execute(
            HttpServer.IHttpClientContext context, 
            HttpServer.IHttpRequest request, 
            HttpServer.IHttpResponse response, 
            HttpServer.Sessions.IHttpSession session);
    }
}
