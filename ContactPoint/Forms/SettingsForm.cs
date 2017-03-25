using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using ContactPoint.BaseDesign;
using ContactPoint.Common;
using ContactPoint.Common.Audio;
using ContactPoint.Common.PluginManager;
using ContactPoint.Common.SIP;
using ContactPoint.Common.SIP.Account;
using Microsoft.Win32;
using OpenFileDialog = System.Windows.Forms.OpenFileDialog;

namespace ContactPoint.Forms
{
    public enum SettingsFormPart
    {
        General = 0,
        Audio,
        Codecs,
        Plugins
    }

    public partial class SettingsForm : Form
    {
        private ICore _core;

        private Action<IEnumerable<IAudioDevice>> _audioDevicesChangedHandler;
        private Action<IAudioDevice> _audioDeviceChangedHandler;

        private bool _restart = false;
        public bool RestartRequired
        {
            get { return _restart; }
            private set { _restart = value; }
        }

        private bool _reregister = false;
        public bool ReregisterRequired
        {
            get { return _reregister; }
            private set { _reregister = value; }
        }

        public event EventHandler ReRegistered;

        public SettingsForm(ICore core)
        {
            _audioDevicesChangedHandler = new Action<IEnumerable<IAudioDevice>>(AudioDevicesChanged);
            _audioDeviceChangedHandler = new Action<IAudioDevice>(AudioDeviceChanged);

            InitializeComponent();

            _core = core;

            _core.Audio.AudioDevicesAdded += _audioDevicesChangedHandler;
            _core.Audio.AudioDevicesRemoved += _audioDevicesChangedHandler;
            _core.Audio.PlaybackDeviceChanged += _audioDeviceChangedHandler;
            _core.Audio.RecordingDeviceChanged += _audioDeviceChangedHandler;
        }

