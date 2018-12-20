using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Security.Permissions;

namespace ContactPoint.BaseDesign.BaseNotifyControls
{
    public partial class NotifyForm : Form
    {
        private const int SW_SHOWNOACTIVATE = 4;
        private const int HWND_TOPMOST = -1;
        private const uint SWP_NOACTIVATE = 0x0010;

        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        [DllImport("user32.dll", EntryPoint = "SetWindowPos")]
        static extern bool SetWindowPos(
             int hWnd,              // window handle
             int hWndInsertAfter,   // placement-order handle
             int X,                 // horizontal position
             int Y,                 // vertical position
             int cx,                // width
             int cy,                // height
             uint uFlags);

        private readonly object _lockObj = new object();
        private readonly MouseMessageFilter _messageFilter;
        private readonly Timer _closeTimer = new Timer();
        private readonly Timer _showTimer = new Timer();
        private int _timeout = 0;
        private bool _loaded = false;
        private bool _closeOnShow = false;

        public int Timeout
        {
            get { return _timeout; }
            set 
            { 
                _timeout = value;

                if (_timeout > 0)
                    _closeTimer.Interval = _timeout;
            }
        }

        public NotifyControl NotifyControl { get; set; }

        protected override bool ShowWithoutActivation
        {
            get { return true; }
        }

        public NotifyForm(NotifyControl notifyControl)
        {
            _messageFilter = new MouseMessageFilter(this);

            NotifyControl = notifyControl;

            InitializeComponent();

            notifyControl.Dock = DockStyle.Fill;
            Controls.Add(notifyControl);

            _closeTimer.Tick += _closeTimer_Tick;
        }

        public void QueryClose()
        {
            lock (_lockObj)
            {
                if (_loaded) CloseInternal();
                else _closeOnShow = true;
            }
        }

        protected override void OnMouseHover(EventArgs e)
        {
            base.OnMouseHover(e);

            _closeTimer.Stop();
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);

            if (Timeout > 0)
                _closeTimer.Start();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);

            _closeTimer.Tick -= _closeTimer_Tick;

            NotifyControl.OnClosing();

            Application.RemoveMessageFilter(_messageFilter);
        }

        private void CloseInternal()
        {
            BeginInvoke(new Action(Close));
        }

        void _closeTimer_Tick(object sender, EventArgs e)
        {
            CloseInternal();
        }

        #region Resizing blockers

        private Size _oldSize;
        private Point _oldPosition;
        protected override void OnResizeBegin(EventArgs e)
        {
            _oldSize = Size;
            _oldPosition = Location;
        }

        protected override void OnResizeEnd(EventArgs e)
        {
            Size = _oldSize;
            Location = _oldPosition;
        }

        protected override void OnResize(EventArgs e)
        { }

        protected override void OnSizeChanged(EventArgs e)
        { }

        #endregion

        #region Mouse catcher tools

        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
        private class MouseMessageFilter : IMessageFilter
        {
            private static int WM_MOUSEHOVER = 0x2A1;
            private static int WM_MOUSELEAVE = 0x2A3;

            NotifyForm _notifyForm;

            public MouseMessageFilter(NotifyForm form)
            {
                _notifyForm = form;
            }

            public bool PreFilterMessage(ref Message m)
            {
                // handle the wm_mousehover message
                if (m.Msg == WM_MOUSEHOVER)
                {
                    _notifyForm.OnMouseHover(null);
                }
                else if (m.Msg == WM_MOUSELEAVE)
                {
                    _notifyForm.OnMouseLeave(null);
                }
                return false;
            }
        }

        #endregion

        #region UI routine

        private const int WS_EX_NOACTIVATE = 0x08000000;
        private const int WS_EX_TOPMOST = 0x00000008;

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams p = base.CreateParams;
                p.ExStyle |= (WS_EX_NOACTIVATE | WS_EX_TOPMOST);
                p.Parent = IntPtr.Zero;

                return p;
            }
        }

        void _showTimer_Tick(object sender, EventArgs e)
        {
            ShowTimerTick();
        }

        private delegate void ShowTimerTickDelegate();
        void ShowTimerTick()
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new ShowTimerTickDelegate(ShowTimerTick));
                return;
            }

            if (this.Opacity < 1)
            {
                this.Opacity += 0.1;
            }
            else
            {
                this.Opacity = 1;
                this._showTimer.Tick -= _showTimer_Tick;
                this._showTimer.Stop();
                this._showTimer.Dispose();

                if (Timeout > 0) _closeTimer.Start();
            }
        }

        internal void SetPosition(Point position)
        {
            SetWindowPos(Handle.ToInt32(), HWND_TOPMOST,
                position.X, position.Y, Width, Height,
                SWP_NOACTIVATE);
        }

        internal void ShowGracefully()
        {
            lock (_lockObj)
            {
                Opacity = 0;
                ShowWindow(Handle, SW_SHOWNOACTIVATE);

                _showTimer.Interval = 30;
                _showTimer.Tick += new EventHandler(_showTimer_Tick);
                _showTimer.Start();

                Application.AddMessageFilter(_messageFilter);
                NotifyControl.OnShow();

                _loaded = true;

                if (_closeOnShow) CloseInternal();
            }
        }

        #endregion
    }
}
