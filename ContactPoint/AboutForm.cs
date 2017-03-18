using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ContactPoint.Common;

namespace ContactPoint
{
    public partial class AboutForm : Form
    {
        public AboutForm()
        {
            InitializeComponent();

            try
            {
                pictureBoxPartnerLogo.Image = Image.FromFile("partner_logo.png");
            }
            catch
            {
                Logger.LogNotice("Unable to load partner logo.");
            }

            labelVersion.Text = GetType().Assembly.GetName().Version.ToString(4);
        }
    }
}
