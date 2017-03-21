using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using ContactPoint.BaseDesign.BaseNotifyControls;

namespace ContactPoint.BaseDesign
{
    public class NotifyManager
    {
        private static NotifyManager _instance;

        List<NotifyForm> _items = new List<NotifyForm>();
        Form _mainForm;

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

        private delegate void NotifyDelegate(NotifyControl control, int timeout);
        public void Notify(NotifyControl control, int timeout)
        {
            if (_mainForm.InvokeRequired)
            {
                _mainForm.BeginInvoke(new NotifyDelegate(Notify), control, timeout);

                return;
            }

            var frm = new NotifyForm(control) { NotifyControl = control, Timeout = timeout };
            control.NotifyForm = frm;

            frm.SetPosition(GetNextPosition(frm.Width, frm.Height));

            this._items.Add(frm);

            frm.FormClosed += new FormClosedEventHandler(frm_FormClosed);

            control.NotifyOnCreate();

            frm.ShowGracefully();
        }

        private Point GetNextPosition(int frmWidth, int frmHeight)
        {
            Rectangle rect = Screen.GetWorkingArea(this._mainForm);

            for (int i = 1; i <= this._items.Count; i++)
            {
                Point checkPoint = new Point(
                    rect.Width - frmWidth - 10,
                    rect.Height - (frmHeight + 10) * i
                    );

                if (CheckPosition(checkPoint))
                    return checkPoint;
            }

            return new Point(
                    rect.Width - frmWidth - 10,
                    rect.Height - (frmHeight + 10) * (this._items.Count + 1)
                    );
        }

        private bool CheckPosition(Point point)
        {
            foreach (NotifyForm form in this._items)
                if (form.Location.Y == point.Y)
                    return false;

            return true;
        }

        void frm_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (sender != null)
                this._items.Remove(sender as NotifyForm);
        }
    }
}
