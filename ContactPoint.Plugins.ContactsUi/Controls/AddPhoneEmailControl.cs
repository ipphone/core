using System;
using System.Windows.Forms;

namespace ContactPoint.Plugins.ContactsUi.Controls
{
  internal partial class AddPhoneEmailControl : UserControl
    {
        public event Action AddPhone;
        public event Action AddEmail;

        public AddPhoneEmailControl()
        {
            InitializeComponent();
        }

        private void buttonAddPhone_Click(object sender, EventArgs e)
        {
            if (AddPhone != null) AddPhone();
        }

        private void buttonAddEmail_Click(object sender, EventArgs e)
        {
            if (AddEmail != null) AddEmail();
        }
    }
}
