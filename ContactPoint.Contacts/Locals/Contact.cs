using System;
using System.Collections.Generic;
using System.Linq;
using ContactPoint.Common.Contacts;
using ContactPoint.Common.Contacts.Local;

namespace ContactPoint.Contacts.Locals
{
    internal class Contact : IContact
    {
        private readonly List<ContactInfoLocal> _contactInfos = new List<ContactInfoLocal>();
        private string _firstName = String.Empty;
        private string _lastName = String.Empty;
        private string _company = String.Empty;
        private string _middleName = String.Empty;

        public event Action<IContact> Changed;

        public ContactsManager ContactsManager { get; private set; }
        public long Id { get; set; }

        public string FirstName
        {
            get { return _firstName; }
            set { _firstName = value; }
        }

        public string LastName
        {
            get { return _lastName; }
            set { _lastName = value; }
        }

        public string MiddleName
        {
            get { return _middleName; }
            set { _middleName = value; }
        }

        public string Company
        {
            get { return _company; }
            set { _company = value; }
        }

        public string ShowedName 
        { 
            get
            {
                var result = String.Empty;

                if (!string.IsNullOrEmpty(LastName)) result = LastName;
                if (!string.IsNullOrEmpty(FirstName)) result += string.Format("{1}{0}", FirstName, result.Length > 0 ? ", " : "");
                if (!string.IsNullOrEmpty(MiddleName)) result += string.Format("{1}{0}", MiddleName, result.Length > 0 ? " " : "");
                if (!string.IsNullOrEmpty(Company)) result += string.Format("{1}[{0}]", Company, result.Length > 0 ? " " : "");

                return result;
            }
        }

        IEnumerable<IContactInfoLocal> IContact.ContactInfos
        {
            get { return _contactInfos.AsEnumerable<IContactInfoLocal>(); }
        }

        internal List<ContactInfoLocal> ContactInfos
        {
            get { return _contactInfos; }
        }

        internal Contact(ContactsManager contactsManager)
            : this(contactsManager, -1)
        { }

        public Contact(ContactsManager contactsManager, int id)
        {
            ContactsManager = contactsManager;

            Id = id;
        }

        public void LinkContactInfo(IContactInfoLocal contact)
        {
            if (_contactInfos.Any(x => x.AddressBook.Id == contact.AddressBook.Id)) throw new InvalidOperationException("Contact info already linked.");

            var addressBook = ContactsManager.AddressBooks.FirstOrDefault(x => x.Id == contact.AddressBook.Id);
            if (addressBook == null) throw new InvalidOperationException("Address book is not registered.");

            var contactInfoLocal = contact as ContactInfoLocal ?? new ContactInfoLocal(contact, addressBook, ContactsManager);

            _contactInfos.Add(contactInfoLocal);
        }

        public void UnlinkContactInfo(IContactInfoLocal contactInfo)
        {
            var target = _contactInfos.FirstOrDefault(x => x.Id == contactInfo.Id);

            if (target != null)
                _contactInfos.Remove(target);
        }

        public void Submit()
        {
            Submit(true, true);
        }

        internal void Submit(bool submitItems, bool repairDeleted)
        {
            ContactsManager.InsertOrUpdateContact(this);

            if (submitItems)
                foreach (var item in ContactInfos)
                {
                    item.Submit(repairDeleted);
                    ContactsManager.AddLink(this, item);
                }

            RaiseChanged();
        }

        internal void RaiseChanged()
        {
            if (Changed != null) Changed.Invoke(this);
        }

        public void Remove()
        {
            foreach (var item in ContactInfos.ToArray())
                item.Remove();

            ContactsManager.RemoveContact(this);
        }

        public override bool Equals(object obj)
        {
            var contact = obj as Contact;
            if (contact == null) return base.Equals(obj);

            return contact.Id == Id;
        }
    }
}
