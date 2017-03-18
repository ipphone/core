using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ContactPoint.Common.Contacts;
using MySql.Data.MySqlClient;

namespace ContactPoint.Plugins.MySqlAddressBook.AddressBook
{
    internal class AddressBookService : IAddressBook
    {
        private readonly Guid _key = Guid.Parse("{BDCBB1F5-B35A-40A2-86FC-31B0ED8E53BA}");
        private readonly MySqlAddressBookPlugin _plugin;

        public Guid Key { get { return _key; } }
        public bool ReadOnly { get { return false; } }
        public string Name { get { return "MySQL shared"; } }
        public bool IsOnline { get; set; }
        public IVersionGenerator VersionGenerator { get; private set; }

        public AddressBookService(MySqlAddressBookPlugin plugin)
        {
            _plugin = plugin;
        }

        public IEnumerable<IContactInfo> GetContacts()
        {
            using (var connection = _plugin.ConnectionManager.OpenConnection())
            {
                var contacts = new Dictionary<int, ContactInfo>();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "select id, first_name, last_name, middle_name, company, job_title, note, version_tag from contacts";

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var contactId = reader.GetInt32(0);
                            if (contacts.ContainsKey(contactId)) continue;

                            contacts.Add(contactId, new ContactInfo
                                {
                                    Id = contactId,
                                    FirstName = reader.GetStringSafe(1),
                                    LastName = reader.GetStringSafe(2),
                                    MiddleName = reader.GetStringSafe(3),
                                    Company = reader.GetStringSafe(4),
                                    JobTitle = reader.GetStringSafe(5),
                                    Note = reader.GetStringSafe(6),
                                    VersionKey = reader.GetInt32(7)
                                });
                        }
                    }
                }

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "select id, contact_id, `number`,  from contact_phones";
                }
            }

            return null;
        }

        public IContactInfo GetContact(string key)
        {
            throw new NotImplementedException();
        }

        public IContactInfo CreateContact()
        {
            throw new NotImplementedException();
        }

        public string InsertOrUpdateContact(IContactInfo contact)
        {
            throw new NotImplementedException();
        }

        public void RemoveContact(string key)
        {
            throw new NotImplementedException();
        }

        public IContactPhone CreatePhoneNumber()
        {
            throw new NotImplementedException();
        }

        public IContactEmail CreateEmail()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IContactTag> GetTags()
        {
            throw new NotImplementedException();
        }

        public IContactTag GetTag(string key)
        {
            throw new NotImplementedException();
        }

        public IContactTag CreateTag()
        {
            throw new NotImplementedException();
        }

        public string InsertOrUpdateTag(IContactTag tag)
        {
            throw new NotImplementedException();
        }

        public void RemoveTag(string key)
        {
            throw new NotImplementedException();
        }
    }
}
