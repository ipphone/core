using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Forms;
using ComponentFactory.Krypton.Toolkit;
using ContactPoint.Common.Contacts;
using ContactPoint.Common.Contacts.Local;
using ContactPoint.Plugins.ContactsUi.Forms;

namespace ContactPoint.Plugins.ContactsUi.ViewModels
{
    internal class ContactViewModel : ViewModel
    {
        private readonly IContact _contact;
        private readonly IContactsManager _contactsManager;

        private ObservableCollection<ContactInfoViewModel> _contactInfos;
        private ContactInfoViewModel _selectedContactInfo;

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MiddleName { get; set; }
        public string Company { get; set; }

        public IContact Contact
        {
            get { return _contact; }
        }

        public ObservableCollection<ContactInfoViewModel> ContactInfos
        {
            get { return _contactInfos; }
        }

        public IEnumerable<ContactTagViewModel> Tags
        {
            get { return _contactInfos.SelectMany(x => x.Tags).Distinct().Select(x => new ContactTagViewModel(x)); }
        }

        public IEnumerable<ContactTagViewModel> TagLocals
        {
            get { return _contactInfos.SelectMany(x => x.ContactInfo.Tags).Distinct().Select(x => new ContactTagViewModel(x)); }
        }

        public string ShowedName
        {
            get { return _contact.ShowedName; }
        }

        public string PhoneNumbersString
        {
            get
            {
                return string.Join(", ",
                                   ContactInfos.SelectMany(
                                       x => x.PhoneNumbers.OfType<ContactPhoneViewModel>().Select(y => y.Number))
                                               .Distinct());
            }
        }

        public IEnumerable<ContactPhoneViewModel> PhoneNumbers
        {
            get
            {
                return ContactInfos.SelectMany(x => x.PhoneNumbers);
            }
        }

        public IEnumerable<ContactPhoneViewModel> PhoneNumbersNotEmpty
        {
            get
            {
                return ContactInfos.SelectMany(x => x.PhoneNumbers.Where(y => !string.IsNullOrEmpty(y.Number)));
            }
        }

        public IEnumerable<ContactEmailViewModel> EmailsNotEmpty
        {
            get
            {
                return ContactInfos.SelectMany(x => x.Emails.Where(y => !string.IsNullOrEmpty(y.Email)));
            }
        }


        public IEnumerable<KryptonCommand> AddressBooksItems
        {
            get
            {
                return _contactsManager.AddressBooks.Where(x => !x.ReadOnly && _contactInfos.All(y => y.AddressBookId != x.Id)).Select(x => new AddContactInfoCommand(this, _contactsManager, x));
            }
        }

        public ContactInfoViewModel SelectedContactInfo
        {
            get { return _selectedContactInfo; }
            set
            {
                _selectedContactInfo = value; 
                
                NotifyPropertyChanged("SelectedContactInfo");
                NotifyPropertyChanged("AddTagButtonVisibility");
            }
        }

        public string Note
        {
            get
            {
                return string.Join("; ", ContactInfos.Where(x => !string.IsNullOrEmpty(x.Note)).Select(x => x.Note));
            }
        }

        internal ContactViewModel(IContactsManager contactsManager, IContact contact)
        {
            _contactsManager = contactsManager;
            _contact = contact;

            _contact.Changed += ContactChanged;

            RefreshInfo();
        }

        void ContactChanged(IContact obj)
        {
            RefreshInfo();
        }
    
        internal void AddNewTag(IAddressBookLocal addressBook)
        {
            var tagWindow = new TagForm(new ContactTagViewModel(addressBook.CreateTag()));
            if (tagWindow.ShowDialog() == DialogResult.OK)
                NotifyPropertyChanged("Tags");
        }

        internal void AddTag(IContactTagLocal tag)
        {
            if (tag == null) return;
            if (SelectedContactInfo == null) return;
            if (SelectedContactInfo.Tags.Any(x => x.Id == tag.Id)) return;

            SelectedContactInfo.Tags.Add(tag);

            NotifyPropertyChanged("Tags");
        }

        internal void RemoveTag(ContactTagViewModel tagViewModel)
        {
            if (tagViewModel == null || tagViewModel.ReadOnly) return;

            foreach (var contactInfo in _contactInfos.Where(x => x.Tags.Any(y => y.Id == tagViewModel.Tag.Id)))
                contactInfo.Tags.Remove(tagViewModel.Tag);
        }

        internal void SubmitContact()
        {
            _contact.FirstName = ContactInfos.Where(x => !string.IsNullOrEmpty(x.FirstName)).Select(x => x.FirstName).FirstOrDefault();
            _contact.LastName = ContactInfos.Where(x => !string.IsNullOrEmpty(x.LastName)).Select(x => x.LastName).FirstOrDefault();
            _contact.MiddleName = ContactInfos.Where(x => !string.IsNullOrEmpty(x.MiddleName)).Select(x => x.MiddleName).FirstOrDefault();
            _contact.Company = ContactInfos.Where(x => !string.IsNullOrEmpty(x.Company)).Select(x => x.Company).FirstOrDefault();

            foreach (var item in ContactInfos)
            {
                item.UnWrap(item.ContactInfo);

                if (_contact.ContactInfos.All(x => x.Id != item.ContactInfo.Id))
                    _contact.LinkContactInfo(item.ContactInfo);
            }

            _contact.Submit();
        }

        public void RefreshInfo()
        {
            FirstName = _contact.FirstName;
            LastName = _contact.LastName;
            MiddleName = _contact.MiddleName;
            Company = _contact.Company;

            _contactInfos = new ObservableCollection<ContactInfoViewModel>(_contact.ContactInfos.Select(x => new ContactInfoViewModel(_contactsManager, x)));
            foreach (var item in _contactInfos)
                item.RefreshInfo();

            SelectedContactInfo = ContactInfos.FirstOrDefault();
        }

        public void CancelChanges()
        {
            RefreshInfo();
        }
    }

    internal class AddContactInfoCommand : KryptonCommand
    {
        private readonly ContactViewModel _contactViewModel;
        private readonly IContactsManager _contactsManager;
        private readonly IAddressBookLocal _addressBook;

        public AddContactInfoCommand(ContactViewModel contactViewModel, IContactsManager contactsManager, IAddressBookLocal addressBook)
        {
            _contactViewModel = contactViewModel;
            _contactsManager = contactsManager;
            _addressBook = addressBook;

            Text = addressBook.Name;
        }

        protected override void OnExecute(EventArgs e)
        {
            base.OnExecute(e);

            if (_addressBook == null) return;

            var contactInfo = _addressBook.CreateContactInfo();

            var contactInfoViewModel = new ContactInfoViewModel(_contactsManager, contactInfo);
            _contactViewModel.ContactInfos.Add(contactInfoViewModel);

            _contactViewModel.SelectedContactInfo = contactInfoViewModel;
        }
    }
}
