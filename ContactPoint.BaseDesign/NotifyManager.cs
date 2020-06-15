using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using ContactPoint.BaseDesign.BaseNotifyControls;

namespace ContactPoint.BaseDesign
{
    public class NotifyManager
    {
        private static NotifyManager _instance;

        private readonly List<NotifyForm> _notifyForms = new List<NotifyForm>();
        private readonly Form _mainForm;

        private NotifyManager(Form mainForm)
        {
            _instance = this;
            _mainForm = mainForm;
        }

        public static void Initialize(Form mainForm)
        {
            _instance = new NotifyManager(mainForm);
        }

        public static void NotifyUser(NotifyControl control)
        {
            NotifyUser(control, 5000);
        }

        public static void NotifyUser(NotifyControl control, int timeout)
        {
            if (_instance == null) return;

            _instance.Notify(control, timeout);
        }

        public void Notify(NotifyControl control)
        {
            Notify(control, 5000);
        }

        public void Notify(NotifyControl control, int timeout)
        {
            if (_mainForm.InvokeRequired)
            {
                _mainForm.BeginInvoke(new Action<NotifyControl, int>(Notify), control, timeout);
                return;
            }

            var notifyForm = new NotifyForm(control) 
            { 
                NotifyControl = control, 
                Timeout = timeout
            };

            notifyForm.SetPosition(GetNextPosition(notifyForm.Width, notifyForm.Height));
            lock (_notifyForms)
            {
                _notifyForms.Add(notifyForm);
            }

            notifyForm.FormClosed += NotifyFormClosed;

            control.NotifyForm = notifyForm;
            control.NotifyOnCreate();

            notifyForm.ShowGracefully();
        }

        private Point GetNextPosition(int frmWidth, int frmHeight)
        {
            var rect = Screen.GetWorkingArea(_mainForm);

            lock (_notifyForms)
            {
                for (int i = 1; i <= _notifyForms.Count; i++)
                {
                    var position = new Point(rect.Width - frmWidth - 10, rect.Height - (frmHeight + 10) * i);
                    if (_notifyForms.All(form => form.Location.Y != position.Y))
                    {
                        return position;
                    }
                }
            }

            return new Point(rect.Width - frmWidth - 10, rect.Height - (frmHeight + 10) * (_notifyForms.Count + 1));
        }

        private void NotifyFormClosed(object sender, FormClosedEventArgs e)
        {
            if (sender is NotifyForm form)
            {
                form.FormClosed -= NotifyFormClosed;

                lock (_notifyForms)
                {
                    _notifyForms.Remove(form);
                }
            }
        }
    }
}
