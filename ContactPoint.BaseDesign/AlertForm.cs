using System;
using System.Drawing;
using System.Windows.Forms;

namespace ContactPoint.BaseDesign
{
    public partial class AlertForm : ComponentFactory.Krypton.Toolkit.KryptonForm
    {
        public AlertForm(string message)
        {
            InitializeComponent();

            this.DialogResult = DialogResult.Cancel;
            this.lblText.Text = message;
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        #region Window header routine

        bool _mouseDown = false;
        private Point _mousePos;

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            _mouseDown = true;

            _mousePos.X = e.X;
            _mousePos.Y = e.Y;
        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            _mouseDown = false;
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (_mouseDown)
            {
                Point current_pos = Control.MousePosition;
                current_pos.X = current_pos.X - _mousePos.X;
                current_pos.Y = current_pos.Y - _mousePos.Y;

                this.Location = current_pos;
            }
        }

        #endregion
    }
}
