using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace ContactPoint.Controls
{
    [Designer(typeof(TranspControlDesigner))]
    public class TransparentPanel : Panel
    {
        private bool _drag;
        private Color _fillColor = Color.White;
        private int _opacity = 100;
        private int _alpha;

        public TransparentPanel()
        {
            SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            SetStyle(ControlStyles.Opaque, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.ContainerControl, true);
            SetStyle(ControlStyles.UserPaint, true);
        }

        public Color FillColor
        {
            get
            {
                return _fillColor;
            }
            set
            {
                _fillColor = value;
                Parent?.Invalidate(Bounds, true);
            }
        }

        public int Opacity
        {
            get
            {
                if (_opacity > 100) { _opacity = 100; }
                else if (_opacity < 1) { _opacity = 1; }
                return _opacity;
            }
            set
            {
                _opacity = value;
                if (Parent != null) Parent.Invalidate(Bounds, true);
            }
        }

        public override Color BackColor
        {
            get { return Color.Transparent; }
            set { }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            Graphics g = e.Graphics;
            Rectangle bounds = new Rectangle(0, 0, Width, Height);

            Color frmColor = Parent.BackColor;
            Brush brushColor;
            Brush bckColor;

            _alpha = (_opacity * 255) / 100;

            if (_drag)
            {
                Color dragFillColor;
                Color dragBckColor;

                if (BackColor != Color.Transparent)
                {
                    int rb = BackColor.R * _alpha / 255 + frmColor.R * (255 - _alpha) / 255;
                    int gb = BackColor.G * _alpha / 255 + frmColor.G * (255 - _alpha) / 255;
                    int bb = BackColor.B * _alpha / 255 + frmColor.B * (255 - _alpha) / 255;
                    dragBckColor = Color.FromArgb(rb, gb, bb);
                }
                else dragBckColor = frmColor;

                if (_fillColor != Color.Transparent)
                {
                    int rf = _fillColor.R * _alpha / 255 + frmColor.R * (255 - _alpha) / 255;
                    int gf = _fillColor.G * _alpha / 255 + frmColor.G * (255 - _alpha) / 255;
                    int bf = _fillColor.B * _alpha / 255 + frmColor.B * (255 - _alpha) / 255;
                    dragFillColor = Color.FromArgb(rf, gf, bf);
                }
                else dragFillColor = dragBckColor;

                _alpha = 255;
                brushColor = new SolidBrush(Color.FromArgb(_alpha, dragFillColor));
                bckColor = new SolidBrush(Color.FromArgb(_alpha, dragBckColor));
            }
            else
            {
                Color color = _fillColor;
                brushColor = new SolidBrush(Color.FromArgb(_alpha, color));
                bckColor = new SolidBrush(Color.FromArgb(_alpha, BackColor));
            }

            Pen pen = new Pen(ForeColor);

            if (BackColor != Color.Transparent | _drag)
            {
                g.FillRectangle(bckColor, bounds);
            }

            if (FillColor != Color.Transparent | _drag)
            {
                g.FillRectangle(brushColor, bounds);
            }
            else g.FillRectangle(new SolidBrush(Color.FromArgb(1, Color.White)), bounds);

            g.DrawRectangle(pen, bounds);

            pen.Dispose();
            brushColor.Dispose();
            bckColor.Dispose();
            g.Dispose();
        }

        protected override void OnBackColorChanged(EventArgs e)
        {
            Parent?.Invalidate(Bounds, true);
            base.OnBackColorChanged(e);
        }

        protected override void OnParentBackColorChanged(EventArgs e)
        {
            Invalidate();
            base.OnParentBackColorChanged(e);
        }

        internal class TranspControlDesigner : ControlDesigner
        {
            private TransparentPanel _myControl;

            protected override void OnMouseDragMove(int x, int y)
            {
                _myControl = (TransparentPanel)(Control);
                _myControl._drag = true;
                base.OnMouseDragMove(x, y);
            }

            protected override void OnMouseLeave()
            {
                _myControl = (TransparentPanel)(Control);
                _myControl._drag = false;
                base.OnMouseLeave();
            }
        }
    }
}
