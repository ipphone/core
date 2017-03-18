using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Globalization;
using System.Text;
using System.Windows.Forms;
using ContactPoint.Common;
using ContactPoint.BaseDesign.BaseNotifyControls;

namespace ContactPoint.NotifyControls
{
    public partial class CallNotifyControl : NotifyControl
    {
        private ICall _call;

        public ICall Call
        {
            get { return this._call; }
        }

        public CallNotifyControl(ICall call)
        {
            InitializeComponent();

            lock (call)
            {
                _call = call;

                _call.OnRemoved += new Action<CallRemoveReason>(_call_OnRemoved);
                _call.OnStateChanged += new EmptyDelegate(_call_OnStateChanged);
                _call.OnInfoChanged += new EmptyDelegate(_call_OnInfoChanged);
                _call.OnDurationChanged += new EmptyDelegate(_call_OnDurationChanged);
            }

            RefreshUI();
        }

        void _call_OnDurationChanged()
        {
            if (InvokeRequired)
            {
                BeginInvoke(new EmptyDelegate(_call_OnDurationChanged));

                return;
            }

            this.lblTime.Text = _call.Duration.ToFormattedString();
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

            if (this._call.State == CallState.ACTIVE && ParentForm != null)
                Close();
        }

        void _call_OnRemoved(CallRemoveReason reason)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action<CallRemoveReason>(_call_OnRemoved), reason);

                return;
            }

            if (Handle == IntPtr.Zero) _call = null;
            else Close();
        }

        void RefreshUI()
        {
            this.lblLine.Text = this._call.Line >= 0 ? (this._call.Line + 1).ToString() : "";
            this.lblName.Text = this._call.Name.Length > 0 ? this._call.Name : "-";
            this.lblNumber.Text = this._call.Number;
        }

        private void btnCall_Click(object sender, EventArgs e)
        {
            _call.Answer();

            Close();
        }

        private void btnDrop_Click(object sender, EventArgs e)
        {
            _call.Drop();

            Close();
        }

        public override void OnShow()
        {
            if (_call == null || _call.IsDisposed) Close();

            base.OnShow();
        }

        public override void OnClosing()
        {
            if (this._call != null)
            {
                this._call.OnRemoved -= _call_OnRemoved;
                this._call.OnStateChanged -= _call_OnStateChanged;
                this._call.OnInfoChanged -= _call_OnInfoChanged;
                this._call.OnDurationChanged -= _call_OnDurationChanged;
            }
        }
    }
}
