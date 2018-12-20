using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;
using ComponentFactory.Krypton.Toolkit;
using ContactPoint.BaseDesign;
using ContactPoint.BaseDesign.Wpf;
using ContactPoint.Commands;
using ContactPoint.Common;
using ContactPoint.Common.Audio;
using ContactPoint.Common.PluginManager;
using ContactPoint.Common.SIP.Account;
using ContactPoint.Controls;
using ContactPoint.Core;
using ContactPoint.Properties;
using ContactPoint.Services;

namespace ContactPoint.Forms
{
    public partial class MainForm : KryptonForm, IUi
    {
        private readonly string _baseTitle = CaptionStrings.CaptionStrings.BaseTitle;
        private readonly MainFormPhoneLineControl[] _mainFormPhoneLineControls = new MainFormPhoneLineControl[5];
        //private readonly MainFormShadeControl _shade;

        private MainFormPhoneLineControl _lastMainFormPhoneLineControl;
        private ICore _core;
        private bool _transferMode;

        internal ICore Core
        {
            get { return _core; }
            set
            {
                if (_core != null)
                    throw new InvalidOperationException("Core object is already assigned");

                _core = value;

                // Subscribe for neccesary events
                _core.Sip.Account.RegisterStateChanged += OnAccountRegisterStateChanged;

                _core.CallManager.OnCallStateChanged += CallManager_OnCallStateChanged;
                _core.CallManager.OnCallRemoved += CallManager_OnCallRemoved;

                _core.Sip.Account.PresenceStatusChanged += OnAccountPresenceStatusChanged;

                _mainFormPhoneStatusControl.Core = _core;
            }
        }

        private string _header = "";
        public string Header
        {
            get { return _header; }
            set
            {
                Text = value;
                windowHeader.Text = value;
                notifyIconTray.Text = value;
            }
        }

        public StartPhoneCallCommand CallOnStartup { get; set; }

        private bool TransferMode
        {
            get { return _transferMode; }
            set
            {
                MainFormPhoneLineControl lineControl = GetActiveUILineWrapper();

                if (value &&
                    lineControl != null &&
                    lineControl.Call != null &&
                    (lineControl.Call.State == CallState.ACTIVE ||
                     lineControl.Call.State == CallState.HOLDING))
                {
                    _transferMode = true;

                    lineControl.Call.Hold();

                    ChangeCallButton(Color.FromArgb(236, 191, 13), Resources.btn_transfer);

                    txtNumber.SelectAll();
                    txtNumber.Focus();
                }
                else
                {
                    _transferMode = false;

                    ChangeCallButton(Color.FromArgb(99, 196, 18), Resources.green);
                }

                btnTransfer.Checked = _transferMode;
            }
        }

        public MainForm()
        {
            //_shade = new MainFormShadeControl();
            //_shade.ButtonClicked += Shade_ButtonClicked;

            InitializeComponent();

            //_shade.MouseDown += pictureBox1_MouseDown;
            //_shade.MouseMove += pictureBox1_MouseMove;
            //_shade.MouseUp += pictureBox1_MouseUp;

            btnReconnect.Visible = false;

            Ui.SetCurrent(this);
            NotifyManager.Initialize(this);

            _messageTransport = new SharedFileMessageTransportHost();
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == Program.WM_COPYDATA && m.WParam.ToInt32() == Program.MAKECALL_MESSAGE_ID)
            {
                var st = (COPYDATASTRUCT)Marshal.PtrToStructure(m.LParam, typeof(COPYDATASTRUCT));

                if (st.lpData.Length > 0)
                {
                    CallInternal(st.lpData);
                }
            }

            base.WndProc(ref m);
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            if (_core == null)
            {
                throw new NullReferenceException("Core is not initialized");
            }

            Ui.SetCurrent(this);

            Header = string.Format(_baseTitle, CaptionStrings.CaptionStrings.StateInitializing);

            _mainFormPhoneLineControls[0] = new MainFormPhoneLineControl(Core, 0, btnLine1, labelCallerIDName, txtNumber, lblDuration, labelCallState, btnHold);
            _mainFormPhoneLineControls[1] = new MainFormPhoneLineControl(Core, 1, btnLine2, labelCallerIDName, txtNumber, lblDuration, labelCallState, btnHold);
            _mainFormPhoneLineControls[2] = new MainFormPhoneLineControl(Core, 2, btnLine3, labelCallerIDName, txtNumber, lblDuration, labelCallState, btnHold);
            _mainFormPhoneLineControls[3] = new MainFormPhoneLineControl(Core, 3, btnLine4, labelCallerIDName, txtNumber, lblDuration, labelCallState, btnHold);
            _mainFormPhoneLineControls[4] = new MainFormPhoneLineControl(Core, 4, btnLine5, labelCallerIDName, txtNumber, lblDuration, labelCallState, btnHold);

