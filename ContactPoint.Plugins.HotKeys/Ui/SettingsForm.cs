using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace ContactPoint.Plugins.HotKeys.Ui
{
    public partial class SettingsForm : Form
    {
        private HotKeysPlugin _plugin;

        public SettingsForm(HotKeysPlugin plugin)
        {
            _plugin = plugin;

            InitializeComponent();

            shortcutInputAnswer.Keys = (Keys)_plugin.HotKeysListener.Combinations[0].Combination;
            shortcutInputDrop.Keys = (Keys)_plugin.HotKeysListener.Combinations[1].Combination;
            shortcutInputHold.Keys = (Keys)_plugin.HotKeysListener.Combinations[2].Combination;
        }

        private void buttonOk_Click(object sender, EventArgs e)
        {
            _plugin.HotKeysListener.Combinations[0].Combination = (int)shortcutInputAnswer.Keys;
            _plugin.HotKeysListener.Combinations[1].Combination = (int)shortcutInputDrop.Keys;
            _plugin.HotKeysListener.Combinations[2].Combination = (int)shortcutInputHold.Keys;

            _plugin.PluginManager.Core.SettingsManager.Set("HotKeysPluginAnswerKey", _plugin.HotKeysListener.Combinations[0].Combination);
            _plugin.PluginManager.Core.SettingsManager.Set("HotKeysPluginDropKey", _plugin.HotKeysListener.Combinations[1].Combination);
            _plugin.PluginManager.Core.SettingsManager.Set("HotKeysPluginHoldKey", _plugin.HotKeysListener.Combinations[2].Combination);

            Close();
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
