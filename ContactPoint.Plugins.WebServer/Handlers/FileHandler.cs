using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace ContactPoint.Plugins.WebServer.Handlers
{
    internal class FileHandler : IHandler
    {
        public void Execute(HttpServer.IHttpClientContext context, HttpServer.IHttpRequest request, HttpServer.IHttpResponse response, HttpServer.Sessions.IHttpSession session)
        {
            var filePath = request.Uri.LocalPath;

            // Prevent hacking
            filePath = filePath.Replace('/', '\\');
            filePath = filePath.Substring(filePath.LastIndexOf('\\'));

            if (filePath[0] == '\\') filePath = filePath.Substring(1);

            response.Connection = HttpServer.ConnectionType.Close;
            try
            {
                var content = TemplateTools.ReadResourceFile(filePath);
                response.ContentType = GetMimeTypeByExtension(filePath);
                response.ContentLength = content.Length;
                response.SendHeaders();

                response.SendBody(content);
            }
            catch
            {
                response.Reason = "HTTP/1.1 404 Not Found";
                response.Send();
            }
        }

        private string GetMimeTypeByExtension(string fileName)
        {
            var extension = fileName.Substring(fileName.LastIndexOf('.') + 1);

            switch (extension.ToLower())
            {
                case "jpe": 
                case "jpg": 
                case "jpeg": return "image/jpeg";
                case "gif": return "image/gif";
                case "ico": return "image/x-icon";
                case "png": return "image/png";
                default: return "application/octet-stream";
            }
        }
    }
}
