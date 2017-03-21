using System;
using System.Collections.Generic;
using ContactPoint.Common.Contacts;

namespace ContactPoint.Contacts.Fakes
{
    internal class AddressBookFake : IAddressBook
    {
        public long Id { get; set; }
        public Guid Key { get; private set; }
        public bool ReadOnly { get; private set; }
        public string Name { get; set; }
        public bool IsOnline { get; private set; }
        public IVersionGenerator VersionGenerator { get; private set; }

        public AddressBookFake(long id, Guid key, string name)
        {
            VersionGenerator = new VersionGeneratorFake();
            Id = id;
            Key = key;
            ReadOnly = true;
            Name = name;
            IsOnline = false;
        }

        public IEnumerable<IContactInfo> GetContacts()
        {
            yield break;
        }

        public IContactInfo GetContact(string key)
        {
            return null;
        }

        public IContactInfo CreateContact()
        {
            return null;
        }

        public IContactPhone CreatePhoneNumber()
        {
            return null;
        }

        public IContactEmail CreateEmail()
        {
            return null;
        }

        public IEnumerable<IContactTag> GetTags()
        {
            yield break;
        }

        public IContactTag GetTag(string key)
        {
            return null;
        }

        public IContactTag CreateTag()
        {
            return null;
        }

        public string InsertOrUpdateContact(IContactInfo contact)
        {
            return String.Empty;
        }

        public string InsertOrUpdateTag(IContactTag tag)
        {
            return String.Empty;
        }

        public void RemoveContact(string key)
        { }

        public void RemoveTag(string key)
        { }
    }
}
