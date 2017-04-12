using System;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;

namespace ContactPoint.Forms
{
    public partial class LoaderForm : Form
    {
        private Image _partnerLogo = null;

        public LoaderForm()
        {
            InitializeComponent();

            var assembly = GetType().Assembly;
            labelVersion.Text = "version: " + (assembly.ReflectionOnly ? null : assembly.GetCustomAttribute<AssemblyFileVersionAttribute>()?.Version) ?? assembly.GetName().Version.ToString(4);

            labelTrademarks.Text =
                "© Copyright ContactPoint company 2008, " + DateTime.Now.Year.ToString() + ". All rights reserved.\r\n" +
                "ContactPoint and all ContactPoint-based trademarks and logos are\r\n" +
                "trademarks or registered trademarks of ContactPoint";

            TryLoadPartnerImage();

            SetStyle(ControlStyles.DoubleBuffer | ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint, true);
            UpdateStyles();
        }

        internal void SetLoadingText(string text)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action<string>(SetLoadingText), new object[] { text });
                return;
            }

            labelLoadingText.Text = text;
        }

        internal void TryClose()
        {
            if (InvokeRequired)
                BeginInvoke(new MethodInvoker(Close));
            else
                Close();
        }

        private void TryLoadPartnerImage()
        {
            try
            {
                _partnerLogo = Image.FromFile("partner_logo.png");

                pictureBox1.Paint += BackgroundRepaint;
            }
            catch { }
        }

        private void BackgroundRepaint(object sender, PaintEventArgs e)
        {
            if (_partnerLogo != null)
                e.Graphics.DrawImage(_partnerLogo, 270, 194, 139, 36);
        }

        private void LoaderForm_FormClosing(object sender, FormClosingEventArgs e)
        {
        }
    }
}
