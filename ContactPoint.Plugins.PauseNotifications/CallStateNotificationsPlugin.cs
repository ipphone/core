using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Net.Cache;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using ContactPoint.Common;
using ContactPoint.Common.Contacts;
using ContactPoint.Common.PluginManager;
using ContactPoint.Common.SIP.Account;
using ContactPoint.Core.PluginManager;

namespace ContactPoint.Plugins.PauseNotifications
{
    [Plugin("{0240fea2-63a9-44d5-aa90-0def96479a1d}", "Call State Notifications")]
    public class CallStateNotificationsPlugin : Plugin
    {
        private bool _isStarted;
        private readonly string _uri;
        private readonly string _httpMethod;
        private readonly Func<string, ICall, ISipAccount, string> _callSubstFunc;
        private readonly Func<ICall, bool> _applicabilityFunc;
        private readonly WebProxy _proxy;

        public override IEnumerable<IPluginUIElement> UIElements => null;
        public override bool IsStarted => _isStarted;

        public CallStateNotificationsPlugin(IPluginManager pluginManager)
            : base(pluginManager)
        {
            var applicabilityFunc = _applicabilityFunc;
            var targetStateStr = PluginManager.Core.SettingsManager.GetValueOrSetDefault<string>("CallState", null);
            if (!string.IsNullOrEmpty(targetStateStr))
            {
                var targetState = (CallState) Enum.Parse(typeof(CallState), targetStateStr, true);
                applicabilityFunc = c => c.State == targetState;
            }

            var targetLastStateStr = PluginManager.Core.SettingsManager.GetValueOrSetDefault<string>("LastCallState", null);
            if (!string.IsNullOrEmpty(targetLastStateStr))
            {
                var targetState = (CallState)Enum.Parse(typeof(CallState), targetLastStateStr, true);
                var func = applicabilityFunc;
                applicabilityFunc = func == null
                    ? (Func<ICall, bool>) (c => c.LastState == targetState)
                    : (c => c.LastState == targetState && func(c));
            }

            if (applicabilityFunc == null)
            {
                return;
            }

            _applicabilityFunc = c => c != null && c.State != c.LastState && applicabilityFunc(c);

            _uri = PluginManager.Core.SettingsManager.GetValueOrSetDefault("Uri", string.Empty);
            _httpMethod = PluginManager.Core.SettingsManager.GetValueOrSetDefault("Method", "GET");

            var proxyAddress = PluginManager.Core.SettingsManager.GetValueOrSetDefault<string>("Proxy", null);
            _proxy = !string.IsNullOrEmpty(proxyAddress) ? new WebProxy(proxyAddress, false) : null;

            var callSubstFunc = GetSubstFunc<ICall>(_uri);
            var accountSubstFunc = GetSubstFunc<ISipAccount>(_uri, "account");
            var contactSubstFunc = GetSubstFunc<IContact>(_uri, "contact");

            _callSubstFunc = (s, c, a) => accountSubstFunc(contactSubstFunc(callSubstFunc(s, c), c.Contact), a);
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public override void Start()
        {
            if (_applicabilityFunc == null || _isStarted)
            {
                return;
            }

            _isStarted = true;

            PluginManager.Core.CallManager.OnCallStateChanged += OnCallStateChanged;
            PluginManager.Core.CallManager.OnCallInfoChanged += OnCallInfoChanged;
            PluginManager.Core.CallManager.OnIncomingCall += OnIncomingCall;
            PluginManager.Core.CallManager.OnCallRemoved += OnCallRemoved;

            RaiseStartedEvent();
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public override void Stop()
        {
            if (!_isStarted)
            {
                return;
            }

            _isStarted = false;

            PluginManager.Core.CallManager.OnCallStateChanged -= OnCallStateChanged;
            PluginManager.Core.CallManager.OnCallInfoChanged -= OnCallInfoChanged;
            PluginManager.Core.CallManager.OnIncomingCall -= OnIncomingCall;
            PluginManager.Core.CallManager.OnCallRemoved -= OnCallRemoved;

            RaiseStoppedEvent(string.Empty);
        }

        private Func<string, T, string> GetSubstFunc<T>(string input, string prefix = null)
        {
            prefix = prefix != null ? $"{prefix}:" : null;
            Func<string, T, string> result = (s, p) => s;
            var param = Expression.Parameter(typeof(T), "x");
            var matches = Regex.Matches(input, $@"(?:{{{prefix}(?<propName>[\w\d_]+)}})*", RegexOptions.IgnoreCase)
                .Cast<Match>()
                .Where(x => x != null)
                .Select(x => x.Groups["propName"]?.Value)
                .Where(x => !string.IsNullOrEmpty(x))
                .ToArray();

            foreach (var subst in matches)
            {
                var func = result;
                var valueFunc = Expression.Lambda<Func<T, object>>(Expression.PropertyOrField(param, subst), param).Compile();
                result = (s, a) => func(s, a).Replace($"{{{prefix}{subst}}}", valueFunc(a)?.ToString());
            }

            return result;
        }

        private void OnCallRemoved(ICall call, CallRemoveReason reason)
        {
            OnCallStateChanged(call);
        }

        private void OnCallInfoChanged(ICall call)
        {
            OnCallStateChanged(call);
        }

        private void OnIncomingCall(ICall call)
        {
            OnCallStateChanged(call);
        }

        private void OnCallStateChanged(ICall call)
        {
            if (PluginManager.Core.Sip?.Account == null || call?.State == null || !_applicabilityFunc(call))
            {
                return;
            }

            try
            {
                var uri = _callSubstFunc(_uri, call, PluginManager.Core.Sip.Account);
                using (var client = new WebClient { CachePolicy = new RequestCachePolicy(RequestCacheLevel.NoCacheNoStore), Proxy = _proxy })
                {
                    if ("GET".Equals(_httpMethod, StringComparison.InvariantCultureIgnoreCase))
                    {
                        client.DownloadString(uri);
                    }
                    else
                    {
                        using (var stream = client.OpenWrite(uri, _httpMethod))
                        {
                            stream.Flush();
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Logger.LogWarn(e, "Error sending call state update");
            }
        }
    }
}
