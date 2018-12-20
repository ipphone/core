using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using ComponentFactory.Krypton.Toolkit;
using ContactPoint.Common;

namespace ContactPoint.Controls
{
    class MainFormPhoneLineControl
    {
        private readonly ICore _core;
        private readonly EmptyDelegate _callOnStateChanged;
        private readonly EmptyDelegate _callOnInfoChanged;
        private readonly EmptyDelegate _callOnDurationChanged;
        private readonly Action<CallRemoveReason> _callOnRemoved;
        private readonly Control _controlCallerName;
        private readonly Control _controlCallerNumber;
        private readonly Control _controlDuration;
        private readonly Control _controlCallState;
        private readonly KryptonCheckButton _holdButton;
        private ICall _call;
        private bool _active = false;

        public int Line { get; } = -1;
        public KryptonCheckButton ControlButton { get; }

        public ICall Call
        {
            get { return _call; }
            set {
                Trace.TraceInformation("Set call with sessionId '" + value?.SessionId + "' on MainFormPhoneLineControl" + ControlButton.Text);

                if (_call != null)
                {
                    _call.OnStateChanged -= _callOnStateChanged;
                    _call.OnRemoved -= _callOnRemoved;
                    _call.OnInfoChanged -= _callOnInfoChanged;
                    _call.OnDurationChanged -= _callOnDurationChanged;
                    _call = null;
                }

                if (value != null && !value.IsDisposed)
                {
                    _call = value;
                    _call.OnStateChanged += _callOnStateChanged;
                    _call.OnRemoved += _callOnRemoved;
                    _call.OnInfoChanged += _callOnInfoChanged;
                    _call.OnDurationChanged += _callOnDurationChanged;
                }

                if (_active)
                {
                    _core.CallManager.ActiveCall = _call;

                    RefreshUI();
                }

                RefreshBackUI();
            }
        }

        public bool Active
        {
            get { return _active; }
            set {
                Trace.TraceInformation("Activate MainFormPhoneLineControl" + ControlButton.Text);

                _active = value;

                if (_active)
                    _core.CallManager.ActiveCall = _call;
                else
                    if (_call != null && !_call.IsInConference)
                        _call.Hold();

                RefreshUI();
                RefreshBackUI();
            }
        }

        public MainFormPhoneLineControl(
            ICore core,
            int line,
            KryptonCheckButton controlButton, 
            Control controlCallerName, 
            Control controlCallerNumber,
            Control controlDuration, 
            Control controlCallState,
            KryptonCheckButton holdButton
            )
        {
            _callOnDurationChanged = Call_OnDurationChanged;
            _callOnInfoChanged = Call_OnInfoChanged;
            _callOnStateChanged = Call_OnStateChanged;
            _callOnRemoved = Call_OnRemoved;

            _core = core;
            Line = line;
            ControlButton = controlButton;
            _controlCallerName = controlCallerName;
            _controlCallerNumber = controlCallerNumber;
            _controlDuration = controlDuration;
            _controlCallState = controlCallState;
            _holdButton = holdButton;

            _controlCallerNumber.TextChanged += _controlCallerNumber_TextChanged;

            RefreshBackUI();
        }

        void Call_OnDurationChanged()
        {
            if (!_active || _call == null)
            {
                return;
            }

            if (ControlButton.InvokeRequired)
            {
                ControlButton.BeginInvoke(_callOnDurationChanged);

                return;
            }

            if (_call.ActiveDuration > TimeSpan.Zero)
            {
                _controlDuration.Text = _call.ActiveDuration.ToFormattedString();
            }
            else if (_call.Duration > TimeSpan.Zero)
            {
                _controlDuration.Text = _call.Duration.ToFormattedString();
            }
        }

        void Call_OnStateChanged()
        {
            if (ControlButton.InvokeRequired)
            {
                ControlButton.BeginInvoke(_callOnStateChanged);

                return;
            }

            Trace.TraceInformation("New call state on MainFormPhoneLineControl" + ControlButton.Text + ", new state: " + SipHelper.SipCallStateDecode(_call?.State ?? CallState.NULL));

            if (_active)
            {
                RefreshUI();
            }

            RefreshBackUI();
        }

        void Call_OnInfoChanged()
        {
            if (ControlButton.InvokeRequired)
            {
                ControlButton.BeginInvoke(_callOnInfoChanged);

                return;
            }

            if (_active)
            {
                RefreshUI();
            }

            RefreshBackUI();
        }

        void Call_OnRemoved(CallRemoveReason reason)
        {
            if (ControlButton.InvokeRequired)
            {
                ControlButton.BeginInvoke(_callOnRemoved, reason);

                return;
            }

            if (Active && Call != null && Call.Number == _controlCallerNumber.Text)
            {
                _controlCallerNumber.Text = "";
            }

            Call = null;
            
            RefreshBackUI();
        }

        void _controlCallerNumber_TextChanged(object sender, EventArgs e)
        {
            if (_callerNumberTrackChanges)
                _callerNumberChanged = true;
        }

