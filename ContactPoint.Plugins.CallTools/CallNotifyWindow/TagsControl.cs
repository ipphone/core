using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ContactPoint.Common.Contacts;

namespace ContactPoint.Plugins.CallTools.CallNotifyWindow
{
    public partial class TagsControl : UserControl
    {
        private IContact _contact;

        public IContact Contact
        {
            get { return _contact; }
            set { _contact = value; RefreshData(); }
        }

        public TagsControl()
        {
            InitializeComponent();
        }
    
        private void RefreshData()
        {
            if (_contact == null) return;

            panelContainer.SuspendLayout();
            panelContainer.Controls.Clear();

            foreach (var tag in _contact.ContactInfos.SelectMany(x => x.Tags))
                panelContainer.Controls.Add(new Label()
                    {
                        Text = "#" + tag.Name, 
                        ForeColor = ParseColor(tag.Color), 
                        AutoSize = true, 
                        Font = new Font(FontFamily.GenericSansSerif, 10), 
                            Margin = new Padding(3, 0, 0, 0)
                    });

            panelContainer.ResumeLayout(true);
        }

        private static Color ParseColor(string color)
        {
            if (string.IsNullOrEmpty(color) || !color.StartsWith("#") || color.Length != 7) return Color.Black;

            var r = byte.Parse(color.Substring(1, 2), NumberStyles.HexNumber);
            var g = byte.Parse(color.Substring(3, 2), NumberStyles.HexNumber);
            var b = byte.Parse(color.Substring(5, 2), NumberStyles.HexNumber);

            return Color.FromArgb(r, g, b);
        }
    }
}
