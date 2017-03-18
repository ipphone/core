using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.IO;

namespace ContactPoint.Plugins.WebServer
{
    internal static class TemplateTools
    {
        private static string DEFAULT_LOCATION = @"webserver\templates";

        public static void InitializePath(string defaultLocation)
        {
            DEFAULT_LOCATION = Path.Combine(defaultLocation, "templates");
        }

        public static byte[] ReadResourceFile(string fileName)
        {
            return File.ReadAllBytes(Path.Combine(DEFAULT_LOCATION, fileName));
        }

        public static string ReadTemplateFile(string fileName)
        {
            return File.ReadAllText(Path.Combine(DEFAULT_LOCATION, fileName));
        }

        public static byte[] ProcessTemplate(string fileName, IDictionary<string, string> replaces)
        {
            var content = ReadTemplateFile(fileName);

            foreach (var replaceToken in replaces)
                content = content.Replace(String.Format("%{0}%", replaceToken.Key), replaceToken.Value);

            return Encoding.UTF8.GetBytes(content);
        }
    }
}