        private bool _callerNumberChanged = false;
        private readonly bool _callerNumberTrackChanges = true;
        void RefreshUI()
        {
            if (Active)
            {
                ControlButton.Checked = true;
                ControlButton.StateCommon.Content.ShortText.Font = new Font(ControlButton.StateCommon.Content.ShortText.Font, FontStyle.Bold);
            }
            else
            {
                ControlButton.Checked = false;
                ControlButton.StateCommon.Content.ShortText.Font = new Font(ControlButton.StateCommon.Content.ShortText.Font, FontStyle.Regular);
            }
            
            if (_call != null && Active)
            {
                _controlCallState.Text = SipHelper.SipCallStateDecode(_call.State);
                _controlCallerName.Text = _call.Name;

                if (_call.Contact != null && !string.IsNullOrWhiteSpace(_call.Contact.ShowedName))
                    _controlCallerName.Text = _call.Contact.ShowedName;

                if (string.IsNullOrEmpty(_controlCallerNumber.Text))
                {
                    _controlCallerNumber.Text = _call.Number;

                    _callerNumberChanged = false;
                }

                if (_call.ActiveDuration > TimeSpan.Zero)
                {
                    _controlDuration.Text = _call.ActiveDuration.ToFormattedString();
                }
                else if (_call.Duration > TimeSpan.Zero)
                {
                    _controlDuration.Text = _call.Duration.ToFormattedString();
                }

                _holdButton.Checked = _call.State == CallState.HOLDING;

                // Change color if it was provided via headers
                if (_call.Headers.Contains("x-color"))
                {
                    try
                    {
                        _controlCallerName.BackColor = Color.FromName(_call.Headers["x-color"].Value);
                    }
                    catch (Exception e)
                    {
                        Logger.LogWarn(e, "Header 'x-color' was found but color can't be parsed. Color value is invalid or null.");
                    }
                }
                else
                    _controlCallerName.BackColor = Color.DimGray;
            }
            else
            {
                _controlCallState.Text = string.Empty;
                _controlCallerName.Text = "-";
                _controlDuration.Text = string.Empty;
                _holdButton.Checked = false;
                _controlCallerName.BackColor = Color.DimGray;

                if (!_callerNumberChanged)
                {
                    _controlCallerNumber.Text = string.Empty;
                }
            }

            _controlCallerName.Visible = !string.IsNullOrWhiteSpace(_controlCallerName.Text) && !string.Equals(_controlCallerName.Text, "-") && !string.Equals(_controlCallerName.Text, "---");
        }

        static readonly Color DefaultInactiveColor = Color.FromArgb(135, 135, 135);
        static readonly Color DefaultSelectedColor = Color.FromArgb(215, 215, 215);
        static readonly Color DefaultActiveColor = Color.Green;
        static readonly Color DefaultIncomingColor = Color.Red;
        static readonly Color DefaultOutgoingColor = Color.SkyBlue;
        static readonly Color DefaultHoldingColor = Color.Orange;
        static readonly Color DefaultConferenceColor = Color.HotPink;

        void RefreshBackUI()
        {
            if (_call != null)
            {
                switch (_call.State)
                {
                    case CallState.ACTIVE:
                        SetColorOnControlButton(_call.IsInConference ? DefaultConferenceColor : DefaultActiveColor); return;
                    case CallState.CONNECTING:
                    case CallState.ALERTING:
                        SetColorOnControlButton(DefaultOutgoingColor); return;
                    case CallState.INCOMING:
                        SetColorOnControlButton(DefaultIncomingColor); return;
                    case CallState.HOLDING:
                        SetColorOnControlButton(DefaultHoldingColor); return;
                    case CallState.IDLE:
                    case CallState.NULL:
                    case CallState.TERMINATED:
                    case CallState.RELEASED: break;
                }
            } 

            SetColorOnControlButton(Active ? DefaultSelectedColor : DefaultInactiveColor);
        }

        void SetColorOnControlButton(Color color)
        {
            var lightColor = ControlPaint.LightLight(color);

            ControlButton.OverrideDefault.Back.Color2 = color;
            ControlButton.OverrideDefault.Back.Color1 = lightColor;

            ControlButton.OverrideFocus.Back.Color2 = color;
            ControlButton.OverrideFocus.Back.Color1 = lightColor;

            ControlButton.StateCommon.Back.Color2 = color;
            ControlButton.StateCommon.Back.Color1 = lightColor;

            ControlButton.StateNormal.Back.Color2 = color;
            ControlButton.StateNormal.Back.Color1 = lightColor;

            ControlButton.StatePressed.Back.Color2 = color;
            ControlButton.StatePressed.Back.Color1 = lightColor;

            ControlButton.StateTracking.Back.Color2 = color;
            ControlButton.StateTracking.Back.Color1 = lightColor;

            ControlButton.StateCheckedNormal.Back.Color2 = color;
            ControlButton.StateCheckedNormal.Back.Color1 = lightColor;

            ControlButton.StateCheckedPressed.Back.Color2 = color;
            ControlButton.StateCheckedPressed.Back.Color1 = lightColor;

            ControlButton.StateCheckedTracking.Back.Color2 = color;
            ControlButton.StateCheckedTracking.Back.Color1 = lightColor;

            ControlButton.Update();
        }
    }
}