        public void Show(SettingsFormPart part)
        {
            switch (part)
            {
                case SettingsFormPart.Audio: tabControlSettings.SelectedTab = tabPageSettingsAudio; break;
                case SettingsFormPart.Codecs: tabControlSettings.SelectedTab = tabCodecsPage; break;
                case SettingsFormPart.Plugins: tabControlSettings.SelectedTab = tabPagePlugins; break;
                default: tabControlSettings.SelectedTab = tabPageSettingsSIP; break;
            }

            Show();
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            _core.Audio.AudioDevicesAdded -= _audioDevicesChangedHandler;
            _core.Audio.AudioDevicesRemoved -= _audioDevicesChangedHandler;
            _core.Audio.PlaybackDeviceChanged -= _audioDeviceChangedHandler;
            _core.Audio.RecordingDeviceChanged -= _audioDeviceChangedHandler;
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void clearAll()
        {
            textBoxDisplayName.Text = "";
            textBoxUsername.Text = "";
            textBoxPassword.Text = "";
            textBoxRegistrarAddress.Text = "";
            textBoxDomain.Text = "*";
        }

        private bool ApplySettings()
        {
            this._core.Sip.Account.Server = textBoxRegistrarAddress.Text;
            this._core.Sip.Account.FullName = textBoxDisplayName.Text;
            this._core.Sip.Account.UserName = textBoxUsername.Text;
            this._core.Sip.Account.Password = textBoxPassword.Text;
            this._core.Sip.Account.Realm = textBoxDomain.Text;

            this._core.Sip.TransportType = (SipTransportType)comboBoxSIPTransport.SelectedIndex;

            // additional settings
            this._core.Sip.Account.Port = Int16.Parse(textBoxListenPort.Text);
            this._core.Sip.Account.RtpPort = Int32.Parse(textBoxRTPPort.Text);
            this._core.Sip.Account.RegisterTimeout = Int32.Parse(textBoxExpires.Text);
            this._core.Sip.VoiceActiveDetection = checkBoxVAD.Checked;
            this._core.Sip.EchoCancelationTimeout = Int32.Parse(textBoxECTail.Text);
            this._core.Sip.DTMFMode = (SipDTMFMode)comboBoxDtmfMode.SelectedIndex;

            ContactPoint.Services.AutoAnswerService.Instance.IsStarted = checkBoxAutoAnswer.Checked;
            ContactPoint.Services.AutoAnswerService.Instance.Interval = Int32.Parse(textBoxAutoAnswerPeriod.Text);

            // save audio devices
            _cancelAudioEvents = true;
            this._core.Audio.PlaybackDevice = this.comboBoxPlaybackDevices.SelectedItem as IAudioDevice;
            this._core.Audio.RecordingDevice = this.comboBoxRecordingDevices.SelectedItem as IAudioDevice;
            _cancelAudioEvents = false;

            _core.SettingsManager.Set("AudioPlayOnIncoming", checkBoxPlayIncomingSound.Checked);
            _core.SettingsManager.Set("AudioPlayOnIncomingAndActive", checkBoxPlayIncomingSound.Checked && checkBoxPlayIncomingWhileActive.Checked);
            _core.SettingsManager.Set("AudioPlayOutgoing", checkBoxPlayRingback.Checked);

            // check if at least 1 codec selected
            if (listBoxEnCodecs.Items.Count == 0)
            {
                (new AlertForm(ContactPoint.CaptionStrings.CaptionStrings.NoCodecsSelected)).ShowDialog();

                this.tabControlSettings.SelectTab(this.tabCodecsPage);

                return false;
            }

            // save codec list
            foreach (ISipCodec codec in this._core.Sip.Codecs)
            {
                if (this.listBoxEnCodecs.Items.Contains(codec.Name))
                    codec.Enabled = true;
                if (this.listBoxDisCodecs.Items.Contains(codec.Name))
                    codec.Enabled = false;
            }

            // save license
            if (!String.IsNullOrEmpty(textBoxLicensePath.Text))
                try
                {
                    using (var key = Registry.CurrentUser.CreateSubKey(@"SOFTWARE\ContactPoint\IpPhone", RegistryKeyPermissionCheck.ReadWriteSubTree, RegistryOptions.None))
                    {
                        if (key != null)
                        {
                            key.SetValue("LicenseObject", File.ReadAllBytes(textBoxLicensePath.Text));
                            RestartRequired = true;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.LogWarn(ex);
                }

            return true;
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            if (ApplySettings())
            {
                this._core.SettingsManager.Save();

                // reinitialize stack
                if (RestartRequired)
                {
                    (new AlertForm(ContactPoint.CaptionStrings.CaptionStrings.RestartNeeded)).ShowDialog();

                    Application.Restart();
                }

                if (ReregisterRequired)
                {
                    if (this._core.Sip.Account.RegisterState != SipAccountState.Offline)
                        this._core.Sip.Account.UnRegister();

                    this._core.Sip.Account.Register();

                    if (ReRegistered != null)
                        ReRegistered(this, null);
                }

                Close();
            }
        }

        private void SettingsForm_Load(object sender, EventArgs e)
        {
            textBoxDisplayName.Text = this._core.Sip.Account.FullName;
            textBoxUsername.Text = this._core.Sip.Account.UserName;
            textBoxPassword.Text = this._core.Sip.Account.Password;
            textBoxRegistrarAddress.Text = this._core.Sip.Account.Server;
            textBoxDomain.Text = this._core.Sip.Account.Realm;
            comboBoxSIPTransport.SelectedIndex = (int)this._core.Sip.TransportType;
            textBoxAutoAnswerPeriod.Text = ContactPoint.Services.AutoAnswerService.Instance.Interval.ToString();
            checkBoxAutoAnswer.Checked = ContactPoint.Services.AutoAnswerService.Instance.IsStarted;
            checkBoxAutoAnswer_CheckedChanged(null, null);

            textBoxListenPort.Text = this._core.Sip.Account.Port.ToString();
            textBoxRTPPort.Text = this._core.Sip.Account.RtpPort.ToString();

            comboBoxDtmfMode.SelectedIndex = (int)this._core.Sip.DTMFMode;
            textBoxExpires.Text = this._core.Sip.Account.RegisterTimeout.ToString();
            checkBoxVAD.Checked = this._core.Sip.VoiceActiveDetection;
            textBoxECTail.Text = this._core.Sip.EchoCancelationTimeout.ToString();

            checkBoxPlayIncomingSound.Checked = _core.SettingsManager.GetValueOrSetDefault("AudioPlayOnIncoming", true);
            checkBoxPlayIncomingWhileActive.Enabled = checkBoxPlayIncomingSound.Checked;
            checkBoxPlayIncomingWhileActive.Checked = _core.SettingsManager.GetValueOrSetDefault("AudioPlayOnIncomingAndActive", false);
            checkBoxPlayRingback.Checked = _core.SettingsManager.GetValueOrSetDefault("AudioPlayOutgoing", true);

            // init audio
            ReloadAudioDevices();

            // load codecs from system 
            foreach (ISipCodec codec in this._core.Sip.Codecs)
                if (codec.Enabled) listBoxEnCodecs.Items.Add(codec.Name);
                else listBoxDisCodecs.Items.Add(codec.Name);

            // Load plugins settings
            this.listBoxPlugins.DisplayMember = "Name";
            this.listBoxPlugins.Items.Clear();
            foreach (var plugin in this._core.PluginManager.Plugins)
            {
                plugin.Started += new ServiceStartedDelegate(plugin_Started);
                plugin.Stopped += new ServiceStoppedDelegate(plugin_Stopped);

                this.listBoxPlugins.Items.Add(plugin);
            }

            // set stack flags
            ReregisterRequired = false;
            RestartRequired = false;
        }
        
        public void activateTab(int index)
        {
            this.tabControlSettings.SelectTab(index);
        }

        private void buttonEnable_Click(object sender, EventArgs e)
        {
            if (listBoxDisCodecs.SelectedItems.Count > 0)
            {
                // get selected item from disabled codecs
                listBoxEnCodecs.Items.Add(listBoxDisCodecs.SelectedItem);
                // remove from disabled list
                listBoxDisCodecs.Items.Remove(listBoxDisCodecs.SelectedItem);
            }
        }

        private void buttonDisable_Click(object sender, EventArgs e)
        {
            if (listBoxEnCodecs.SelectedItems.Count > 0)
            {
                // get selected item from enabled codecs
                listBoxDisCodecs.Items.Add(listBoxEnCodecs.SelectedItem);
                // remove from enabled list
                listBoxEnCodecs.Items.Remove(listBoxEnCodecs.SelectedItem);
            }
        }

        private void reregistrationRequired_TextChanged(object sender, EventArgs e)
        {
            ReregisterRequired = true;
        }

        private void restartRequired_TextChanged(object sender, EventArgs e)
        {
            RestartRequired = true;
            ReregisterRequired = true;
        }

        private void checkBoxDefault_CheckedChanged(object sender, EventArgs e)
        {
            ReregisterRequired = true;
        }

        private void numEvaluate_KeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e)
        {
            if (((e.KeyChar < '0') || (e.KeyChar > '9')) && (e.KeyChar != '\b'))
            {
                e.Handled = true;
            }
        }

        private void buttonLogger_Click(object sender, EventArgs e)
        {
            (new LoggerForm()).Show();
        }

        private void checkBoxPlayIncomingSound_CheckedChanged(object sender, EventArgs e)
        {
            if (!checkBoxPlayIncomingSound.Checked)
                checkBoxPlayIncomingWhileActive.Checked = checkBoxPlayIncomingWhileActive.Enabled = false;
            else
                checkBoxPlayIncomingWhileActive.Enabled = true;
        }

        private void checkBoxAutoAnswer_CheckedChanged(object sender, EventArgs e)
        {
            textBoxAutoAnswerPeriod.Enabled = checkBoxAutoAnswer.Checked;
        }

        private void buttonLicenseUpload_Click(object sender, EventArgs e)
        {
            var uploadDialog = new OpenFileDialog();
            if (uploadDialog.ShowDialog() == DialogResult.OK)
            {
                if (!uploadDialog.CheckFileExists) return;

                textBoxLicensePath.Text = Path.GetFullPath(uploadDialog.FileName);
            }
        }

        #region Audio

        private bool _cancelAudioEvents = false;
        private void ReloadAudioDevices()
        {
            if (InvokeRequired)
            {
                BeginInvoke(new EmptyDelegate(ReloadAudioDevices));

                return;
            }

            // Load playback devices
            this.comboBoxPlaybackDevices.Items.Clear();

            this.comboBoxPlaybackDevices.DisplayMember = "Name";
            this.comboBoxPlaybackDevices.ValueMember = "Name";

            this.comboBoxPlaybackDevices.Items.Add("<None>");
            var devices = this._core.Audio.AudioDevices.ToArray();
            foreach (IAudioDevice device in devices)
                if (device.Type == AudioDeviceType.Playback)
                    this.comboBoxPlaybackDevices.Items.Add(device);

            if (_core.Audio.PlaybackDevice != null) this.comboBoxPlaybackDevices.SelectedItem = devices.FirstOrDefault(x => x.Name == this._core.Audio.PlaybackDevice.Name && x.Type == AudioDeviceType.Playback);
            else comboBoxPlaybackDevices.SelectedItem = comboBoxPlaybackDevices.Items[0];

            // Load recording devices
            this.comboBoxRecordingDevices.Items.Clear();

            this.comboBoxRecordingDevices.DisplayMember = "Name";
            this.comboBoxRecordingDevices.ValueMember = "Name";

            this.comboBoxRecordingDevices.Items.Add("<None>");
            foreach (IAudioDevice device in devices)
                if (device.Type == AudioDeviceType.Recording)
                    this.comboBoxRecordingDevices.Items.Add(device);

            if (_core.Audio.RecordingDevice != null) this.comboBoxRecordingDevices.SelectedItem = devices.FirstOrDefault(x => x.Name == this._core.Audio.RecordingDevice.Name && x.Type == AudioDeviceType.Recording);
            else comboBoxRecordingDevices.SelectedItem = comboBoxRecordingDevices.Items[0];
        }

        private void AudioDeviceChanged(IAudioDevice device)
        {
            if (!_cancelAudioEvents)
                ReloadAudioDevices();
        }

        private void AudioDevicesChanged(IEnumerable<IAudioDevice> devices)
        {
            if (!_cancelAudioEvents)
                ReloadAudioDevices();
        }

        #endregion Audio

        #region Plugins

        private void listBoxPlugins_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.listBoxPlugins.SelectedItem != null)
            {
                var plugin = this.listBoxPlugins.SelectedItem as IPluginInformation;

                if (plugin != null)
                {
                    this.groupBoxPluginInfo.Enabled = true;

                    this.labelPluginID.Text = plugin.ID.ToString();
                    this.labelPluginName.Text = plugin.Name;
                    this.labelPluginVersion.Text = plugin.Version;

                    SetStartStopButtonStates(plugin);

                    this.buttonPluginSettings.Enabled = plugin.HaveSettingsForm;

                    this.checkBoxAutoStartPlugin.Checked = this._core.SettingsManager.GetValueOrSetDefault(plugin.ID.ToString(), true);

                    return;
                }
            }

            this.groupBoxPluginInfo.Enabled = false;
            this.labelPluginName.Text = "-";
            this.labelPluginVersion.Text = "-";
            this.labelPluginID.Text = "-";

            this.buttonPluginStart.Enabled = false;
            this.buttonPluginStop.Enabled = false;
            this.buttonPluginSettings.Enabled = false;
        }

