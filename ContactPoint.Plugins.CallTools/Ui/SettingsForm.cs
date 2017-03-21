using System;
using System.Windows.Forms;

namespace ContactPoint.Plugins.CallTools.Ui {
    public partial class SettingsForm : Form {
        private readonly CallToolsPlugin _plugin;
        private readonly CallToolsOptions _copiedOptions;

        public SettingsForm(CallToolsPlugin plugin) {
            _plugin = plugin;
            _copiedOptions = _plugin.CallToolsOptions.Clone();

            InitializeComponent();

            BindOptions();
        }

        private void BindOptions() {
            AutoAnswerCheckBox.DataBindings.Add(new Binding("Checked", _copiedOptions, "AutoAnswer"));
            OneLineServiceCheckBox.DataBindings.Add(new Binding("Checked", _copiedOptions, "OneLineService"));
            ShowIncomingCallWindowCheckBox.DataBindings.Add(new Binding("Checked", _copiedOptions, "ShowIncomingCallWindow"));
            DontHideCallWindowCheckBox.DataBindings.Add(new Binding("Checked", _copiedOptions, "NotHideCallWindow"));
            DontHideCallWindowCheckBox.DataBindings.Add(new Binding("Enabled", ShowIncomingCallWindowCheckBox, "Checked"));
            DontHideCallWindowLabel.DataBindings.Add(new Binding("Enabled", ShowIncomingCallWindowCheckBox, "Checked"));
        }
        
        private void SaveOptions() {
            _plugin.CallToolsOptions.Assign(_copiedOptions);
        }

        private void ButtonOkClick(object sender, EventArgs e) {
            SaveOptions();
            Close();
        }

        private void ButtonCancelClick(object sender, EventArgs e) {
            Close();
        }
    }
}