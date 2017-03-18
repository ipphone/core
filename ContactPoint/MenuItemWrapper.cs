using System;
using System.Collections.Generic;
using System.Text;
using ComponentFactory.Krypton.Toolkit;

namespace ContactPoint
{
    internal class MenuItemWrapper : KryptonContextMenuItem
    {
        private KryptonContextMenuItems _parent;

        public KryptonContextMenuItems Parent
        {
            get { return this._parent; }
        }

        public MenuItemWrapper(KryptonCommand command, KryptonContextMenuItems parent)
            : base()
        {
            this._parent = parent;

            this.KryptonCommand = command;

            this.KryptonCommand.Disposed += new EventHandler(KryptonCommand_Disposed);
        }

        protected override void Dispose(bool disposing)
        {
            this.KryptonCommand.Disposed -= KryptonCommand_Disposed;

            base.Dispose(disposing);
        }

        void KryptonCommand_Disposed(object sender, EventArgs e)
        {
            this.Parent.Items.Remove(this);

            this.Dispose();
        }
    }
}
