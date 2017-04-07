using System;
using System.Collections.Generic;
using ContactPoint.Common.PluginManager;
using ContactPoint.Core.PluginManager;
using ContactPoint.Plugins.CallTools.Properties;

namespace ContactPoint.Plugins.CallTools.OneLine
{
    public sealed class OneLinePluginUIElement : PluginCheckedUIElementBase
    {
        private readonly OneLineService _service;

        public override bool ShowInToolBar => true;
        public override System.Drawing.Bitmap Image => Resources._1line;
        public override IEnumerable<IPluginUIElement> Childrens => null;

        public override Guid ActionCode { get; }
        public override string Text { get; }

        public OneLinePluginUIElement(IPlugin plugin, OneLineService service, Guid actionCode = default(Guid), string text = null)
            : base(plugin)
        {
            ActionCode = actionCode != default(Guid) ? actionCode : Guid.Parse("{80A08139-0A47-4eea-93BD-E217E24B1C5B}");
            Text = text ?? "Ограничить кол-во линий одной";

            _service = service;
            _service.IsOneLine = Checked = Plugin.PluginManager.Core.SettingsManager.GetValueOrSetDefault("OneLinePluginEnabled", true);
        }

        protected override void ExecuteCheckedCommand(object sender, bool checkedValue, object data)
        {
            _service.IsOneLine = checkedValue;
            Plugin.PluginManager.Core.SettingsManager["OneLinePluginEnabled"] = checkedValue;
        }
    }
}
