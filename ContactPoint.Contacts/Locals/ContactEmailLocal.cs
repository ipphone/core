using System;
using ContactPoint.Common.Contacts;
using ContactPoint.Common.Contacts.Local;

namespace ContactPoint.Contacts.Locals
{
    internal class ContactEmailLocal : VersionableLocal, IContactEmailLocal
    {
        private string _comment = String.Empty;
        private string _email = String.Empty;

        public string Key { get; set; }
        public string Comment
        {
            get { return _comment; }
            set { IsChanged |= _comment != value; _comment = value; }
        }

        public string Email
        {
            get { return _email; }
            set { IsChanged |= _email != value; _email = value; }
        }

        public long Id { get; set; }

        public ContactEmailLocal(IAddressBookLocal addressBook)
            : base(addressBook)
        {
            Id = -1;
            Key = String.Empty;
        }

        public ContactEmailLocal(long id, IAddressBookLocal addressBook)
            : this(addressBook)
        {
            Id = id;
        }

        public ContactEmailLocal(long id, IContactEmail email, IAddressBookLocal addressBook)
            : this(id, addressBook)
        {
            Key = email.Key;

            UpdateFrom(email);
        }

        public ContactEmailLocal(IContactEmail email, IAddressBookLocal addressBook)
            : this(-1, email, addressBook)
        { }

        public void UpdateFrom(IContactEmail contactEmail)
        {
            Comment = contactEmail.Comment;
            Email = contactEmail.Email;
            VersionKey = contactEmail.VersionKey == null ? String.Empty : contactEmail.VersionKey.ToString();

            IsChanged = false;
        }

        public void UpdateKey(string key)
        {
            if (string.IsNullOrEmpty(Key)) Key = key;
        }
    }
}
