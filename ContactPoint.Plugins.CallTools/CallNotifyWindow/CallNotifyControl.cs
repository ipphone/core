using System;
using System.Drawing;
using ContactPoint.BaseDesign.BaseNotifyControls;
using ContactPoint.Common;

namespace ContactPoint.Plugins.CallTools.CallNotifyWindow
{
    public partial class CallNotifyControl : NotifyControl
    {
        private readonly bool _persistentWindow;
        private ICall _call;

        public CallNotifyControl(ICall call, bool persistentWindow)
        {
            InitializeComponent();

            _call = call;
            _call.OnRemoved += _call_OnRemoved;
            _call.OnStateChanged += _call_OnStateChanged;
            _call.OnInfoChanged += _call_OnInfoChanged;
            _call.OnDurationChanged += _call_OnDurationChanged;

            _persistentWindow = persistentWindow;

            labelColor.BackColor = Color.LightGray;

            RefreshUI();
        }

        private void DetachFromEventHandlers()
        {
            _call.OnRemoved -= _call_OnRemoved;
            _call.OnStateChanged -= _call_OnStateChanged;
            _call.OnInfoChanged -= _call_OnInfoChanged;
            _call.OnDurationChanged -= _call_OnDurationChanged;
        }

        void _call_OnDurationChanged()
        {
            if (InvokeRequired)
            {
                BeginInvoke(new EmptyDelegate(_call_OnDurationChanged));

                return;
            }

            lblTime.Text = _call.Duration.ToFormattedString();
        }

        void _call_OnInfoChanged()
        {
            if (InvokeRequired)
            {
                BeginInvoke(new EmptyDelegate(_call_OnInfoChanged));
                return;
            }

            RefreshUI();
        }

        void _call_OnStateChanged()
        {
            if (InvokeRequired)
            {
                BeginInvoke(new EmptyDelegate(_call_OnStateChanged));
                return;
            }

            if (_call.State == CallState.ACTIVE && ParentForm != null && !_persistentWindow)
            {
                Close();
            }
            else if (ParentForm != null)
            {
                RefreshUI();
            }
        }

        void _call_OnRemoved(CallRemoveReason reason)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action<CallRemoveReason>(_call_OnRemoved), reason);

                return;
            }

            if (Handle == IntPtr.Zero)
            {
                _call = null;
            }
            else
            {
                Close();
            }
        }

        void RefreshUI()
        {
            this.lblLine.Text = this._call.Line >= 0 ? (this._call.Line + 1).ToString() : "";
            this.lblName.Text = this._call.Name.Length > 0 ? this._call.Name : "-";
            this.lblNumber.Text = this._call.Number;
            this.tagsControl1.Contact = this._call.Contact;

            if (_call.Headers.Contains("x-info"))
                this.labelInfo.Text = _call.Headers.GetValueSafe("x-info");

            if (_call.Contact != null && !string.IsNullOrWhiteSpace(_call.Contact.ShowedName))
            {
                lblName.Text = _call.Contact.ShowedName;

                var queueDisplayName = _call.Headers.GetValueSafe("x-queue-display-name");
                if (!String.IsNullOrEmpty(queueDisplayName))
                    this.labelInfo.Text = Uri.UnescapeDataString(queueDisplayName);
            }
            else
            {
                var queueDisplayName = _call.Headers.GetValueSafe("x-queue-display-name");
                if (!String.IsNullOrEmpty(queueDisplayName))
                    this.lblName.Text = Uri.UnescapeDataString(queueDisplayName);
            }

            if (_call.Headers.Contains("x-color"))
            {
                try
                {
                    labelColor.BackColor = Color.FromName(_call.Headers["x-color"].Value);
                }
                catch (Exception e)
                {
                    Logger.LogWarn(e, "Header 'x-color' was found but color can't be parsed. Color value is invalid or null.");
                }
            }
        }

        private void btnCall_Click(object sender, EventArgs e)
        {
            _call.Answer();

            if (!_persistentWindow)
            {
                Close();
            }
        }

        private void btnDrop_Click(object sender, EventArgs e)
        {
            _call.Drop();

            Close();
        }

        public override void OnShow()
        {
            if (_call == null || _call.IsDisposed) { Close(); }

            base.OnShow();
        }

        public override void OnClosing()
        {
            if (_call != null)
            {
                DetachFromEventHandlers();
            }
        }
    }
}
