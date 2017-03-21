using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Microsoft.Win32;

namespace ContactPoint.Plugins.WebBrowser
{
    public partial class WebBrowserSettings : Form
    {
        private readonly WebBrowserPluginDefinition _plugin;

        public WebBrowserSettings(WebBrowserPluginDefinition plugin)
        {
            _plugin = plugin;

            InitializeComponent();
        }

        private void WebBrowserSettings_Load(object sender, EventArgs e)
        {
            comboBoxWebBrowser.DataSource = WebBrowserPluginDefinition.Browsers;
            comboBoxWebBrowser.DisplayMember = "Name";
            comboBoxWebBrowser.SelectedItem = _plugin.Browser;

            txtIncomingUrl.Text = _plugin.IncomingCallUrl;
            txtTriggerHeaders.Text = String.Join(";", _plugin.TriggerHeaders);
        }

        private void buttonOk_Click(object sender, EventArgs e)
        {
            _plugin.IncomingCallUrl = txtIncomingUrl.Text;
            _plugin.Browser = comboBoxWebBrowser.SelectedItem as Browser;
            _plugin.TriggerHeaders = txtTriggerHeaders.Text.Split(';');

            DialogResult = DialogResult.OK;
            Close();
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}