            _mainFormPhoneLineControls[0].Active = true;
            _lastMainFormPhoneLineControl = _mainFormPhoneLineControls[0];

            // Bind to audio subsystem
            LoadAudioValues();

            // UI features
            TopMost = _core.SettingsManager.GetValueOrSetDefault("mainFormTopMost", false);
            toolStripMenuItemTopAll.Checked = TopMost;
            Location = _core.SettingsManager.GetValueOrSetDefault("MainFormLocation", Location);

            // Load plugins ui elements
            ReloadUIElementsFromPlugins();

            AutoAnswerService.Create(_core);

            // Check if first run - run settings
            if (_core.Sip.Account.UserName == "")
            {
                kryptonCommandSettings.PerformExecute();
            }
            else
            {
                // Connect
                _core.Sip.Account.Register();
            }

            labelCallState.Visible = false;
#if DEBUG
            labelCallState.Visible = true;
#endif

            _messageTransport.MessageReceived += OnRemoteMessageReceived;
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!e.CloseReason.HasFlag(CloseReason.ApplicationExitCall) &&
                !e.CloseReason.HasFlag(CloseReason.WindowsShutDown) &&
                !e.CloseReason.HasFlag(CloseReason.TaskManagerClosing))
            {
                if ((new QuestionForm(CaptionStrings.CaptionStrings.CloseQuestion)).ShowDialog() != DialogResult.OK)
                {
                    e.Cancel = true;
                    return;
                }
            }

            _core.SettingsManager.Set("MainFormLocation", Location);
            _core.SettingsManager.Save();

