using System;
using System.Windows.Forms;
using ContactPoint.BaseDesign.Components;
using ContactPoint.Plugins.ContactsUi.ViewModels;

namespace ContactPoint.Plugins.ContactsUi.Controls
{
  internal partial class PhoneControl : UserControl
    {
        private readonly ContactPhoneViewModel _phoneViewModel;
        private readonly ContactInfoViewModel _contactInfoViewModel;
        private readonly TextBoxWatermarkExtender _numberWatermark;
        private readonly TextBoxWatermarkExtender _commentWatermark;
        private bool _readOnly;

        public bool ReadOnly
        {
            get { return _readOnly; }
            set { _readOnly = textBoxComment.ReadOnly = textBoxNumber.ReadOnly = value; }
        }

        public PhoneControl(ContactPhoneViewModel phoneViewModel, ContactInfoViewModel contactInfoViewModel)
        {
            _phoneViewModel = phoneViewModel;
            _contactInfoViewModel = contactInfoViewModel;

            InitializeComponent();

            _numberWatermark = new TextBoxWatermarkExtender(textBoxNumber, "phone number");
            _commentWatermark = new TextBoxWatermarkExtender(textBoxComment, "comment...");

            textBoxNumber.Text = _phoneViewModel.Number;
            textBoxComment.Text = _phoneViewModel.Comment;

            _readOnly = textBoxNumber.ReadOnly = textBoxComment.ReadOnly = contactInfoViewModel.ReadOnly;
            toolStripMenuItemRemove.Enabled = !_readOnly;

            textBoxNumber.TextChanged += TextBoxNumberTextChanged;
            textBoxComment.TextChanged += TextBoxCommentTextChanged;
        }

        void TextBoxCommentTextChanged(object sender, EventArgs e)
        {
            _phoneViewModel.Comment = _commentWatermark.GetText();
        }

        void TextBoxNumberTextChanged(object sender, EventArgs e)
        {
            _phoneViewModel.Number = _numberWatermark.GetText();
        }

        private void MenuItemRemoveNumberClick(object sender, System.EventArgs e)
        {
            if (MessageBox.Show(string.Format("Delete '{0}'?", _phoneViewModel.Number), "Confirm number delete.", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                _contactInfoViewModel.PhoneNumbers.Remove(_phoneViewModel);
        }

        private void MenuItemCallNumberClick(object sender, System.EventArgs e)
        {
            _phoneViewModel.Call();
        }

        private void label_Click(object sender, EventArgs e)
        {
            _phoneViewModel.Call();
        }
    }
}
