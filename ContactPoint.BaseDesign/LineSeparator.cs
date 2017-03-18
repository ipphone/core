using System.Drawing;
using System.Windows.Forms;

namespace ContactPoint.BaseDesign
{
    public class LineSeparator : UserControl
    {
        public LineSeparator()
        {
            this.Paint += LineSeparator_Paint;

            this.MaximumSize = new Size(2000, 2);
            this.MinimumSize = new Size(0, 2);

            this.Width = 350;
            this.Height = 2;
        }

        private void LineSeparator_Paint(object sender, PaintEventArgs e)
        {
            using (Graphics g = e.Graphics)
            {
                g.DrawLine(Pens.DarkGray, new Point(0, 0), new Point(this.Width, 0));
                g.DrawLine(Pens.White, new Point(0, 1), new Point(this.Width, 1));
            }
        }
    }
}
