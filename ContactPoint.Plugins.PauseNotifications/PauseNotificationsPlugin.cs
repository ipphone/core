using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Net.Cache;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using ContactPoint.Common;
using ContactPoint.Common.PluginManager;
using ContactPoint.Common.SIP.Account;
using ContactPoint.Core.PluginManager;

namespace ContactPoint.Plugins.PauseNotifications
{
    [Plugin("{12AE7ED9-C7C9-45ee-B1B2-C7425E331611}", "Pause Notifications")]
    public class PauseNotificationsPlugin : Plugin
    {
        private bool _isStarted;
        private bool _isActive;
        private readonly string _uri;
        private readonly string _httpMethod;
        private readonly PresenceStatusCode? _targetState;
        private readonly Func<string, ISipAccount, string> _substFunc = (s, a) => s;
        private readonly WebProxy _proxy;

        public override IEnumerable<IPluginUIElement> UIElements => null;
        public override bool IsStarted => _isStarted;

        public PauseNotificationsPlugin(IPluginManager pluginManager)
            : base(pluginManager)
        {
            var targetState = PluginManager.Core.SettingsManager.GetValueOrSetDefault<string>("PauseSipState", null);
            if (string.IsNullOrEmpty(targetState))
            {
                return;
            }

            _targetState = (PresenceStatusCode)Enum.Parse(typeof(PresenceStatusCode), targetState, true);
            _uri = PluginManager.Core.SettingsManager.GetValueOrSetDefault("Uri", string.Empty);
            _httpMethod = PluginManager.Core.SettingsManager.GetValueOrSetDefault("Method", "GET");

            var proxyAddress = PluginManager.Core.SettingsManager.GetValueOrSetDefault<string>("Proxy", null);
            _proxy = !string.IsNullOrEmpty(proxyAddress) ? new WebProxy(proxyAddress, false) : null;

            var matches = Regex.Matches(_uri, @"(?:{(?<propName>[\w\d_]+)})*", RegexOptions.IgnoreCase)
                .Cast<Match>()
                .Where(x => x != null)
                .Select(x => x.Groups[0].Value)
                .Where(x => !string.IsNullOrEmpty(x))
                .ToArray();

            foreach (var subst in matches)
            {
                var param = Expression.Parameter(typeof(ISipAccount), "x");
                var valueExpr = Expression.Lambda<Func<ISipAccount, object>>(Expression.PropertyOrField(param, subst.Trim('{', '}')), param);
                var valueFunc = valueExpr.Compile();
                var substFunc = _substFunc;

                _substFunc = (s, a) => substFunc(s, a).Replace(subst, valueFunc(a)?.ToString());
            }
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public override void Start()
        {
            if (_targetState == null)
            {
                return;
            }

            if (_isActive)
            {
                return;
            }

            PluginManager.Core.Sip.Account.PresenceStatusChanged += AccountPresenceStatusChanged;

            _isStarted = true;
            RaiseStartedEvent();
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public override void Stop()
        {
            if (!_isStarted)
            {
                return;
            }

            PluginManager.Core.Sip.Account.PresenceStatusChanged -= AccountPresenceStatusChanged;

            _isStarted = false;
            RaiseStoppedEvent(string.Empty);
        }

        private void AccountPresenceStatusChanged(ISipAccount account)
        {
            if (_isActive && account.PresenceStatus.Code == _targetState)
            {
                try
                {
                    var uri = _substFunc(_uri, account);
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
                    Logger.LogWarn(e, "Error sending state update");
                }
            }
            else
            {
                _isActive = true;
            }
        }
    }
}
