using System;
using ContactPoint.Common;
using ContactPoint.Common.Contacts;
using ContactPoint.Common.Contacts.Local;
using ContactPoint.Contacts.Fakes;

namespace ContactPoint.Contacts.Locals
{
    internal class AddressBookLocal : IAddressBookLocal
    {
        private readonly ContactsManager _contactsManager;
        private IAddressBook _instance;

        public long Id { get; set; }
        public int OrderNumber { get; set; }

        public bool IsOnline
        {
            get { return _instance != null && _instance.IsOnline; }
        }

        public Guid Key
        {
            get { return _instance.Key; }
        }

        public bool ReadOnly
        {
            get { return !IsOnline || _instance.ReadOnly; }
        }

        public string Name
        {
            get { return _instance.Name; }
        }

        public IVersionGenerator VersionGenerator
        {
            get { return _instance.VersionGenerator; }
        }

        public IContactInfoLocal CreateContactInfo()
        {
            try
            {
                return new ContactInfoLocal(_instance.CreateContact(), this, _contactsManager);
            }
            catch (Exception ex)
            {
                Logger.LogWarn(ex, "Can't create contact info for address book '{0}'.", Name);
            }

            return new ContactInfoLocal(this, _contactsManager);
        }

        public IContactPhoneLocal CreatePhoneNumber()
        {
            try
            {
                return new ContactPhoneLocal(_instance.CreatePhoneNumber(), this);
            }
            catch (Exception ex)
            {
                Logger.LogWarn(ex, "Can't create contact phone for address book '{0}'.", Name);
            }

            return new ContactPhoneLocal(this);
        }

        public IContactEmailLocal CreateEmail()
        {
            try
            {
                return new ContactEmailLocal(_instance.CreateEmail(), this);
            }
            catch (Exception ex)
            {
                Logger.LogWarn(ex, "Can't create contact phone for address book '{0}'.", Name);
            }

            return new ContactEmailLocal(this);
        }

        public IContactTagLocal CreateTag()
        {
            try
            {
                return new TagLocal(_instance.CreateTag(), this, _contactsManager);
            }
            catch (Exception ex)
            {
                Logger.LogWarn(ex, "Can't create contact phone for address book '{0}'.", Name);
            }

            return new TagLocal(this, _contactsManager);
        }

        public IAddressBook AddressBook
        {
            get { return _instance; }
        }

        public DateTime LastUpdate { get; set; }

        internal AddressBookLocal(ContactsManager contactsManager, long id, Guid key, string name)
            : this(contactsManager, id, new AddressBookFake(id, key, name))
        { }

        internal AddressBookLocal(ContactsManager contactsManager, long id, IAddressBook instance)
        {
            Id = id;

            _contactsManager = contactsManager;
            _instance = instance;
        }

        internal void UpdateInstance(IAddressBook instance)
        {
            _instance = instance;
        }

        internal void InsertOrUpdateContactInfo(ContactInfoLocal contactInfo)
        {
            if (ReadOnly) return;

            try
            {
                // Check if we receive newer object that have locally
                //var remote = _instance.GetContact(contactInfo.Key);
                //if (remote != null && contactInfo.IsUpdateNeeded(remote)) throw new NewerVersionException(contactInfo, remote);

                contactInfo.Key = _instance.InsertOrUpdateContact(contactInfo);
            }
            catch (Exception ex)
            {
                Logger.LogWarn(ex, "Can't insert ot update contact on address book '{0}'.", Name);
            }
        }

        internal void RemoveContact(ContactInfoLocal contact)
        {
            if (ReadOnly) return;

            try
            {
                _instance.RemoveContact(contact.Key);
            }
            catch (Exception ex)
            {
                Logger.LogWarn(ex, "Can't remove contact from address book '{0}'.", Name);
            }
        }

        internal void InsertOrUpdateTag(TagLocal tag)
        {
            if (ReadOnly) return;

            try
            {
                tag.Key = _instance.InsertOrUpdateTag(tag);
            }
            catch (Exception ex)
            {
                Logger.LogWarn(ex, "Can't insert ot update tag on address book '{0}'.", Name);
            }
        }

        internal void RemoveTag(IContactTagLocal tag)
        {
            if (ReadOnly) return;

            try
            {
                _instance.RemoveTag(tag.Key);
            }
            catch (Exception ex)
            {
                Logger.LogWarn(ex, "Can't remove tag from address book '{0}'.", Name);
            }
        }

        internal void Submit()
        {
            _contactsManager.UpdateAddressBook(this);
        }
    }

    public class NewerVersionException : Exception
    {
        private readonly IContactInfoLocal _localValue;
        private readonly IContactInfo _remoteValue;

        public IContactInfoLocal LocalValue
        {
            get { return _localValue; }
        }

        public IContactInfo RemoteValue
        {
            get { return _remoteValue; }
        }

        public NewerVersionException(IContactInfoLocal localValue, IContactInfo remoteValue)
        {
            _localValue = localValue;
            _remoteValue = remoteValue;
        }
    }
}
