using System;
using System.Drawing;
using System.Windows.Forms;

namespace ContactPoint.BaseDesign.Components
{
    public class TextBoxWatermarkExtender
    {
        private readonly TextBox _textBox;
        private readonly string _watermarkText;
        private readonly Color _foreColor;
        private bool _enabled;

        public bool Enabled
        {
            get { return _enabled; }
        }

        public string GetText()
        {
            return !_enabled ? _textBox.Text : string.Empty;
        }

        public TextBoxWatermarkExtender(TextBox textBox, string watermarkText)
        {
            _textBox = textBox;
            _watermarkText = watermarkText;
            _foreColor = _textBox.ForeColor;

            if (!_textBox.ReadOnly) ActivateWatermark();

            _textBox.TextChanged += TextBoxTextChanged;
            _textBox.GotFocus += TextBoxGotFocus;
            _textBox.LostFocus += TextBoxLostFocus;
            _textBox.ReadOnlyChanged += TextBoxReadOnlyChanged;
        }

        void TextBoxReadOnlyChanged(object sender, EventArgs e)
        {
            if (_textBox.ReadOnly && _enabled) DeactivateWatermark();
        }

        private void TextBoxLostFocus(object sender, EventArgs e)
        {
            if (_textBox.ReadOnly) return;

            if (string.IsNullOrEmpty(_textBox.Text) && !_enabled)
                ActivateWatermark();
        }

        private void TextBoxGotFocus(object sender, EventArgs e)
        {
            if (_textBox.ReadOnly) return;

            if (_enabled) DeactivateWatermark();
        }

        void TextBoxTextChanged(object sender, EventArgs e)
        {
            if (_textBox.ReadOnly) return;
            if (_textBox.Focused) return;

            if (string.IsNullOrEmpty(_textBox.Text))
            {
                if (!_enabled) ActivateWatermark();
                else _textBox.Text = _watermarkText;
            }
            else
                if (_enabled && !string.Equals(_textBox.Text, _watermarkText, StringComparison.InvariantCulture)) DeactivateWatermark();
        }

        void ActivateWatermark()
        {
            _enabled = true;
            _textBox.Text = _watermarkText;
            _textBox.ForeColor = Color.Gray;
        }

        void DeactivateWatermark()
        {
            if (string.Equals(_textBox.Text, _watermarkText, StringComparison.InvariantCulture) && _enabled)
                _textBox.Text = string.Empty;

            _textBox.ForeColor = _foreColor;
            _enabled = false;
        }
    }
}
