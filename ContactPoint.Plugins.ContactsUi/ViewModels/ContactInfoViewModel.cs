using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using ContactPoint.Common.Contacts;
using ContactPoint.Common.Contacts.Local;

namespace ContactPoint.Plugins.ContactsUi.ViewModels
{
    internal class ContactInfoViewModel : ViewModel
    {
        private readonly IContactsManager _contactsManager;
        private string _firstName;
        private string _lastName;
        private string _middleName;
        private string _company;
        private string _jobTitle;
        private string _note;

        public string AddressBookName { get; private set; }
        public long AddressBookId { get; private set; }

        public string FirstName
        {
            get { return _firstName; }
            set { _firstName = value; NotifyPropertyChanged("FirstName"); }
        }

        public string LastName
        {
            get { return _lastName; }
            set { _lastName = value; NotifyPropertyChanged("LastName"); }
        }

        public string MiddleName
        {
            get { return _middleName; }
            set { _middleName = value; NotifyPropertyChanged("MiddleName"); }
        }

        public string Company
        {
            get { return _company; }
            set { _company = value; NotifyPropertyChanged("Company"); }
        }

        public string JobTitle
        {
            get { return _jobTitle; }
            set { _jobTitle = value; NotifyPropertyChanged("JobTitle"); }
        }

        public string Note
        {
            get { return _note; }
            set { _note = value; NotifyPropertyChanged("Note"); }
        }

        public bool ReadOnly { get; private set; }

        public IContactInfoLocal ContactInfo { get; private set; }

        public ObservableCollection<ContactPhoneViewModel> PhoneNumbers { get; private set; }
        public ObservableCollection<ContactEmailViewModel> Emails { get; private set; }
        public List<IContactTagLocal> Tags { get; private set; }

        public ContactInfoViewModel(IContactsManager contactsManager, IContactInfoLocal contactInfo)
        {
            _contactsManager = contactsManager;
            ContactInfo = contactInfo;

            RefreshInfo();
        }

        public void RefreshInfo()
        {
            AddressBookName = ContactInfo.AddressBook.Name;
            AddressBookId = ContactInfo.AddressBook.Id;

            FirstName = ContactInfo.FirstName;
            LastName = ContactInfo.LastName;
            MiddleName = ContactInfo.MiddleName;
            Company = ContactInfo.Company;
            JobTitle = ContactInfo.JobTitle;
            Note = ContactInfo.Note;

            ReadOnly = ContactInfo.AddressBook.ReadOnly;

            PhoneNumbers = new ObservableCollection<ContactPhoneViewModel>();
            foreach (var item in ContactInfo.PhoneNumbers)
                PhoneNumbers.Add(new ContactPhoneViewModel(_contactsManager.Core.CallManager, item));

            Emails = new ObservableCollection<ContactEmailViewModel>();
            foreach (var item in ContactInfo.Emails)
                Emails.Add(new ContactEmailViewModel(item));

            Tags = ContactInfo.Tags.ToList();
        }

        public void UnWrap(IContactInfoLocal contactInfo)
        {
            contactInfo.FirstName = FirstName;
            contactInfo.LastName = LastName;
            contactInfo.MiddleName = MiddleName;
            contactInfo.Company = Company;
            contactInfo.JobTitle = JobTitle;
            contactInfo.Note = Note;

            // Update phone numbers
            foreach (var item in PhoneNumbers)
            {
                if (string.IsNullOrWhiteSpace(item.Number)) continue;

                var target = contactInfo.PhoneNumbers.FirstOrDefault(t => t.Id == item.Id);
                if (target == null || item.Id < 0)
                {
                    target = contactInfo.AddressBook.CreatePhoneNumber();
                    contactInfo.PhoneNumbers.Add(target);
                }

                item.UnWrap(target);
            }
            foreach (var item in contactInfo.PhoneNumbers.Where(x => PhoneNumbers.All(y => x.Id != y.Id) || string.IsNullOrWhiteSpace(x.Number)).ToArray())
                contactInfo.PhoneNumbers.Remove(item);

            // Update emails
            foreach (var item in Emails)
            {
                if (string.IsNullOrWhiteSpace(item.Email)) continue;

                var target = contactInfo.Emails.FirstOrDefault(t => t.Id == item.Id);
                if (target == null || item.Id < 0)
                {
                    target = contactInfo.AddressBook.CreateEmail();
                    contactInfo.Emails.Add(target);
                }

                item.UnWrap(target);
            }
            foreach (var item in contactInfo.Emails.Where(x => Emails.All(y => x.Id != y.Id) || string.IsNullOrWhiteSpace(x.Email)).ToArray())
                contactInfo.Emails.Remove(item);

            // Update tags
            foreach (var item in Tags)
            {
                var target = contactInfo.Tags.FirstOrDefault(t => t.Id == item.Id);
                if (target != null) continue;

                target = _contactsManager.Tags.FirstOrDefault(x => x.Id == item.Id);
                if (target != null) contactInfo.Tags.Add(target);
            }
            foreach (var item in contactInfo.Tags.Where(x => Tags.All(y => x.Id != y.Id)).ToArray())
                contactInfo.Tags.Remove(item);
        }
    }
}
