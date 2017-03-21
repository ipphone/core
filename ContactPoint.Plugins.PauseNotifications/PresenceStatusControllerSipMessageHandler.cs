using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.XPath;
using ContactPoint.Common;
using ContactPoint.Common.PluginManager;
using ContactPoint.Common.SIP.Account;
using ContactPoint.Core.PluginManager;

namespace ContactPoint.Plugins.PauseNotifications
{
    [Plugin("{13a1b678-c92d-45a8-b1a1-c3f618374021}", "SIP Presence Status Controller")]
    public class PresenceStatusControllerSipMessageHandler : Plugin
    {
        private static readonly XPathExpression PeerIdExpr;
        private static readonly XPathExpression StatusNameExpr;
        private static readonly XPathExpression NoteExpr;
        private static readonly Regex PeerRegex;

        private bool _isStarted;
        private readonly Func<string, bool> _criteriaFunc;
        private readonly Guid _targetActionCode;
        private readonly Guid? _targetPluginId;

        public override IEnumerable<IPluginUIElement> UIElements { get; } = null;
        public override bool IsStarted => _isStarted;

        static PresenceStatusControllerSipMessageHandler()
        {
            var xmlNsManager = new XmlNamespaceManager(new NameTable());
            xmlNsManager.AddNamespace("x", "urn:ietf:params:xml:ns:pidf");
            xmlNsManager.AddNamespace("pp", "urn:ietf:params:xml:ns:pidf:person");
            xmlNsManager.AddNamespace("es", "urn:ietf:params:xml:ns:pidf:rpid:status:rpid-status");
            xmlNsManager.AddNamespace("ep", "urn:ietf:params:xml:ns:pidf:rpid:rpid-person");

            PeerIdExpr = XPathExpression.Compile("/x:presence/x:tuple/x:contact", xmlNsManager);
            StatusNameExpr = XPathExpression.Compile("/x:presence/pp:person/x:status/ep:activities/ep:*", xmlNsManager);
            NoteExpr = XPathExpression.Compile("/x:presence/x:note", xmlNsManager);

            PeerRegex = new Regex(@"(^)(?:sip:)?(?<peerId>[\w\d\-\._]+)(?:(@|$)+)");
        }

        public PresenceStatusControllerSipMessageHandler(IPluginManager pluginManager) : base(pluginManager)
        {
            _targetActionCode = Guid.Parse(pluginManager.Core.SettingsManager.Get<string>("TargetActionCode"));

            var activityName = pluginManager.Core.SettingsManager.Get<string>("ActivityName");
            if (!string.IsNullOrEmpty(activityName))
            {
                _criteriaFunc = x => activityName.Equals(x, StringComparison.InvariantCultureIgnoreCase);
            }
            else
            {
                var names = Enum.GetNames(typeof(PresenceStatusCode));
                _criteriaFunc = x => names.Contains(x, StringComparer.InvariantCultureIgnoreCase);
            }

            Guid pluginId;
            _targetPluginId = Guid.TryParse(pluginManager.Core.SettingsManager.Get<string>("TargetPluginId"), out pluginId) ? (Guid?)pluginId : null;
        }

        public override void Start()
        {
            if (_isStarted)
            {
                return;
            }

            PluginManager.Core.Sip.Messenger.MessageReceived += MessengerOnMessageReceived;

            _isStarted = true;
            RaiseStartedEvent();
        }

        public override void Stop()
        {
            if (!_isStarted)
            {
                return;
            }

            _isStarted = false;
            PluginManager.Core.Sip.Messenger.MessageReceived -= MessengerOnMessageReceived;

            RaiseStoppedEvent("Normal stop");
        }

        private void MessengerOnMessageReceived(string sender, string message)
        {
            Logger.LogNotice($"Message received from '{sender}': {message}");
            try
            {
                using (var reader = new StringReader(message))
                {
                    var nav = new XPathDocument(reader).CreateNavigator();
                    var peerUri = nav.SelectSingleNode(PeerIdExpr)?.Value;
                    var note = nav.SelectSingleNode(NoteExpr)?.Value ?? string.Empty;
                    var statusName = nav.SelectSingleNode(StatusNameExpr)?.LocalName;

                    if (statusName == null)
                    {
                        throw new InvalidOperationException("StatusName is not provided cannot be parsed");
                    }

                    if (peerUri == null || PeerRegex.Match(peerUri).Groups["peerId"].Value != PluginManager.Core.Sip.Account.UserName)
                    {
                        throw new InvalidOperationException("Peer URI is not provided or cannot be parsed");
                    }

                    if (_criteriaFunc(statusName))
                    {
                        PluginManager.ExecuteAction(_targetActionCode, _targetPluginId, note);
                    }
                }
            }
            catch (InvalidOperationException e)
            {
                Logger.LogWarn(e, "Required data missing");
            }
            catch (Exception e)
            {
                Logger.LogWarn(e, "Unable to parse message as Presence/XML");
            }
        }
    }
}
