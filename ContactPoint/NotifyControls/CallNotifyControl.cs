using System;
using ContactPoint.Common;
using ContactPoint.BaseDesign.BaseNotifyControls;

namespace ContactPoint.NotifyControls
{
    public partial class CallNotifyControl : NotifyControl
    {
        public ICall Call { get; private set; }

        public CallNotifyControl(ICall call)
        {
            InitializeComponent();

            Call = call;

            Call.OnRemoved += new Action<CallRemoveReason>(_call_OnRemoved);
            Call.OnStateChanged += new EmptyDelegate(_call_OnStateChanged);
            Call.OnInfoChanged += new EmptyDelegate(_call_OnInfoChanged);
            Call.OnDurationChanged += new EmptyDelegate(_call_OnDurationChanged);

            RefreshUI();
        }

        void _call_OnDurationChanged()
        {
            if (InvokeRequired)
            {
                BeginInvoke(new EmptyDelegate(_call_OnDurationChanged));

                return;
            }

            this.lblTime.Text = Call.Duration.ToFormattedString();
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

            if (this.Call.State == CallState.ACTIVE && ParentForm != null)
                Close();
        }

        void _call_OnRemoved(CallRemoveReason reason)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action<CallRemoveReason>(_call_OnRemoved), reason);

                return;
            }

            if (Handle == IntPtr.Zero) Call = null;
            else Close();
        }

        void RefreshUI()
        {
            this.lblLine.Text = this.Call.Line >= 0 ? (this.Call.Line + 1).ToString() : "";
            this.lblName.Text = this.Call.Name.Length > 0 ? this.Call.Name : "-";
            this.lblNumber.Text = this.Call.Number;
        }

        private void btnCall_Click(object sender, EventArgs e)
        {
            Call.Answer();

            Close();
        }

        private void btnDrop_Click(object sender, EventArgs e)
        {
            Call.Drop();

            Close();
        }

        public override void OnShow()
        {
            if (Call == null || Call.IsDisposed) Close();

            base.OnShow();
        }

        public override void OnClosing()
        {
            if (this.Call != null)
            {
                this.Call.OnRemoved -= _call_OnRemoved;
                this.Call.OnStateChanged -= _call_OnStateChanged;
                this.Call.OnInfoChanged -= _call_OnInfoChanged;
                this.Call.OnDurationChanged -= _call_OnDurationChanged;
            }
        }
    }
}
