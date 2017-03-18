using System;
using System.Windows.Forms;

namespace ContactPoint.Plugins.ContactsUi.Controls
{
  public partial class TextDetailsControl : UserControl
    {
        private readonly Action<string> _valueFunc;

        public bool ReadOnly
        {
            get { return textBox.ReadOnly; }
            set { textBox.ReadOnly = value; }
        }

        public TextDetailsControl(string name, string initialValue, Action<string> valueFunc)
        {
            _valueFunc = valueFunc;
            InitializeComponent();

            label.Text = name;
            textBox.Text = initialValue;
        }

        private void textBox_TextChanged(object sender, EventArgs e)
        {
            _valueFunc(textBox.Text);
        }
    }
}
