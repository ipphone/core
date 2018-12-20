using System;
using System.Windows.Forms;
using System.Drawing;

namespace ContactPoint.BaseDesign.Components
{
    public class UIToolStripButton : ToolStripButton
    {
        private UIButton _myButton = new UIButton();

        public UIToolStripButton()
            : base()
        { }

        public override Image Image
        {
            get
            {
                return base.Image;
            }
            set
            {
                base.Image = value;
                _myButton.Values.Image = value;
            }
        }

        public override Size Size
        {
            get
            {
                return base.Size;
            }
            set
            {
                base.Size = value;
                _myButton.Size = value;
            }
        }

        public override string Text
        {
            get
            {
                return base.Text;
            }
            set
            {
                base.Text = value;
                _myButton.Text = value;
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            _myButton.PaintMe(e.Graphics);
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            _myButton.MouseEnterMe(e);
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            _myButton.MouseLeaveMe(e);
        }
    }
}
