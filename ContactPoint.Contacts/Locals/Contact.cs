using System;
using System.Collections.Generic;
using System.Linq;
using ContactPoint.Common.Contacts;
using ContactPoint.Common.Contacts.Local;

namespace ContactPoint.Contacts.Locals
{
    internal class Contact : IContact
    {
        public event Action<IContact> Changed;

        public ContactsManager ContactsManager { get; private set; }
        public long Id { get; set; }

        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string MiddleName { get; set; } = string.Empty;
        public string Company { get; set; } = string.Empty;

        public string ShowedName 
        { 
            get
            {
                var result = string.Empty;

                if (!string.IsNullOrEmpty(LastName)) result = LastName;
                if (!string.IsNullOrEmpty(FirstName)) result += string.Format("{1}{0}", FirstName, result.Length > 0 ? ", " : "");
                if (!string.IsNullOrEmpty(MiddleName)) result += string.Format("{1}{0}", MiddleName, result.Length > 0 ? " " : "");
                if (!string.IsNullOrEmpty(Company)) result += string.Format("{1}[{0}]", Company, result.Length > 0 ? " " : "");

                return result;
            }
        }

        IEnumerable<IContactInfoLocal> IContact.ContactInfos
        {
            get { return ContactInfos.AsEnumerable<IContactInfoLocal>(); }
        }

        internal List<ContactInfoLocal> ContactInfos { get; } = new List<ContactInfoLocal>();

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
            if (ContactInfos.Any(x => x.AddressBook.Id == contact.AddressBook.Id)) throw new InvalidOperationException("Contact info already linked.");

            var addressBook = ContactsManager.AddressBooks.FirstOrDefault(x => x.Id == contact.AddressBook.Id);
            if (addressBook == null) throw new InvalidOperationException("Address book is not registered.");

            var contactInfoLocal = contact as ContactInfoLocal ?? new ContactInfoLocal(contact, addressBook, ContactsManager);

            ContactInfos.Add(contactInfoLocal);
        }

        public void UnlinkContactInfo(IContactInfoLocal contactInfo)
        {
            var target = ContactInfos.FirstOrDefault(x => x.Id == contactInfo.Id);

            if (target != null)
                ContactInfos.Remove(target);
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

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}
