using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ContactPoint.BaseDesign.Components;
using ContactPoint.Common;
using ContactPoint.Common.Contacts;
using ContactPoint.Common.Contacts.Local;
using ContactPoint.Plugins.ContactsUi.ViewModels;

namespace ContactPoint.Plugins.ContactsUi.Controls
{
    internal class ContactDetailsControl : TabPage
    {
        private readonly IContactsManager _contactsManager;
        private readonly ContactInfoViewModel _contactInfoViewModel;
        private readonly VerticalLayoutPanel _panel;
        private readonly AddPhoneEmailControl _addPhoneEmailControl;

        public ContactInfoViewModel ContactInfoViewModel
        {
            get { return _contactInfoViewModel; }
        }

        public ContactDetailsControl(IContactsManager contactsManager, ContactInfoViewModel contactInfoViewModel)
        {
            _contactsManager = contactsManager;
            _contactInfoViewModel = contactInfoViewModel;

            _panel = new VerticalLayoutPanel { Dock = DockStyle.Fill };
            _panel.UseZebra = false;

            _addPhoneEmailControl = new AddPhoneEmailControl();
            _addPhoneEmailControl.AddPhone += PhoneEmailControlAddPhone;
            _addPhoneEmailControl.AddEmail += PhoneEmailControlAddEmail;

            Controls.Add(_panel);

            RefreshInfo();

            _contactInfoViewModel.PhoneNumbers.CollectionChanged += PhoneNumbersEmailsCollectionChanged;
            _contactInfoViewModel.Emails.CollectionChanged += PhoneNumbersEmailsCollectionChanged;
        }

        void PhoneNumbersEmailsCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            RefreshInfo();
        }

        private void RefreshInfo()
        {
            _panel.SuspendLayout();

            _panel.Controls.Clear();

            Text = _contactInfoViewModel.AddressBookName;

            _panel.Controls.Add(new TextDetailsControl("Last name:", _contactInfoViewModel.LastName, s => _contactInfoViewModel.LastName = s) {ReadOnly = _contactInfoViewModel.ReadOnly});
            _panel.Controls.Add(new TextDetailsControl("First name:", _contactInfoViewModel.FirstName, s => _contactInfoViewModel.FirstName = s) { ReadOnly = _contactInfoViewModel.ReadOnly });
            _panel.Controls.Add(new TextDetailsControl("Middle name:", _contactInfoViewModel.MiddleName, s => _contactInfoViewModel.MiddleName = s) { ReadOnly = _contactInfoViewModel.ReadOnly });
            _panel.Controls.Add(new TextDetailsControl("Company:", _contactInfoViewModel.Company, s => _contactInfoViewModel.Company = s) { ReadOnly = _contactInfoViewModel.ReadOnly });
            _panel.Controls.Add(new TextDetailsControl("Job title:", _contactInfoViewModel.JobTitle, s => _contactInfoViewModel.JobTitle = s) { ReadOnly = _contactInfoViewModel.ReadOnly });
            _panel.Controls.Add(new TextDetailsControl("Note:", _contactInfoViewModel.Note, s => _contactInfoViewModel.Note = s) { ReadOnly = _contactInfoViewModel.ReadOnly });

            foreach (var item in _contactInfoViewModel.PhoneNumbers)
                _panel.Controls.Add(new PhoneControl(item, _contactInfoViewModel) { ReadOnly = _contactInfoViewModel.ReadOnly });

            foreach (var item in _contactInfoViewModel.Emails)
                _panel.Controls.Add(new EmailControl(item, _contactInfoViewModel) { ReadOnly = _contactInfoViewModel.ReadOnly });

            if (!_contactInfoViewModel.ReadOnly)
                _panel.Controls.Add(_addPhoneEmailControl);

            _panel.ResumeLayout(true);
        }

        void PhoneEmailControlAddPhone()
        {
            _contactInfoViewModel.PhoneNumbers.Add(new ContactPhoneViewModel(_contactsManager.Core.CallManager, _contactInfoViewModel.ContactInfo.AddressBook.CreatePhoneNumber()));
        }

        void PhoneEmailControlAddEmail()
        {
            _contactInfoViewModel.Emails.Add(new ContactEmailViewModel(_contactInfoViewModel.ContactInfo.AddressBook.CreateEmail()));
        }
    }
}