        private void SetStartStopButtonStates(IPluginInformation plugin) {
            if (plugin.IsStarted) {
                this.buttonPluginStart.Enabled = false;
                this.buttonPluginStop.Enabled = true;
            } else {
                this.buttonPluginStart.Enabled = true;
                this.buttonPluginStop.Enabled = false;
            }
        }

        private void buttonPluginStart_Click(object sender, EventArgs e)
        {
            this.buttonPluginStart.Enabled = false;

            if (this.listBoxPlugins.SelectedItem != null)
            {
                var plugin = this.listBoxPlugins.SelectedItem as IPluginInformation;

                if (plugin != null)
                {
                    plugin.Start();
                }
                SetStartStopButtonStates(plugin);
            }
        }

        private void buttonPluginStop_Click(object sender, EventArgs e)
        {
            this.buttonPluginStop.Enabled = false;

            if (this.listBoxPlugins.SelectedItem != null)
            {
                var plugin = this.listBoxPlugins.SelectedItem as IPluginInformation;

                if (plugin != null)
                {
                    plugin.Stop();
                }
                SetStartStopButtonStates(plugin);
            }
        }

        private void buttonPluginSettings_Click(object sender, EventArgs e)
        {
            if (this.listBoxPlugins.SelectedItem != null)
            {
                var plugin = this.listBoxPlugins.SelectedItem as IPluginInformation;

                if (plugin != null && plugin.HaveSettingsForm)
                {
                    plugin.ShowSettingsDialog();
                }
            }
        }

        private void checkBoxAutoStartPlugin_CheckedChanged(object sender, EventArgs e)
        {
            if (this.listBoxPlugins.SelectedItem != null)
            {
                var plugin = this.listBoxPlugins.SelectedItem as IPluginInformation;

                if (plugin != null)
                {
                    this._core.SettingsManager.Set(plugin.ID.ToString(), this.checkBoxAutoStartPlugin.Checked);
                }
            }
        }

        void plugin_Stopped(object sender, string message)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => plugin_Stopped(sender, message)));

                return;
            }

            if (this.listBoxPlugins.SelectedItem != null)
            {
                IPlugin plugin = this.listBoxPlugins.SelectedItem as IPlugin;

                if (plugin != null && plugin == sender)
                {
                    this.buttonPluginStart.Enabled = true;
                }
            }
        }

        void plugin_Started(object sender)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => plugin_Started(sender)));

                return;
            }

            if (this.listBoxPlugins.SelectedItem != null)
            {
                IPlugin plugin = this.listBoxPlugins.SelectedItem as IPlugin;

                if (plugin != null && plugin == sender)
                {
                    this.buttonPluginStop.Enabled = true;
                }
            }
        }

        #endregion
    }
}
