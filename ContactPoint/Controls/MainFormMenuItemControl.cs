using System;
using ComponentFactory.Krypton.Toolkit;

namespace ContactPoint.Controls
{
    class MainFormMenuItemControl : KryptonContextMenuItem
    {
        private readonly KryptonCommand _command;

        public KryptonContextMenuItems Parent { get; }

        public override KryptonCommand KryptonCommand
        {
            get { return _command; }
            set { }
        }

        public MainFormMenuItemControl(KryptonCommand command, KryptonContextMenuItems parent)
        {
            Parent = parent;

            _command = command;
            _command.Disposed += CommandDisposed;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                Parent?.Items?.Remove(this);
                KryptonCommand.Disposed -= CommandDisposed;
            }

            base.Dispose(disposing);
        }

        private void CommandDisposed(object sender, EventArgs e)
        {
            Dispose(true);
        }
    }
}
