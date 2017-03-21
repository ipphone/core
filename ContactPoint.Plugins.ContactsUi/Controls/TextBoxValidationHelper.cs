using System;
using System.Drawing;
using System.Windows.Forms;

namespace ContactPoint.Plugins.ContactsUi.Controls
{
  internal class TextBoxValidationHelper
    {
        private readonly TextBox _textBox;
        private readonly Func<bool> _validator;
        private readonly Color _initialColor;

        public TextBoxValidationHelper(TextBox textBox, Func<bool> validator)
        {
            _textBox = textBox;
            _validator = validator;

            _initialColor = textBox.BackColor;

            textBox.Validating += TextBoxValidating;
        }

        void TextBoxValidating(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!_validator.Invoke())
            {
                _textBox.BackColor = Color.LightCoral;
                e.Cancel = true;
            }
            else
            {
                _textBox.BackColor = _initialColor;
                e.Cancel = false;
            }
        }
    }
}
