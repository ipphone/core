using System;
using System.Windows.Forms;
using ContactPoint.Commands;
using ContactPoint.Common;
using ContactPoint.Common.SIP.Account;
using ContactPoint.Forms;

namespace ContactPoint.Controls
{
    public partial class MainFormPhoneStatusControl : UserControl
    {
        private ICore _core;
        private readonly Timer _timer1Second = new Timer { Interval = 1000 };

        public ICore Core
        {
            get { return _core; }
            set
            {
                if (_core != null)
                {
                    _core.Sip.Account.RegisterStateChanged -= OnAccountStateChanged;
                    _core.Sip.Account.PresenceStatusChanged -= OnAccountStateChanged;
                    _core.Audio.PlaybackDeviceChanged -= OnDeviceChanged;
                    _core.Audio.RecordingDeviceChanged -= OnDeviceChanged;
                    _core.CallManager.OnCallStateChanged -= OnCallStateChanged;
                    _core.CallManager.OnCallRemoved -= OnCallStateRemoved;
                }

                _core = value;

                if (_core != null)
                {
                    _core.Sip.Account.RegisterStateChanged += OnAccountStateChanged;
                    _core.Sip.Account.PresenceStatusChanged += OnAccountStateChanged;
                    _core.Audio.PlaybackDeviceChanged += OnDeviceChanged;
                    _core.Audio.RecordingDeviceChanged += OnDeviceChanged;
                    _core.CallManager.OnCallStateChanged += OnCallStateChanged;
                    _core.CallManager.OnCallRemoved += OnCallStateRemoved;
                }
            }
        }

        public MainFormPhoneStatusControl()
        {
            InitializeComponent();

            _timer1Second.Tick += Timer1SecondTick;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            _timer1Second.Enabled = true;
        }

        private void OnCallStateRemoved(ICall call, CallRemoveReason reason)
        {
            if (InvokeRequired) { Invoke(new Action<ICall, CallRemoveReason>(OnCallStateRemoved), call, reason); return; }

            RefreshStatus();
        }

        private void OnCallStateChanged(ICall call)
        {
            if (InvokeRequired) { Invoke(new Action<ICall>(OnCallStateChanged), call); return; }

            RefreshStatus();
        }

        void OnAccountStateChanged(ISipAccount account)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<ISipAccount>(OnAccountStateChanged), account);
                return;
            }

            RefreshStatus();
        }

        void OnDeviceChanged(Common.Audio.IAudioDevice obj)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<Common.Audio.IAudioDevice>(OnDeviceChanged), obj);
                return;
            }

            RefreshStatus();
        }

        void RefreshStatus()
        {
            var stateViewModel = SipAccountStatusViewModel.Create(Core);

            labelStatus.ForeColor = stateViewModel.Color;
            labelStatus.Text = $"{stateViewModel.Text} {stateViewModel.TimeElapsed}";
            pictureBoxStatus.Image = stateViewModel.Image;
        }

        private void labelStatus_Click(object sender, EventArgs e)
        {
            new LoggerForm().Show();
        }

        private void Timer1SecondTick(object sender, EventArgs eventArgs)
        {
            RefreshStatus();
        }
    }
}
