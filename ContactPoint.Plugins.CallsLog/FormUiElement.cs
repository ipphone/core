using System;
using System.Collections.Generic;
using ContactPoint.Core.PluginManager;
using ContactPoint.Plugins.CallsLog.Properties;

namespace ContactPoint.Plugins.CallsLog
{
    internal class FormUiElement : PluginCheckedUIElementBase
    {
        readonly PluginService _plugin;
        CallLogForm _form;
        bool _formShown;

        public override bool ShowInMenu { get { return true; } }
        public override bool ShowInToolBar { get { return true; } }

        public override System.Drawing.Bitmap Image
        {
            get { return Resources.address_book; }
        }

        public override string Text
        {
            get { return "История звонков"; }
        }

        public override IEnumerable<ContactPoint.Common.PluginManager.IPluginUIElement> Childrens
        {
            get { return null; }
        }

        public override Guid ActionCode
        {
            get { return Guid.Parse("{E75B9E15-6E56-4b09-A406-4B02B85B1FDF}"); }
        }

        public FormUiElement(PluginService plugin)
            : base(plugin)
        {
            _plugin = plugin;

            _form = new CallLogForm(_plugin);
            _form.FormClosed += Form_FormClosed;
        }

        void Form_FormClosed(object sender, System.Windows.Forms.FormClosedEventArgs e)
        {
            _form.FormClosed -= Form_FormClosed;
            _formShown = false;

            Checked = false;

            _form = new CallLogForm(_plugin);
            _form.FormClosed += Form_FormClosed;
        }

        protected override void ExecuteCheckedCommand(object sender, bool checkedValue, object data)
        {
            if (_plugin.IsStarted && checkedValue && !_formShown)
            {
                _form.Show();
                _formShown = true;
            }

            if (_plugin.IsStarted && !checkedValue && _formShown)
            {
                _form.Close();
            }
        }
    }
}
