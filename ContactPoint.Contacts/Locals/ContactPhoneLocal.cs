using System;
using ContactPoint.Common.Contacts;
using ContactPoint.Common.Contacts.Local;

namespace ContactPoint.Contacts.Locals
{
    internal class ContactPhoneLocal : VersionableLocal, IContactPhoneLocal
    {
        private string _comment = String.Empty;
        private string _number = String.Empty;

        public string Key { get; set; }

        public string Comment
        {
            get { return _comment; }
            set { IsChanged |= _comment != value; _comment = value; }
        }

        public string Number
        {
            get { return _number; }
            set { IsChanged |= _number != value; _number = value; }
        }

        public long Id { get; set; }

        public ContactPhoneLocal(IAddressBookLocal addressBook)
            : this(-1, addressBook)
        { }

        public ContactPhoneLocal(long id, IAddressBookLocal addressBook)
            : base(addressBook)
        {
            Id = id;
            Key = String.Empty;
        }

        public ContactPhoneLocal(long id, IContactPhone contactPhone, IAddressBookLocal addressBook)
            : this(id, addressBook)
        {
            Key = contactPhone.Key;

            UpdateFrom(contactPhone);
        }

        public ContactPhoneLocal(IContactPhone contactPhone, IAddressBookLocal addressBook)
            : this(-1, contactPhone, addressBook)
        { }

        public void UpdateFrom(IContactPhone phoneNumber)
        {
            Comment = phoneNumber.Comment;
            Number = phoneNumber.Number;
            VersionKey = phoneNumber.VersionKey == null ? String.Empty : phoneNumber.VersionKey.ToString();

            IsChanged = false;
        }

        public void UpdateKey(string key)
        {
            if (string.IsNullOrEmpty(Key)) Key = key;
        }
    }
}
