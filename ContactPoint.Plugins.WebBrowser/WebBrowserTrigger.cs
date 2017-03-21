using System;
using System.Collections.Generic;
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
            if (call.State == CallState.ACTIVE && call.IsIncoming && !call.Tags.ContainsKey("BrowserForm") && _triggerHeaders.Any(x => call.Headers.Contains(x)))
            {
                call.Tags.Add("BrowserForm", null);

                if (_browser != null)
                    _browser.OpenUrl(CreateUrl(_urlPattern, call));
            }
        }

        public void Dispose()
        {
            _callManager.OnCallStateChanged -= CallManager_OnCallStateChanged;
        }

        string CreateUrl(string baseUrl, ICall call)
        {
            var result = baseUrl;
            if (call != null)
            {
                result = result.Replace("%number%", call.Number);

                if (call.Headers != null)
                    result = call.Headers.Aggregate(result, (current, header) => current.Replace('%' + header.Name + '%', header.Value));
            }

            return result;
        }
    }
}
