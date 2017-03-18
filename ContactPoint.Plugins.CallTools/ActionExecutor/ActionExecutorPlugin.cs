using System;
using System.IO;
using System.Xml.XPath;
using ContactPoint.Common.PluginManager;
using ContactPoint.Core.PluginManager;

namespace ContactPoint.Plugins.CallTools.ActionExecutor
{
    public class ActionExecutorPlugin : Plugin
    {
        private static readonly XPathExpression ActionsExpr;

        private readonly IPluginManager _pluginManager;

        static ActionExecutorPlugin()
        {
            ActionsExpr = XPathExpression.Compile("/actions/*");
        }

        public ActionExecutorPlugin(IPluginManager pluginManager) : base(pluginManager)
        {
            _pluginManager = pluginManager;
        }

        public override void Start()
        {
            _pluginManager.Core.Sip.Messenger.MessageReceived += OnMessageReceived;
            base.Start();
        }

        public override void Stop()
        {
            _pluginManager.Core.Sip.Messenger.MessageReceived -= OnMessageReceived;
            base.Stop();
        }

        private void OnMessageReceived(string sender, string message)
        {
            if (string.IsNullOrEmpty(message))
            {
                return;
            }

            using (var reader = new StringReader(message))
            {
                var nav = new XPathDocument(reader).CreateNavigator();
                foreach (XPathNavigator actionNode in nav.Select(ActionsExpr))
                {
                    if (actionNode.Name.Equals("uiElementActionRef", StringComparison.InvariantCultureIgnoreCase))
                    {
                        var actionId = Guid.Parse(actionNode.GetAttribute("Id", string.Empty));
                        var data = actionNode.Value;

                        Guid pluginId;
                        Guid.TryParse(actionNode.GetAttribute("PluginId", string.Empty), out pluginId);

                        _pluginManager.ExecuteAction(actionId, !Guid.Empty.Equals(pluginId) ? (Guid?)pluginId : null, data);
                    }
                }
            }

        }
    }
}
