using System;
using System.Diagnostics;

namespace ContactPoint.Plugins.WebBrowser
{
    class Browser
    {
        private readonly string _exe;
        public string Name { get; private set; }

        public Browser(string name, string exe)
        {
            _exe = exe;
            Name = name;
        }

        public void OpenUrl(string url)
        {
            Process.Start(_exe, url);
        }

        public override string ToString()
        {
            return Name;
        }

        public static Browser Create(string serialized)
        {
            if (string.IsNullOrEmpty(serialized)) return null;

            var browserNameIndex = serialized.IndexOf(';');
            if (browserNameIndex <= 0) return null;

            var name = serialized.Substring(0, browserNameIndex);
            var exe = serialized.Substring(browserNameIndex + 1);

            return new Browser(name, exe);
        }

        public static string Serialize(Browser browser)
        {
            if (browser == null) return String.Empty;

            return string.Format("{0};{1}", browser.Name, browser._exe);
        }
    }
}