            _messageTransport.Dispose();
        }

        #region CallManager events

        void CallManager_OnCallStateChanged(ICall call)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new CallDelegate(CallManager_OnCallStateChanged), call);

                return;
            }

            if (call != null && call.Line < 5)
            {
                _mainFormPhoneLineControls[call.Line].Call = call;

                if (call.State == CallState.ACTIVE && !_mainFormPhoneLineControls[call.Line].Active)
                {
                    ChangeLineWrapper(_mainFormPhoneLineControls[call.Line]);
                }
            }

            UpdateAccountStateUi(Core);
        }

        void CallManager_OnCallRemoved(ICall call, CallRemoveReason reason)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new CallRemovedDelegate(CallManager_OnCallRemoved), call, reason);

                return;
            }

            if (reason == CallRemoveReason.InvalidData)
            {
                TimeredBaloon.Show(txtNumber, CaptionStrings.CaptionStrings.ErrorWrongNumberTitle, CaptionStrings.CaptionStrings.ErrorWrongNumberDescription, ToolTipIcon.Error, 4000);
            }
            else if (reason == CallRemoveReason.Busy)
            {
                TimeredBaloon.Show(txtNumber, CaptionStrings.CaptionStrings.ErrorBusyTitle, CaptionStrings.CaptionStrings.ErrorBusyDescription, ToolTipIcon.Info, 4000);
            }
            else if ((reason == CallRemoveReason.RemoteHangup || reason == CallRemoveReason.UserHangup || reason == CallRemoveReason.Transfer) && call != null && call.LastState == CallState.ACTIVE)
            {
                var currentLineWrapper = GetActiveUILineWrapper();

                // Normal call stop. Try to activate another call.
                if (currentLineWrapper?.Call == null)
                {
                    ActivateNextLineWrapper();
                }
            }

            UpdateAccountStateUi(Core);
        }

        void ActivateNextLineWrapper()
        {
            MainFormPhoneLineControl nextLineControl = null;
            long lastPartTime = -1, lastPartId = -1;

            foreach (var lineWrapper in _mainFormPhoneLineControls)
                if (lineWrapper?.Call != null && !lineWrapper.Active)
                {
                    if (nextLineControl == null)
                    {
                        nextLineControl = lineWrapper;
                    }

                    // We will try to compare unique ids of call and found call with minimal number.
                    // This is because if call is in queue it can leave queue and enter queue again
                    // and it is better to pick it up as soon as possible.
                    var unid = lineWrapper.Call.Headers["x-unid"];
                    if (unid != null)
                    {
                        // Asterisk unique id. Example: 1349177069.455
                        var uniqueId = unid.Value;

                        var uniqueIdParts = uniqueId.Split('.');

                        if (uniqueIdParts.Length == 2)
                        {
                            long partTime, partId;

                            if (long.TryParse(uniqueIdParts[0], out partTime) && long.TryParse(uniqueIdParts[1], out partId))
                            {
                                if (partTime < lastPartTime || (partTime == lastPartTime && partId < lastPartId))
                                {
                                    nextLineControl = lineWrapper;

                                    lastPartTime = partTime;
                                    lastPartId = partId;
                                }
                                else if (nextLineControl == lineWrapper)
                                {
                                    lastPartTime = partTime;
                                    lastPartId = partId;
                                }
                            }
                        }
                    }
                }

            if (nextLineControl != null)
            {
                ChangeLineWrapper(nextLineControl);
            }
        }

        #endregion

        #region Other core events

        void OnAccountPresenceStatusChanged(ISipAccount account)
        {
            if (account?.PresenceStatus == null)
            {
                return;
            }

            if (InvokeRequired)
            {
                Invoke(new Action<ISipAccount>(OnAccountPresenceStatusChanged), account);
                return;
            }

            labelUserName.Text = $"{account.FullName} ({account.UserName})";
            _mainFormPhoneStatusControl.Text = account.PresenceStatus?.Message ?? @"-";

            //if (account.PresenceStatus.Code == PresenceStatusCode.NotAvailable || account.PresenceStatus.Code == PresenceStatusCode.DND || account.PresenceStatus.Code == PresenceStatusCode.Away)
            //{
            //    // Removed shading for UI issues
            //    Controls.Add(_shade);

            //    grpMain.Enabled = false;

            //    _shade.BringToFront();
            //}
            //else
            //{
            //    if (Controls.Contains(_shade))
            //    {
            //        Controls.Remove(_shade);
            //    }

            //    grpMain.Enabled = true;
            //}
        }

        //void Shade_ButtonClicked(object sender, EventArgs e)
        //{
        //    _core.Sip.Account.PresenceStatus = new PresenceStatus(PresenceStatusCode.Available);
        //}

        void OnAccountRegisterStateChanged(ISipAccount account)
        {
            if (account == null)
            {
                return;
            }

            if (InvokeRequired)
            {
                BeginInvoke(new Action<ISipAccount>(OnAccountRegisterStateChanged), account);

                return;
            }

            labelUserName.Text = $"{account.FullName} ({account.UserName})";

            switch (account.RegisterState)
            {
                case SipAccountState.Connecting:
                    Header = string.Format(_baseTitle, CaptionStrings.CaptionStrings.StateConnecting.ToLower());
                    btnReconnect.Visible = false;
                    break;
                case SipAccountState.Online:
                    btnReconnect.Visible = false;

                    if (!string.IsNullOrEmpty(CallOnStartup?.Destination))
                    {
                        CallInternal(CallOnStartup);
                        CallOnStartup = null;
                    }

                    UpdateAccountStateUi(Core);

                    break;
                default:
                    Header = string.Format(_baseTitle, CaptionStrings.CaptionStrings.StateDisconnected.ToLower());
                    btnReconnect.Visible = true;
                    btnReconnect.Enabled = true;
                    CallOnStartup = null;

                    break;
            }
        }

        void UpdateAccountStateUi(ICore core)
        {
            Header = string.Format(_baseTitle, SipAccountStatusViewModel.Create(core)?.Text?.ToLower() ?? @"-");
        }

        #endregion

        #region Window header routine

        bool _mouseDown;
        private Point _mousePos;

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            _mouseDown = true;

            _mousePos.X = e.X;
            _mousePos.Y = e.Y;
        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            _mouseDown = false;
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (_mouseDown)
            {
                Point current_pos = MousePosition;
                current_pos.X = current_pos.X - _mousePos.X;
                current_pos.Y = current_pos.Y - _mousePos.Y;

                Location = current_pos;
            }
        }

        #endregion

        #region Dialpad routine

        private void btnNum_Click(object sender, EventArgs e)
        {
            try
            {
                AddCharacterToNumber((sender as KryptonButton).Text);
            }
            catch { }
        }

        private void btnBackspace_Click(object sender, EventArgs e)
        {
            Backspace();
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            var lineWrapper = GetActiveUILineWrapper();

            if (lineWrapper != null &&
                lineWrapper.Call != null &&
                (lineWrapper.Call.State == CallState.ACTIVE || lineWrapper.Call.State == CallState.HOLDING))
            {
                Core.CallManager.DoMakeConference();
            }

            //ClearNumber();

            //MainFormPhoneLineControl lineControl = GetActiveUILineWrapper();
            //lineControl.Call.DoMakeConference();
        }

        private void MainForm_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar >= '0' && e.KeyChar <= '9')
                AddCharacterToNumber(e.KeyChar.ToString());
            else if (e.KeyChar == '\b')
                Backspace();
            else
                txtNumber_KeyPress(sender, e);

            if (!txtNumber.Focused)
            {
                txtNumber.Focus();
                txtNumber.Select(txtNumber.Text.Length, 0);
            }
        }

        private void txtNumber_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13)
                btnCall_Click(sender, e);
            if (e.KeyChar == 27)
                ClearNumber();
        }

        private void ClearNumber()
        {
            txtNumber.Text = "";
        }

        private void Backspace()
        {
            if (txtNumber.Text.Length > 0)
                txtNumber.Text = txtNumber.Text.Substring(0, txtNumber.Text.Length - 1);
        }

        private void AddCharacterToNumber(string c)
        {
            MainFormPhoneLineControl lineControl = GetActiveUILineWrapper();

            if (lineControl != null && lineControl.Call != null && lineControl.Call.State == CallState.ACTIVE)
                lineControl.Call.SendDTMF(c);

            if (txtNumber.Text.Length < 15)
                txtNumber.Text += c;
        }

        #endregion

        #region UI events

        private void btnTransfer_Click(object sender, EventArgs e)
        {
            TransferMode = !TransferMode;
        }

        private void btnApplication_Click(object sender, EventArgs e)
        {
            contextMenuMain.Show(PointToScreen(new Point(btnApplication.Left, btnApplication.Top + btnApplication.Height + 5)));
        }

        private void toolStripMenuItemTopAll_Click(object sender, EventArgs e)
        {
            TopMost = toolStripMenuItemTopAll.Checked;

            _core.SettingsManager["mainFormTopMost"] = TopMost;
        }

        private void toolStripMenuItemShow_Click(object sender, EventArgs e)
        {
            notifyIconTray_MouseDoubleClick(sender, null);
        }

        private void notifyIconTray_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            Show();
            //this.ShowInTaskbar = true;
        }

        private void toolStripMenuItemClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnHold_Click(object sender, EventArgs e)
        {
            MainFormPhoneLineControl lineControl = GetActiveUILineWrapper();

            if (lineControl != null)
            {
                if (lineControl.Call != null)
                    lineControl.Call.ToggleHold();
            }
        }

        private void btnMinimize_Click(object sender, EventArgs e)
        {
            Focus();
            Hide();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Focus();

#if DEBUG
            Close();
#else
            Hide();
#endif
        }

        private void kryptonCommandSettings_Execute(object sender, EventArgs e)
        {
            btnReconnect.Visible = false;

            SettingsForm form = new SettingsForm(_core);
            form.Show();
        }

        private void btnDrop_Click(object sender, EventArgs e)
        {
            MainFormPhoneLineControl lineControl = GetActiveUILineWrapper();

            Logger.LogNotice("Trying to drop");

            if (lineControl.Call != null)
                lineControl.Call.Drop();
            else
                Logger.LogWarn("Drop failed - not found");
        }

        private void btnCall_Click(object sender, EventArgs e)
        {
            if (Core.Sip.Account.RegisterState != SipAccountState.Online)
            {
                TimeredBaloon.Show(txtNumber, CaptionStrings.CaptionStrings.ErrorDisconnectedTitle, CaptionStrings.CaptionStrings.ErrorDisconnectedDescription, ToolTipIcon.Error, 3500);

                return;
            }
            if (Core.Audio.PlaybackDevice == null || Core.Audio.RecordingDevice == null)
            {
                TimeredBaloon.Show(txtNumber, CaptionStrings.CaptionStrings.ErrorNoAudioDevices, Core.Audio.PlaybackDevice == null ? CaptionStrings.CaptionStrings.ErrorNoPlaybackDevice : CaptionStrings.CaptionStrings.ErrorNoRecordingDevice, ToolTipIcon.Error, 3500);

                return;
            }

            var lineWrapper = GetActiveUILineWrapper();
            if (lineWrapper.Call != null)
            {
                if (lineWrapper.Call.State != CallState.ACTIVE &&
                    !TransferMode &&
                    txtNumber.Text == lineWrapper.Call.Number)
                {
                    if (lineWrapper.Call.State != CallState.HOLDING)
                    {
                        Logger.LogNotice("Trying to answer call: " + lineWrapper.Call);
                        lineWrapper.Call.Answer();
                    }
                    else
                    {
                        Logger.LogNotice("Trying to answer call: " + lineWrapper.Call);
                        lineWrapper.Call.UnHold();
                    }
                }
                else if (TransferMode && !string.IsNullOrEmpty(txtNumber.Text) &&
                    (lineWrapper.Call.State == CallState.HOLDING ||
                     lineWrapper.Call.State == CallState.ACTIVE))
                {
                    // Transfer Call
                    string number = txtNumber.Text;
                    Logger.LogNotice($"Transfer call '{lineWrapper.Call}' to '{number}'");

                    Task.Factory.StartNew(() =>
                    {
                        lineWrapper.Call.UnHold();
                        lineWrapper.Call.Transfer(number);

                        lineWrapper.Call.Drop();
                    });

                    TransferMode = false;

                    TimeredBaloon.Show(txtNumber, string.Format(CaptionStrings.CaptionStrings.ActionTransferTitle, number), CaptionStrings.CaptionStrings.ActionTransferDescription, ToolTipIcon.Info);
                }
                else if (
                    lineWrapper.Call.State != CallState.ACTIVE &&
                    lineWrapper.Call.Number != txtNumber.Text &&
                    !TransferMode)
                    CallInternal(txtNumber.Text, true);
            }
            else
            {
                if (!string.IsNullOrEmpty(txtNumber.Text))
                    CallInternal(txtNumber.Text);
            }
        }

        private void OnRemoteMessageReceived(object message)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action<object>(OnRemoteMessageReceived), message);
                return;
            }

            var makeCallMessage = message as StartPhoneCallCommand;
            if (makeCallMessage != null)
            {
                CallInternal(makeCallMessage, false);
            }
        }

        private void CallInternal(string number, bool forceActivateLineWrapper = false)
        {
            CallInternal(new StartPhoneCallCommand { Destination = number }, forceActivateLineWrapper);
        }

        private void CallInternal(StartPhoneCallCommand number, bool forceActivateLineWrapper = false)
        {
            try
            {
                var call = _core.CallManager.MakeCall(number.Destination, number.Attributes);
                if (call == null)
                {
                    TimeredBaloon.Show(txtNumber, CaptionStrings.CaptionStrings.ErrorNoAvailableLinesTitle, CaptionStrings.CaptionStrings.ErrorNoAvailableLinesDescription, ToolTipIcon.Error);
                }
                else
                {
                    if (call.Line < 5)
                    {
                        _mainFormPhoneLineControls[call.Line].Call = call;
                    }

                    if (forceActivateLineWrapper)
                    {
                        ChangeLineWrapper(_mainFormPhoneLineControls[call.Line]);
                    }
                }
            }
            catch (Exception e)
            {
                Logger.LogWarn(e);
            }
        }

        private void btnReconnect_Click(object sender, EventArgs e)
        {
            _core.Sip.Account.Register();
        }

        private void btnLine_Click(object sender, EventArgs e)
        {
            ChangeLineWrapper(GetActiveUILineWrapperByButton(sender as KryptonCheckButton));
        }

        private void ChangeLineWrapper(MainFormPhoneLineControl targetLineControl)
        {
            if (_lastMainFormPhoneLineControl != null && _lastMainFormPhoneLineControl != targetLineControl && TransferMode)
            {
                if (targetLineControl.Call != null &&
                    targetLineControl.Call.State == CallState.HOLDING &&
                    _lastMainFormPhoneLineControl.Call != null &&
                    (new QuestionForm(string.Format("Do you want to transfer '{0}' to '{1}'", _lastMainFormPhoneLineControl.Call.Number, targetLineControl.Call.Number))).ShowDialog() == DialogResult.OK)
                {
                    if (targetLineControl.Call.State == CallState.HOLDING)
                    {
                        targetLineControl.Call.UnHold();
                    }

                    if (_lastMainFormPhoneLineControl.Call.State == CallState.HOLDING)
                    {
                        _lastMainFormPhoneLineControl.Call.UnHold();
                    }

                    targetLineControl.Call.TransferAttended(_lastMainFormPhoneLineControl.Call);
                }

                TransferMode = false;
            }

            if (_lastMainFormPhoneLineControl != null)
                _lastMainFormPhoneLineControl.Active = false;

            if (targetLineControl != null)
            {
                _lastMainFormPhoneLineControl = targetLineControl;
                targetLineControl.Active = true;
            }
        }

        private void kryptonCommandExit_Execute(object sender, EventArgs e)
        {
            Close();
        }

        private void kryptonCommandAboutDialog_Execute(object sender, EventArgs e)
        {
            new AboutWindow { DataContext = new AboutWindowViewModel(_core) }.ShowDialog();
        }

        #endregion

        #region Helpers

        private void ChangeCallButton(Color color, Image image)
        {
            Color color2 = Color.FromArgb(
                (color.R - 36 >= 0 ? color.R - 36 : 0),
                (color.G - 9 >= 0 ? color.G - 9 : 0),
                (color.B - 13 >= 0 ? color.B - 13 : 0));

            Color color3 = Color.FromArgb(
                (color.R - 20 >= 0 ? color.R - 20 : 0),
                (color.G + 34 <= 255 ? color.G + 34 : 255),
                (color.B - 11 >= 0 ? color.B - 11 : 0));

            ChangeOnButtonState(btnCall.StateCommon, color, color2, color2, image);
            ChangeOnButtonState(btnCall.OverrideDefault, color, color2, color2, image);
            ChangeOnButtonState(btnCall.StateTracking, color, color3, color2, image);
            ChangeOnButtonState(btnCall.StatePressed, color3, color, color2, image);
        }

        private void ChangeOnButtonState(IPaletteTriple state, Color color1, Color color2, Color borderColor, Image image)
        {
            if (state is PaletteTriple)
            {
                PaletteTriple _state = (PaletteTriple)state;

                _state.Back.Color1 = color1;
                _state.Back.Color2 = color2;
                _state.Border.Color1 = borderColor;

                if (image != null)
                    _state.Back.Image = image;
            }
            if (state is PaletteTripleRedirect)
            {
                PaletteTripleRedirect _state = (PaletteTripleRedirect)state;

                _state.Back.Color1 = color1;
                _state.Back.Color2 = color2;
                _state.Border.Color1 = borderColor;

                if (image != null)
                    _state.Back.Image = image;
            }
        }

        private MainFormPhoneLineControl GetActiveUILineWrapper()
        {
            foreach (MainFormPhoneLineControl item in _mainFormPhoneLineControls)
                if (item.Active)
                    return item;

            return _mainFormPhoneLineControls[0];
        }

        private MainFormPhoneLineControl GetActiveUILineWrapperByButton(KryptonCheckButton button)
        {
            foreach (MainFormPhoneLineControl item in _mainFormPhoneLineControls)
                if (item.ControlButton == button)
                    return item;

            return null;
        }

        #endregion

        #region Audio routine

        private IAudioDevice _recordingDevice;
        private IAudioDevice _playbackDevice;

        /// <summary>
        /// Load initial values
        /// </summary>
        private void LoadAudioValues()
        {
            sldMic.Minimum = 0;
            sldMic.Maximum = 100;

            sldVol.Minimum = 0;
            sldVol.Maximum = 100;

            RefreshRecordingDevices();
            RefreshPlaybackDevices();

            _core.Audio.PlaybackDeviceChanged += PlaybackDeviceChanged;
            _core.Audio.RecordingDeviceChanged += RecordingDeviceChanged;
        }

        private void RefreshRecordingDevices()
        {
            if (InvokeRequired)
            {
                BeginInvoke(new EmptyDelegate(RefreshRecordingDevices));

                return;
            }

            if (_recordingDevice != null)
            {
                _recordingDevice.VolumeChanged -= RecordingDevice_VolumeChanged;
                _recordingDevice.MuteChanged -= RecordingDevice_MuteChanged;
            }

            if (_core.Audio.RecordingDevice != null)
            {
                _recordingDevice = _core.Audio.RecordingDevice;
                _recordingDevice.VolumeChanged += RecordingDevice_VolumeChanged;
                _recordingDevice.MuteChanged += RecordingDevice_MuteChanged;
                sldMic.Value = _recordingDevice.Volume;
                sldMic.Enabled = true;
                btnMute.Enabled = true;
                btnMute.Checked = _core.Audio.RecordingDevice.Mute;
            }
            else
            {
                _recordingDevice = null;
                sldMic.Enabled = false;
                btnMute.Enabled = false;
            }
        }

        private void RefreshPlaybackDevices()
        {
            if (InvokeRequired)
            {
                BeginInvoke(new EmptyDelegate(RefreshPlaybackDevices));

                return;
            }

            if (_playbackDevice != null)
            {
                _playbackDevice.VolumeChanged -= PlaybackDevice_VolumeChanged;
            }

            if (_core.Audio.PlaybackDevice != null)
            {
                _playbackDevice = _core.Audio.PlaybackDevice;
                _playbackDevice.VolumeChanged += PlaybackDevice_VolumeChanged;
                sldVol.Value = _playbackDevice.Volume;
                sldVol.Enabled = true;
            }
            else
            {
                _playbackDevice = null;
                sldVol.Enabled = false;
            }
        }

        void RecordingDeviceChanged(IAudioDevice obj)
        {
            RefreshRecordingDevices();
        }

        void PlaybackDeviceChanged(IAudioDevice obj)
        {
            RefreshPlaybackDevices();
        }

        void RecordingDevice_MuteChanged(IAudioDevice obj)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action<IAudioDevice>(RecordingDevice_MuteChanged), obj);

                return;
            }

            btnMute.Checked = obj.Mute;
        }

        void RecordingDevice_VolumeChanged(IAudioDevice obj)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action<IAudioDevice>(RecordingDevice_VolumeChanged), obj);

                return;
            }

            sldMic.Value = obj.Volume;
        }

        void PlaybackDevice_VolumeChanged(IAudioDevice obj)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action<IAudioDevice>(PlaybackDevice_VolumeChanged), obj);

                return;
            }

            sldVol.Value = obj.Volume;
        }

        private void sldVol_MouseUp(object sender, MouseEventArgs e)
        {
            _playbackDevice.Volume = sldVol.Value;
        }

        private void sldMic_MouseUp(object sender, MouseEventArgs e)
        {
            _recordingDevice.Volume = sldMic.Value;
        }

        private void btnMute_Click(object sender, EventArgs e)
        {
            _recordingDevice.Mute = btnMute.Checked;
        }

        #endregion

        #region Plugin UI elements routine

        private readonly Dictionary<Guid, KryptonCommand> _uiElementsCommands = new Dictionary<Guid, KryptonCommand>();
        private readonly SharedFileMessageTransportHost _messageTransport;

        private void ReloadUIElementsFromPlugins()
        {
            foreach (var plugin in _core.PluginManager.Plugins)
            {
                plugin.Started += plugin_Started;
                plugin.Stopped += plugin_Stopped;

                if (plugin.IsStarted)
                    plugin_Started(plugin.GetInstance());
            }
        }

        private KryptonCommand CreateCommandForPluginUIElement(IPluginUIElement uiElement)
        {
            KryptonCommand command;

            if (uiElement is IPluginCheckedUIElement)
                command = new CheckedPluginUIElementCommand((IPluginCheckedUIElement)uiElement);
            else
                command = new PluginUIElementCommand(uiElement);

            _uiElementsCommands.Add(uiElement.Id, command);

            return command;
        }

        private void LoadPluginUIElements(IPlugin plugin)
        {
            if (plugin.UIElements == null)
                return;

            UnloadUIElements(plugin.UIElements);

            lock (_uiElementsCommands)
            {
                bool hasMenuItems = false;

                foreach (IPluginUIElement uiElement in plugin.UIElements)
                {
                    if (!_uiElementsCommands.ContainsKey(uiElement.Id))
                    {
                        KryptonCommand command = CreateCommandForPluginUIElement(uiElement);

                        try
                        {
                            if (uiElement.ShowInToolBar)
                                AddToolbarButtonForPluginUIElement(command);
                            if (uiElement.ShowInMenu)
                            {
                                AddMenuItemForPluginUIElement(command, uiElement.Childrens, !hasMenuItems);

                                hasMenuItems = true;
                            }
                        }
                        catch (Exception e)
                        {
                            Logger.LogWarn(e);
                        }
                    }
                }
            }
        }

        private void UnloadUIElements(IEnumerable<IPluginUIElement> uiElements)
        {
            if (uiElements == null)
                return;

            lock (_uiElementsCommands)
            {
                foreach (IPluginUIElement uiElement in uiElements)
                {
                    // Remove childrens first
                    UnloadUIElements(uiElement.Childrens);

                    // Now remove element
                    if (_uiElementsCommands.ContainsKey(uiElement.Id))
                    {
                        _uiElementsCommands[uiElement.Id].Dispose();

                        _uiElementsCommands.Remove(uiElement.Id);
                    }
                }
            }
        }

        private void RemoveSpacerDuplicates()
        {
            lock (_uiElementsCommands)
            {
                if (contextMenuMain.Items.Count > 0 && contextMenuMain.Items[0] is KryptonContextMenuItems)
                {
                    KryptonContextMenuItems rootItem;
                    rootItem = (KryptonContextMenuItems)contextMenuMain.Items[0];

                    bool isLastItemSeparator = false;
                    var itemsToDelete = new List<KryptonContextMenuSeparator>();
                    foreach (var item in rootItem.Items)
                    {
                        if (item is KryptonContextMenuSeparator)
                        {
                            if (isLastItemSeparator)
                                itemsToDelete.Add((KryptonContextMenuSeparator)item);

                            isLastItemSeparator = true;
                        }
                        else
                            isLastItemSeparator = false;
                    }

                    foreach (var item in itemsToDelete)
                        rootItem.Items.Remove(item);
                }
            }
        }

        private void AddMenuItemForPluginUIElement(KryptonCommand command, IEnumerable<IPluginUIElement> childrens)
        {
            AddMenuItemForPluginUIElement(command, childrens, false);
        }

        private void AddMenuItemForPluginUIElement(KryptonCommand command, IEnumerable<IPluginUIElement> childrens, bool isFirst)
        {
            KryptonContextMenuItems rootItem;

            if (contextMenuMain.Items.Count > 0 && contextMenuMain.Items[0] is KryptonContextMenuItems)
            {
                rootItem = (KryptonContextMenuItems)contextMenuMain.Items[0];

                AddMenuItemForPluginUIElement(command, rootItem, childrens, isFirst);
            }
            else
                throw new InvalidOperationException("Can't find menu root item for plugins");
        }

        private void AddMenuItemForPluginUIElement(KryptonCommand command, KryptonContextMenuItems rootItem, IEnumerable<IPluginUIElement> childrens, bool isFirst)
        {
            MainFormMenuItemControl mainFormMenuItem = new MainFormMenuItemControl(command, rootItem);

            // Add childrens
            KryptonContextMenuItems childrenItems = new KryptonContextMenuItems();

            if (childrens != null)
            {
                foreach (IPluginUIElement children in childrens)
                    AddMenuItemForPluginUIElement(CreateCommandForPluginUIElement(children), childrenItems, children.Childrens, false);

                if (childrenItems.Items.Count > 0)
                    mainFormMenuItem.Items.Add(childrenItems);
            }

            // Insert into "all items container" which is in 0 position
            if (isFirst)
                rootItem.Items.Insert(0, new KryptonContextMenuSeparator());

            rootItem.Items.Insert(0, mainFormMenuItem);
        }

        private void AddToolbarButtonForPluginUIElement(KryptonCommand command)
        {
            ToolStripUIElementButton button = new ToolStripUIElementButton(command)
            {
                ImageAlign = ContentAlignment.MiddleCenter,
                ImageScaling = ToolStripItemImageScaling.None,
                CheckOnClick = false,
                DoubleClickEnabled = false,
            };

            toolStripMain.Items.Add(button);
            toolStripMain.Refresh();
        }

        private void plugin_Started(object sender)
        {
            if (sender != null && sender is IPlugin)
            {
                LoadPluginUIElements((IPlugin)sender);
                RemoveSpacerDuplicates();
            }
        }

        private void plugin_Stopped(object sender, string message)
        {
            if (sender != null && sender is IPlugin)
            {
                UnloadUIElements(((IPlugin)sender).UIElements);
                RemoveSpacerDuplicates();
            }
        }

        #endregion

        #region IUi

        public void ActivateCall(ICall call)
        {
            foreach (var lineWrapper in _mainFormPhoneLineControls)
                if (lineWrapper != null && lineWrapper.Call == call)
                    ChangeLineWrapper(lineWrapper);
        }

        public string GetPhoneNumber()
        {
            if (InvokeRequired)
            {
                return (string)Invoke(new Func<string>(GetPhoneNumber));
            }
            return txtNumber.Text;
        }

        #endregion
    }
}
