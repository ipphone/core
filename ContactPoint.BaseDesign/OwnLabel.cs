using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace ContactPoint.BaseDesign
{
    public class OwnLabel : Control
    {
        [Browsable(true)]
        public override string Text
        {
            get
            {
                return base.Text;
            }
            set
            {
                base.Text = value;

                this.Visible = false;

                if (this.Parent != null)
                {
                    this.Parent.Invalidate(this.Bounds);
                    this.Parent.Update();
                }
                
                this.Visible = true;
            }
        }

        public OwnLabel()
        {
        }

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x20;
                return cp;
            }
        }

        /// <summary>
        /// Paints the background.
        /// </summary>
        /// <param name="e">E.</param>
        protected override void OnPaintBackground(PaintEventArgs e)
        {
            // do nothing
        }

        /// <summary>
        /// Paints the control.
        /// </summary>
        /// <param name="e">E.</param>
        protected override void OnPaint(PaintEventArgs e)
        {
            using (SolidBrush brush = new SolidBrush(ForeColor))
            {
                e.Graphics.DrawString(Text, Font, brush, -1, 0);
            }
        }
    }
}
