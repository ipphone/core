using System;
using System.Linq;
using System.Text;
using ContactPoint.Common;

namespace ContactPoint.Plugins.WebBrowser
{
    class WebBrowserTrigger : IDisposable
    {
        private readonly ICallManager _callManager;
        private readonly Browser _browser;
        private readonly string _urlPattern;
        private readonly string[] _triggerHeaders;

        public WebBrowserTrigger(ICallManager callManager, Browser browser, string urlPattern, string[] triggerHeaders)
        {
            _callManager = callManager;
            _browser = browser;
            _urlPattern = urlPattern;
            _triggerHeaders = triggerHeaders;

            _callManager.OnCallStateChanged += CallManager_OnCallStateChanged;
        }

        private void CallManager_OnCallStateChanged(ICall call)
        {
            if (_browser == null)
            {
                return;
            }

            if (call.State == CallState.ACTIVE && call.IsIncoming)
            {
                if (call.Tags.ContainsKey("ExternalWebBrowser"))
                {
                    return;
                }

                if (_triggerHeaders.Length > 0 && !_triggerHeaders.Any(x => call.Headers.Contains(x)))
                {
                    return;
                }

                var url = CreateUrl(_urlPattern, call);
                if (string.IsNullOrWhiteSpace(url))
                {
                    return;
                }

                call.Tags.Add("ExternalWebBrowser", url);

                _browser.OpenUrl(url);
            }
        }

        public void Dispose()
        {
            _callManager.OnCallStateChanged -= CallManager_OnCallStateChanged;
        }

        private static string CreateUrl(string baseUrl, ICall call)
        {
            if (call == null)
            {
                return string.Empty;
            }

            var resultBuilder = new StringBuilder(baseUrl).Replace("%number%", call.Number);
            
            if (call.Headers != null)
            {
                call.Headers.Aggregate(resultBuilder, (current, header) => current.Replace('%' + header.Name + '%', header.Value));
            }

            return resultBuilder.ToString();
        }
    }
}
