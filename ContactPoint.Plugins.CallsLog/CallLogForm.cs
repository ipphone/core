using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace ContactPoint.Plugins.CallsLog
{
    public partial class CallLogForm : Form
    {
        private PluginService _plugin;

        public CallLogForm(PluginService plugin)
        {
            _plugin = plugin;

            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            _plugin.Stopped += _plugin_Stopped;
            _plugin.CallEntryAdded += CallEntryAdded;

            foreach (var item in _plugin.Calls)
                CallEntryAdded(item);

            base.OnLoad(e);
        }

        protected override void OnClosed(EventArgs e)
        {
            _plugin.Stopped -= _plugin_Stopped;
            _plugin.CallEntryAdded -= CallEntryAdded;

            base.OnClosed(e);
        }

        void CallEntryAdded(CallEntry obj)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => CallEntryAdded(obj)), null);

                return;
            }

            var listViewItem = new ListViewItem();

            if (obj.IsIncoming && obj.IsAnswered) listViewItem.ImageIndex = 1;
            else if (!obj.IsIncoming) listViewItem.ImageIndex = 2;
            else listViewItem.ImageIndex = 0;

            listViewItem.SubItems.AddRange(
                new [] {
                    new System.Windows.Forms.ListViewItem.ListViewSubItem(listViewItem, obj.Number),
                    new System.Windows.Forms.ListViewItem.ListViewSubItem(listViewItem, obj.Name),
                    new System.Windows.Forms.ListViewItem.ListViewSubItem(listViewItem, obj.CallDate.ToString("dd.MM.yyyy HH:mm:ss")),
                    new System.Windows.Forms.ListViewItem.ListViewSubItem(listViewItem, obj.Duration.ToString()),
                });

            listView1.Items.Insert(0, listViewItem);
        }

        void _plugin_Stopped(object sender, string message)
        {
            Close();
        }

        private void listView1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (listView1.SelectedItems.Count > 0)
            {
                var selectedItem = listView1.SelectedItems[0];

                _plugin.PluginManager.Core.CallManager.MakeCall(selectedItem.SubItems[1].Text);
            }
        }
    }
}
