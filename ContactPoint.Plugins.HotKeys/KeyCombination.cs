using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using ContactPoint.Common;
using ContactPoint.Plugins.HotKeys.Actions;

namespace ContactPoint.Plugins.HotKeys
{
    internal class KeyCombination : Form, IService
    {
        private const byte ModAlt = 1, ModControl = 2, ModShift = 4, ModWin = 8;

        [DllImport("user32.dll")]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, int fsModifiers, int vlc);
        [DllImport("user32.dll")]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        private ICore _core;

        public KeyCombination(UserAction action, ICore core)
        {
            _core = core;
            Action = action;
        }

        public UserAction Action { get; private set; }

        private int _combination;
        public int Combination 
        {
            get { return _combination; }
            set
            {
                _combination = value;

                if (IsStarted)
                {
                    Stop();
                    Start();
                }
            }
        }

        public int Win32Modifiers
        {
            get
            {
                byte total = 0;

                var k = (Keys)Combination;
                if (((int)k & (int)Keys.Shift) == (int)Keys.Shift)
                    total += ModShift;
                if (((int)k & (int)Keys.Control) == (int)Keys.Control)
                    total += ModControl;
                if (((int)k & (int)Keys.Alt) == (int)Keys.Alt)
                    total += ModAlt;
                if (((int)k & (int)Keys.LWin) == (int)Keys.LWin)
                    total += ModWin;

                return total;
            }
        }

        public int CharCode
        {
            get
            {
                var k = (Keys)Combination;
                if (((int) k & (int) Keys.Shift) == (int) Keys.Shift)
                    k ^= Keys.Shift;
                if (((int) k & (int) Keys.Control) == (int) Keys.Control)
                    k ^= Keys.Control;
                if (((int) k & (int) Keys.Alt) == (int) Keys.Alt)
                    k ^= Keys.Alt;
                if (((int) k & (int) Keys.LWin) == (int) Keys.LWin)
                    k ^= Keys.LWin;

                return (int)k;
            }
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == 0x0312)
                Action.Command.Invoke(_core);

            base.WndProc(ref m);
        }

        #region IService Members

        public event ServiceStartedDelegate Started;

        public event ServiceStoppedDelegate Stopped;

        public void Start()
        {
            if (IsStarted) return;

            Logger.LogNotice("Starting HotKeyCombination " + Action.Name);

            try
            {
                IsStarted = RegisterHotKey(Handle, GetType().GetHashCode(), Win32Modifiers, CharCode);

                if (IsStarted && Started != null)
                    Started(this);
            }
            catch (Exception e)
            {
                Logger.LogWarn(e);
            }
       }

        public void Stop()
        {
            if (!IsStarted) return;

            Logger.LogNotice("Stopping HotKeyCombination " + Action.Name);

            try
            {
                UnregisterHotKey(Handle, GetType().GetHashCode());

                IsStarted = false;

                if (!IsStarted && Stopped != null)
                    Stopped(this, "Normal stop");
            }
            catch (Exception e)
            {
                Logger.LogWarn(e);
            }
        }

        public bool IsStarted
        {
            get;
            private set;
        }

        #endregion
    }
}
