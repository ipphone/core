using System;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using ContactPoint.BaseDesign.Components;
using ContactPoint.Plugins.ContactsUi.ViewModels;

namespace ContactPoint.Plugins.ContactsUi.Controls
{
  internal partial class EmailControl : UserControl
    {
        private readonly ContactEmailViewModel _emailViewModel;
        private readonly ContactInfoViewModel _contactInfoViewModel;
        private readonly TextBoxWatermarkExtender _emailWatermark;
        private readonly TextBoxWatermarkExtender _commentWatermark;
        private bool _readOnly;

        public bool ReadOnly
        {
            get { return _readOnly; }
            set { _readOnly = textBoxComment.ReadOnly = textBoxEmail.ReadOnly = value; }
        }

        public EmailControl(ContactEmailViewModel emailViewModel, ContactInfoViewModel contactInfoViewModel)
        {
            _emailViewModel = emailViewModel;
            _contactInfoViewModel = contactInfoViewModel;

            InitializeComponent();

            _emailWatermark = new TextBoxWatermarkExtender(textBoxEmail, "email address");
            _commentWatermark = new TextBoxWatermarkExtender(textBoxComment, "comment...");

            textBoxEmail.Text = _emailViewModel.Email;
            textBoxComment.Text = _emailViewModel.Comment;

            _readOnly = textBoxEmail.ReadOnly = textBoxComment.ReadOnly = contactInfoViewModel.ReadOnly;
            toolStripMenuItemRemove.Enabled = !_readOnly;

            textBoxEmail.TextChanged += textBoxEmail_TextChanged;
            textBoxComment.TextChanged += textBoxComment_TextChanged;

            new TextBoxValidationHelper(textBoxEmail, () => IsValidEmail(_emailWatermark.GetText()));
        }

        void textBoxComment_TextChanged(object sender, EventArgs e)
        {
            _emailViewModel.Comment = _commentWatermark.GetText();
        }

        void textBoxEmail_TextChanged(object sender, EventArgs e)
        {
            _emailViewModel.Email = _emailWatermark.GetText();
        }

        private void MenuItemRemoveEmailClick(object sender, System.EventArgs e)
        {
            if (MessageBox.Show(string.Format("Delete '{0}'?", _emailViewModel.Email), "Confirm email delete.", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                _contactInfoViewModel.Emails.Remove(_emailViewModel);
        }

        private void MenuItemCreateEmailClick(object sender, System.EventArgs e)
        {
            _emailViewModel.SendEmail();
        }

        private void label_Click(object sender, EventArgs e)
        {
            _emailViewModel.SendEmail();
        }

        #region Email validation

        private bool _isEmailValid;

        private bool IsValidEmail(string strIn)
        {
            _isEmailValid = false;
            if (String.IsNullOrEmpty(strIn))
                return false;

            // Use IdnMapping class to convert Unicode domain names.
            strIn = Regex.Replace(strIn, @"(@)(.+)$", this.DomainMapper);
            if (_isEmailValid)
                return false;

            // Return true if strIn is in valid e-mail format.
            return Regex.IsMatch(strIn,
                   @"^(?("")(""[^""]+?""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" +
                   @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-\w]*[0-9a-z]*\.)+[a-z0-9]{2,17}))$",
                   RegexOptions.IgnoreCase);
        }

        private string DomainMapper(Match match)
        {
            // IdnMapping class with default property values.
            IdnMapping idn = new IdnMapping();

            string domainName = match.Groups[2].Value;
            try
            {
                domainName = idn.GetAscii(domainName);
            }
            catch (ArgumentException)
            {
                _isEmailValid = true;
            }
            return match.Groups[1].Value + domainName;
        }
        #endregion
    }
}
