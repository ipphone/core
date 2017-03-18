using System;
using System.Collections.Generic;
using System.Text;
using ContactPoint.Common.PluginManager;
using ContactPoint.Core.PluginManager;
using ContactPoint.Plugins.OneLine.Properties;

namespace ContactPoint.Plugins.OneLine
{
    public class OneLinePluginUIElement : PluginCheckedUIElement
    {
        public static Guid ACTION_CODE = new Guid("{80A08139-0A47-4eea-93BD-E217E24B1C5B}");

        private OneLinePlugin _plugin;

        public override bool ShowInToolBar
        {
            get
            {
                return true;
            }
        }

        public override System.Drawing.Bitmap Image
        {
            get { return Resources._1line; }
        }

        public override string Text
        {
            get { return "Ограничить кол-во линий одной"; }
        }

        public override List<IPluginUIElement> Childrens
        {
            get { return null; }
        }

        public override Guid ActionCode
        {
            get { return ACTION_CODE; }
        }

        public OneLinePluginUIElement(OneLinePlugin plugin)
            : base(plugin)
        {
            this._plugin = plugin;

            this.Checked = this.Plugin.PluginManager.Core.SettingsManager.GetValueOrSetDefault<bool>("OneLinePluginEnabled", true);
            this._plugin.IsOneLine = this.Checked;
        }

        protected override void InternalExecute(object sender)
        {
            this._plugin.IsOneLine = this.Checked;

            this.Plugin.PluginManager.Core.SettingsManager["OneLinePluginEnabled"] = this.Checked;
        }
    }
}
