using ContactPoint.Common.Contacts;
using ContactPoint.Plugins.LocalAddressBook.Contracts;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace ContactPoint.Plugins.LocalAddressBook
{
    [DataContract]
    internal class LocalAddressBook : IAddressBook
    {
        [IgnoreDataMember]
        private static Guid id = new Guid("{AA225D2C-4A92-480D-B6B6-EFDD765E5400}");

        [IgnoreDataMember]
        private LocalAddressBookPlugin _plugin;
        [DataMember]
        private readonly List<ContactInfo> _contacts = new List<ContactInfo>();
        [IgnoreDataMember]
        private readonly string _dbPath;
        [IgnoreDataMember]
        private readonly DataContractSerializer _serializer = new DataContractSerializer(typeof(List<ContactInfo>), new[] { typeof(ContactEmail), typeof(ContactPhone), typeof(ContactTag) });
        [DataMember]
        private readonly List<ContactTag> _tags = new List<ContactTag>();

        [IgnoreDataMember]
        public Guid Key
        {
            get { return id; }
        }

        [IgnoreDataMember]
        public bool ReadOnly
        {
            get { return false; }
        }

        [IgnoreDataMember]
        public string Name
        {
            get { return "Local address book"; }
        }

        [IgnoreDataMember]
        public IVersionGenerator VersionGenerator
        {
            get { return ContactPoint.Plugins.LocalAddressBook.VersionGenerator.Instance; }
        }

        [IgnoreDataMember]
        public bool IsOnline 
        {
            get { return true; }
        }

        public LocalAddressBook(LocalAddressBookPlugin plugin)
        {
            _plugin = plugin;

            _dbPath = string.Format("{0}{1}contactpoint{1}{2}", Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), Path.DirectorySeparatorChar, "local_address_book.xml");

            if (File.Exists(_dbPath))
            {
                using (var stream = File.OpenRead(_dbPath))
                {
                    _contacts.AddRange((IEnumerable<ContactInfo>)_serializer.ReadObject(stream));
                }
            }
        }

        public IEnumerable<IContactInfo> GetContacts()
        {
            return _contacts;
        }

        public IContactInfo GetContact(string key)
        {
            return _contacts.FirstOrDefault(x => x.Key == key);
        }

        public IContactInfo CreateContact()
        {
            return new ContactInfo();
        }

        public IContactPhone CreatePhoneNumber()
        {
            return new ContactPhone();
        }

        public IContactEmail CreateEmail()
        {
            return new ContactEmail();
        }

        public IEnumerable<IContactTag> GetTags()
        {
            return _tags;
        }

        public IContactTag GetTag(string key)
        {
            return _tags.FirstOrDefault(x => x.Key == key);
        }

        public IContactTag CreateTag()
        {
            return new ContactTag();
        }

        public string InsertOrUpdateContact(IContactInfo contact)
        {
            string result = contact.Key;
            lock (_contacts)
            {
                var target = _contacts.FirstOrDefault(x => x.Key == contact.Key);

                if (target == null)
                {
                    target = ContactInfo.GetLocalObject(contact);
                    target.Key = Guid.NewGuid().ToString("D");
                    result = target.Key;

                    _contacts.Add(target);
                }
                else
                {
                    _contacts.Remove(target);
                    _contacts.Add(ContactInfo.GetLocalObject(contact));
                }
            }

            FlushDb();

            return result;
        }

        public void RemoveContact(string key)
        {
            lock (_contacts)
            {
                var target = _contacts.FirstOrDefault(x => x.Key == key);

                _contacts.Remove(target);
            }

            FlushDb();
        }

        public string InsertOrUpdateTag(IContactTag tag)
        {
            string result = String.Empty;
            lock (_tags)
            {
                var target = _tags.FirstOrDefault(x => x.Key == tag.Key);

                if (target == null)
                {
                    target = ContactTag.GetLocalObject(tag);
                    result = target.Key = Guid.NewGuid().ToString("D");

                    _tags.Add(target);
                }
                else
                {
                    _tags.Remove(target);
                    _tags.Add(ContactTag.GetLocalObject(tag));
                }
            }

            FlushDb();

            return result;
        }

        public void RemoveTag(string key)
        {
            var target = _tags.FirstOrDefault(x => x.Key == key);
            if (target != null)
                _tags.Remove(target);
        }

        private void FlushDb()
        {
            lock (_contacts)
            {
                if (File.Exists(_dbPath)) File.Delete(_dbPath);

                using (var stream = File.OpenWrite(_dbPath))
                {
                    _serializer.WriteObject(stream, _contacts);
                }
            }
        }
    }
}
