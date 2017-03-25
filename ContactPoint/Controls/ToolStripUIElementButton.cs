using System;
using System.Windows.Forms;
using ComponentFactory.Krypton.Toolkit;

namespace ContactPoint.Controls
{
    class ToolStripUIElementButton : ToolStripButton
    {
        private readonly KryptonCommand _command;

        public ToolStripUIElementButton(KryptonCommand command)
        {
            _command = command;

            _command.PropertyChanged += OnPropertyChanged;
            _command.Execute += OnExecute;
            _command.Disposed += OnDisposed;

            UpdateUI();
        }

        protected override void OnOwnerChanged(EventArgs e)
        {
            UpdateUI();

            base.OnOwnerChanged(e);
        }

        protected override void Dispose(bool disposing)
        {
            _command.PropertyChanged -= OnPropertyChanged;
            _command.Execute -= OnExecute;
            _command.Disposed -= OnDisposed;

            base.Dispose(disposing);
        }

        protected override void OnClick(EventArgs e)
        {
            base.OnClick(e);

            _command.PerformExecute();
        }

        protected void UpdateUI()
        {
            if (Owner == null)
            {
                return;
            }

            if (Owner.InvokeRequired)
            {
                Owner.Invoke(new Action(UpdateUI));
                return;
            }

            Image = _command.ImageSmall;
            ToolTipText = _command.Text;
            Checked = _command.Checked;
            Enabled = _command.Enabled;
        }

        void OnPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            UpdateUI();
        }

        void OnExecute(object sender, EventArgs e)
        {
            UpdateUI();
        }

        void OnDisposed(object sender, EventArgs e)
        {
            Dispose(true);
        }
    }
}
