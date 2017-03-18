using System.ComponentModel;
using System.Threading;
using System.Windows.Forms;

namespace ContactPoint.BaseDesign.BaseNotifyControls
{
    public delegate void Action();

    [ToolboxItem(false)]
    public class NotifyControl : UserControl
    {
        private readonly ManualResetEvent _formAssignedEvent = new ManualResetEvent(false);
        public NotifyForm NotifyForm { get; set; }

        public NotifyControl()
        { }

        public void Close()
        {
            if (InvokeRequired)
            {
                Invoke(new Action(Close));

                return;
            }

            if (_formAssignedEvent.WaitOne(2000))
                NotifyForm.QueryClose();
        }

        public virtual void OnShow()
        { }

        public virtual void OnClosing()
        { }

        internal void NotifyOnCreate()
        {
            _formAssignedEvent.Set();
        }
    }
}
