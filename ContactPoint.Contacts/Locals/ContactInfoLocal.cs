using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using ContactPoint.Common.Contacts;
using ContactPoint.Common.Contacts.Local;

namespace ContactPoint.Contacts.Locals
{
    internal class ContactInfoLocal : VersionableLocal, IContactInfoLocal
    {
        private readonly ContactsManager _contactsManager;
        private string _firstName;
        private string _middleName;
        private string _lastName;
        private string _company;
        private string _jobTitle;
        private string _note;

        private readonly ICollection<IContactTag> _tagsWrapper;
        private readonly ICollection<IContactPhone> _phoneNumbersWrapper;
        private readonly ICollection<IContactEmail> _emailsWrapper;

        public event Action<IContactInfoLocal> Changed;

        public long Id { get; set; }
        public AddressBookLocal AddressBook { get; private set; }
        public string Key { get; set; }
        public IContact Contact { get; set; }
        public bool IsDeleted { get; set; }

        public string FirstName
        {
            get { return _firstName; }
            set { IsChanged |= _firstName != value; _firstName = value; }
        }

        public string MiddleName
        {
            get { return _middleName; }
            set { IsChanged |= _middleName != value; _middleName = value; }
        }

        public string LastName
        {
            get { return _lastName; }
            set { IsChanged |= _lastName != value; _lastName = value; }
        }

        public string Company
        {
            get { return _company; }
            set { IsChanged |= _company != value; _company = value; }
        }

        public string JobTitle
        {
            get { return _jobTitle; }
            set { IsChanged |= _jobTitle != value; _jobTitle = value; }
        }

        public string Note
        {
            get { return _note; }
            set { IsChanged |= _note != value; _note = value; }
        }

        public override bool IsChanged
        {
            get { return base.IsChanged || PhoneNumbers.Any(x => x.IsChanged) || Emails.Any(x => x.IsChanged); }
            set { base.IsChanged = value; }
        }

        public ObservableCollection<IContactTagLocal> Tags { get; private set; }
        public ObservableCollection<IContactPhoneLocal> PhoneNumbers { get; private set; }
        public ObservableCollection<IContactEmailLocal> Emails { get; private set; }

        ICollection<IContactTagLocal> IContactInfoLocal.Tags { get { return Tags; } }
        ICollection<IContactPhoneLocal> IContactInfoLocal.PhoneNumbers { get { return PhoneNumbers; } }
        ICollection<IContactEmailLocal> IContactInfoLocal.Emails { get { return Emails; } }

        ICollection<IContactPhone> IContactInfo.PhoneNumbers { get { return _phoneNumbersWrapper; } }
        ICollection<IContactEmail> IContactInfo.Emails { get { return _emailsWrapper; } }
        ICollection<IContactTag> IContactInfo.Tags { get { return _tagsWrapper; } }
    
        IAddressBookLocal IContactInfoLocal.AddressBook { get { return AddressBook; } }

        public ContactInfoLocal(long id, AddressBookLocal addressBook, ContactsManager contactsManager)
            : base(addressBook)
        {
            Id = id;
            AddressBook = addressBook;
            Key = String.Empty;
            _contactsManager = contactsManager;

            PhoneNumbers = new ObservableCollection<IContactPhoneLocal>();
            Emails = new ObservableCollection<IContactEmailLocal>();
            Tags = new ObservableCollection<IContactTagLocal>();

            _phoneNumbersWrapper = new ObservableCollectionMapperConverter<IContactPhoneLocal, IContactPhone>(PhoneNumbers);
            _emailsWrapper = new ObservableCollectionMapperConverter<IContactEmailLocal, IContactEmail>(Emails);
            _tagsWrapper = new ObservableCollectionMapperConverter<IContactTagLocal, IContactTag>(Tags);

            PhoneNumbers.CollectionChanged += InternalCollectionChanged;
            Emails.CollectionChanged += InternalCollectionChanged;
            Tags.CollectionChanged += InternalCollectionChanged;
        }

        public ContactInfoLocal(AddressBookLocal addressBook, ContactsManager contactsManager)
            : this(-1, addressBook, contactsManager)
        { }

        public ContactInfoLocal(IContactInfo contactInfo, AddressBookLocal addressBook, ContactsManager contactsManager)
            : this(-1, addressBook, contactsManager)
        {
            UpdateFrom(contactInfo, true);
        }

        public ContactInfoLocal(IContactInfoLocal contactInfo, AddressBookLocal addressBook, ContactsManager contactsManager)
            : this(contactInfo.Id, addressBook, contactsManager)
        {
            UpdateFrom(contactInfo, true);
        }

        public void UpdateFrom(IContactInfoLocal source, bool suppressChanged)
        {
            UpdateFrom((IContactInfo)source, suppressChanged);

            Contact = source.Contact;
        }

        public void UpdateFrom(IContactInfo source, bool suppressIsChanged)
        {
            FirstName = source.FirstName;
            MiddleName = source.MiddleName;
            LastName = source.LastName;
            Company = source.Company;
            JobTitle = source.JobTitle;
            Note = source.Note;
            Key = source.Key;
            VersionKey = source.VersionKey == null ? String.Empty : source.VersionKey.ToString();

            // Phone numbers
            foreach (var item in source.PhoneNumbers)
            {
                var phoneNumber = PhoneNumbers.FirstOrDefault(x => x.Key == item.Key);
                if (phoneNumber != null) phoneNumber.UpdateFrom(item);
                else PhoneNumbers.Add(new ContactPhoneLocal(item, AddressBook));
            }
            foreach (var item in PhoneNumbers.Where(x => source.PhoneNumbers.All(y => x.Key != y.Key)).ToArray())
                PhoneNumbers.Remove(item);

            // Emails
            foreach (var item in source.Emails)
            {
                var email = Emails.FirstOrDefault(x => x.Key == item.Key);
                if (email != null) email.UpdateFrom(item);
                else Emails.Add(new ContactEmailLocal(item, AddressBook));
            }
            foreach (var item in Emails.Where(x => source.Emails.All(y => x.Key != y.Key)).ToArray())
                Emails.Remove(item);

            // Tags
            foreach (var item in source.Tags)
            {
                var tag = Tags.FirstOrDefault(x => x.Key == item.Key);
                if (tag != null) tag.UpdateFrom(item);
                else
                {
                    var newTag = _contactsManager.Tags.FirstOrDefault(x => x.Key == item.Key);
                    if (newTag != null) Tags.Add(newTag);
                }
            }
            foreach (var item in Tags.Where(x => source.Tags.All(y => x.Key != y.Key)).ToArray())
                Tags.Remove(item);

            if (suppressIsChanged) IsChanged = false;
        }

        public void Submit()
        {
            Submit(true);
        }

        internal void Submit(bool repairDeleted)
        {
            if (!IsChanged && Id > 0) return;

            IncrementVersion();

            AddressBook.InsertOrUpdateContactInfo(this);

            _contactsManager.InsertOrUpdateContactInfo(this, repairDeleted);

            if (Changed != null) Changed.Invoke(this);
        }

        internal void RaiseChanged()
        {
            if (Changed != null) Changed.Invoke(this);
        }

        public void Remove()
        {
            AddressBook.RemoveContact(this);

            _contactsManager.RemoveContactInfo(this);
        }

        void InternalCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            IsChanged = true;
        }
    }
}
