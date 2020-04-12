using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using ContactPoint.Common;
using ContactPoint.Common.Contacts;
using ContactPoint.Common.Contacts.Local;
using ContactPoint.Contacts.Locals;
using ContactPoint.Contacts.Schemas;
using ContactPoint.Contacts.Updater;

namespace ContactPoint.Contacts
{
    public class ContactsManager : IContactsManager
    {
        private SQLiteConnection _sqlConnection;

        private readonly List<AddressBookLocal> _addressBooks = new List<AddressBookLocal>();
        private readonly Dictionary<long, Contact> _contacts = new Dictionary<long, Contact>();
        private readonly Dictionary<long, TagLocal> _tags = new Dictionary<long, TagLocal>();
        private readonly Dictionary<long, List<IContact>> _contactsTags = new Dictionary<long, List<IContact>>();
        private readonly ICollection<IContact> _contactsWrapper;
        private readonly IEnumerable<IContactTagLocal> _tagsWrapper;
        private readonly UpdateWatcher _updateWatcher;

        public event Action<IAddressBookLocal> AddressBookReloaded;

        public ICore Core { get; private set; }

        public IEnumerable<IContactTagLocal> Tags
        {
            get { return _tagsWrapper; }
        }

        internal IEnumerable<AddressBookLocal> AddressBooks
        {
            get { return _addressBooks; }
        }

        internal Dictionary<long, Contact> ContactsDictionary
        {
            get { return _contacts; }
        }

        internal Dictionary<long, TagLocal> TagsDictionary
        {
            get { return _tags; }
        }

        internal SQLiteConnection SqlConnection
        {
            get { return _sqlConnection; }
        }

        internal UpdateWatcher UpdateWatcher { get { return _updateWatcher; } } 

        public ICollection<IContact> Contacts
        {
            get { return _contactsWrapper; }
        }

        public ContactsManager(ICore core, ISynchronizeInvoke syncInvoke)
        {
            Core = core;

            _contactsWrapper = new ObservableDictionaryValuesMapperConverter<Contact, IContact>(_contacts);
            _tagsWrapper = new ObservableDictionaryValuesMapperConverter<TagLocal, IContactTagLocal>(_tags);

            var dbPath = string.Format("{0}{1}contactpoint{1}{2}", Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), Path.DirectorySeparatorChar, "address_book.s3db");

#if CONTACTS_DEBUG
            if (File.Exists(dbPath)) File.Delete(dbPath);
#endif

            if (!File.Exists(dbPath))
            {
                CreateDatabase(dbPath);
            }
            else
            {
                if (!InitializeConnection(dbPath))
                {
                    Logger.LogWarn("Error loading ContactsManager - continue without Contacts API");
                    return;
                }
            }

            var contacts = new Dictionary<long, Contact>();
            using (new EnsuredResourceCriticalOperation(_sqlConnection))
            {
                DatabaseSchema.Upgrade(_sqlConnection);

                #region Load address books

                Logger.LogNotice("Loading Address Books");
                using (var command = _sqlConnection.CreateCommand())
                {
                    command.CommandText = @"select id, name, lastupdate, key from addressbooks";

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            _addressBooks.Add(
                                new AddressBookLocal(
                                    this,
                                    reader.GetInt32(0),
                                    Guid.Parse(reader.GetString(3)),
                                    reader.GetString(1)
                                    )
                                    {
                                        LastUpdate =
                                            reader.IsDBNull(2)
                                                ? DateTime.Now - TimeSpan.FromDays(365)
                                                : reader.GetDateTime(2)
                                    });
                        }
                    }
                }

                #endregion

                #region Load tags

                Logger.LogNotice("Loading Tags");
                using (var command = _sqlConnection.CreateCommand())
                {
                    command.CommandText =
                        @"select id, name, key, color, version_tag, addressbook_id from tags where is_deleted = 0";

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var addressBookId = reader.GetInt64(5);
                            var addressBook = AddressBooks.FirstOrDefault(x => x.Id == addressBookId);
                            if (addressBook == null) continue;

                            var tag = new TagLocal(reader.GetInt64(0), addressBook, this)
                                {
                                    Name = reader.GetString(1),
                                    Key = reader.GetStringSafe(2),
                                    Color = reader.GetStringSafe(3),
                                    VersionKey = reader.GetStringSafe(4),
                                    IsDeleted = false
                                };

                            tag.ResetIsChanged();

                            _tags.Add(tag.Id, tag);
                        }
                    }
                }

                #endregion

                #region Load contacts

                Logger.LogNotice("Loading Contacts");
                using (var command = _sqlConnection.CreateCommand())
                {
                    command.CommandText = @"select id, first_name, last_name, middle_name, company from contacts";

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var id = reader.GetInt32(0);
                            var contact = new Contact(this, id)
                                {
                                    FirstName = reader.GetStringSafe(1),
                                    LastName = reader.GetStringSafe(2),
                                    MiddleName = reader.GetStringSafe(3),
                                    Company = reader.GetStringSafe(4)
                                };

                            contacts.Add(id, contact);
                        }
                    }
                }

                #endregion

                #region Load contact infos links for contacts

                Logger.LogNotice("Loading Contact Links");
                var contactInfoIdsContacts = new Dictionary<long, Contact>();
                using (var command = _sqlConnection.CreateCommand())
                {
                    command.CommandText = @"select contact_info_id, contact_id from contacts_links";

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var contactInfoId = reader.GetInt64(0);
                            if (contactInfoIdsContacts.ContainsKey(contactInfoId)) continue;

                            var contactId = reader.GetInt64(1);

                            if (!contacts.ContainsKey(contactId)) continue;

                            contactInfoIdsContacts.Add(contactInfoId, contacts[contactId]);
                        }
                    }
                }

                #endregion

                #region Load contact infos

                Logger.LogNotice("Loading Contact details");
                var contactInfos = new Dictionary<long, ContactInfoLocal>();
                using (var command = _sqlConnection.CreateCommand())
                {
                    command.CommandText =
                        @"select id, first_name, last_name, middle_name, company, job_title, addressbook_id, key, note, version_tag from contact_infos where is_deleted = 0";

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var id = reader.GetInt64(0);
                            var addressBookId = reader.GetInt64(6);
                            var addressBook = _addressBooks.FirstOrDefault(x => x.Id == addressBookId);

                            if (addressBook == null) continue;
                            if (!contactInfoIdsContacts.ContainsKey(id)) continue;

                            var contactInfo = new ContactInfoLocal(id, addressBook, this)
                                {
                                    FirstName = reader.GetStringSafe(1),
                                    LastName = reader.GetStringSafe(2),
                                    MiddleName = reader.GetStringSafe(3),
                                    Company = reader.GetStringSafe(4),
                                    JobTitle = reader.GetStringSafe(5),
                                    Key = reader.GetString(7),
                                    Note = reader.GetStringSafe(8),
                                    VersionKey = reader.GetStringSafe(9),
                                    Contact = contactInfoIdsContacts[id],
                                    IsDeleted = false
                                };

                            contactInfos.Add(id, contactInfo);
                            contactInfo.ResetIsChanged();

                            contactInfoIdsContacts[id].ContactInfos.Add(contactInfo);
                        }
                    }
                }

                #endregion

                #region Load phone numbers

                Logger.LogNotice("Loading Phone Numbers");
                using (var command = _sqlConnection.CreateCommand())
                {
                    command.CommandText =
                        @"select id, number, comment, key, version_tag, contact_info_id from contact_info_phones";

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var contactInfoId = reader.GetInt64(5);
                            if (!contactInfos.ContainsKey(contactInfoId)) continue;

                            var number = new ContactPhoneLocal(reader.GetInt64(0),
                                                               contactInfos[contactInfoId].AddressBook)
                                {
                                    Number = reader.GetString(1),
                                    Comment = reader.GetStringSafe(2),
                                    Key = reader.GetStringSafe(3),
                                    VersionKey = reader.GetStringSafe(4)
                                };

                            number.ResetIsChanged();

                            contactInfos[contactInfoId].PhoneNumbers.Add(number);
                            contactInfos[contactInfoId].ResetIsChanged();
                        }
                    }
                }

                #endregion

                #region Load emails

                Logger.LogNotice("Loading Emails");
                using (var command = _sqlConnection.CreateCommand())
                {
                    command.CommandText =
                        @"select id, email, comment, key, version_tag, contact_info_id from contact_info_emails";

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var contactInfoId = reader.GetInt64(5);
                            if (!contactInfos.ContainsKey(contactInfoId)) continue;

                            var email = new ContactEmailLocal(reader.GetInt64(0),
                                                              contactInfos[contactInfoId].AddressBook)
                                {
                                    Email = reader.GetString(1),
                                    Comment = reader.GetStringSafe(2),
                                    Key = reader.GetStringSafe(3),
                                    VersionKey = reader.GetStringSafe(4)
                                };

                            email.ResetIsChanged();

                            contactInfos[contactInfoId].Emails.Add(email);
                            contactInfos[contactInfoId].ResetIsChanged();
                        }
                    }
                }

                #endregion

                #region Fill tags links on contacts infos

                Logger.LogNotice("Loading Tag links");
                using (var command = _sqlConnection.CreateCommand())
                {
                    command.CommandText = @"select tag_id, contact_info_id from tags_links";

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var tagId = reader.GetInt32(0);
                            var contactInfoId = reader.GetInt32(1);

                            if (!_tags.ContainsKey(tagId)) continue;
                            if (!contactInfos.ContainsKey(contactInfoId)) continue;

                            contactInfos[contactInfoId].Tags.Add(_tags[tagId]);
                            contactInfos[contactInfoId].ResetIsChanged();

                            if (!_contactsTags.ContainsKey(tagId)) _contactsTags.Add(tagId, new List<IContact>());

                            _contactsTags[tagId].Add(contactInfoIdsContacts[contactInfoId]);
                        }
                    }
                }

                #endregion
            }
            
            // Fill contacts into main collection
            foreach (var item in contacts)
            {
                _contacts.Add(item.Key, item.Value);
            }

            Logger.LogNotice("Starting update watcher");
            _updateWatcher = new UpdateWatcher(this, syncInvoke);
        }

        public void RegisterAddressBook(IAddressBook addressBook)
        {
            var targetWrapper = _addressBooks.FirstOrDefault(x => x.Key == addressBook.Key);

            if (targetWrapper == null)
            {
                lock (_addressBooks)
                {
                    using (new EnsuredResourceCriticalOperation(_sqlConnection))
                    using (var command = _sqlConnection.CreateCommand())
                    {
                        command.CommandText = @"insert into addressbooks(key, name) values (@key, @name)";

                        command.Parameters.Add(new SQLiteParameter("key", addressBook.Key.ToString("D")));
                        command.Parameters.Add(new SQLiteParameter("name", addressBook.Name));

                        command.ExecuteNonQuery();
                    }

                    targetWrapper = new AddressBookLocal(this, GetLastInsertRowId(), addressBook);
                    _addressBooks.Add(targetWrapper);
                }
            }
            else
            {
                targetWrapper.UpdateInstance(addressBook);

                _updateWatcher.QueueCheck(targetWrapper);
            }
        }

        internal void UpdateAddressBook(AddressBookLocal addressBook)
        {
            using (new EnsuredResourceCriticalOperation(_sqlConnection))
            using (var command = _sqlConnection.CreateCommand())
            {
                command.CommandText = @"update addressbooks set name = @name, lastupdate = CURRENT_TIMESTAMP where id = @id";

                command.Parameters.Add(new SQLiteParameter("name", addressBook.Name));
                command.Parameters.Add(new SQLiteParameter("@id", addressBook.Id));

                command.ExecuteNonQuery();
            }
        }

        private long GetLastInsertRowId()
        {
            using (var command = _sqlConnection.CreateCommand())
            {
                command.CommandText = @"select last_insert_rowid()";

                return (long) command.ExecuteScalar();
            }
        }

        private void CreateDatabase(string path)
        {
            Logger.LogNotice($"Creating Database at '{path}'");
            SQLiteConnection.CreateFile(path);

            if (!InitializeConnection(path))
            {
                Logger.LogWarn("Error loading ContactsManager - continue without Contacts API");
                return;
            }

            using (new EnsuredResourceCriticalOperation(_sqlConnection))
            {
                InitialSchema.Create(_sqlConnection);
            }
        }

        private bool InitializeConnection(string path)
        {
            Logger.LogNotice($"Initializing SQLiteConnection to '{path}'");
            try
            {
                _sqlConnection = new SQLiteConnection($"Data Source={path};Version=3;");
                _sqlConnection.Open();

                return true;
            }
            catch (Exception e)
            {
                Logger.LogError(e);
            }

            return false;
        }

        public void Dispose()
        {
            _updateWatcher.Dispose();
            _sqlConnection.Close();
        }

        public IContact CreateContact()
        {
            return new Contact(this);
        }

        internal void RaiseAddressBookReloaded(AddressBookLocal addressBook)
        {
            if (AddressBookReloaded != null) AddressBookReloaded(addressBook);
        }

        public void AddLink(IContact contact, IContactInfoLocal contactInfoLocal)
        {
            if (contactInfoLocal.Id < 0 || contact.Id < 0) return;

            using (new EnsuredResourceCriticalOperation(_sqlConnection))
            using (var command = _sqlConnection.CreateCommand())
            {
                command.CommandText =
                    "select count(*) from contacts_links where contact_id = @contact_id and contact_info_id = @contact_info_id";

                command.Parameters.Add(new SQLiteParameter("@contact_id", contact.Id));
                command.Parameters.Add(new SQLiteParameter("@contact_info_id", contactInfoLocal.Id));

                if ((long) command.ExecuteScalar() > 0) return;

                command.CommandText =
                    "insert into contacts_links(contact_id, contact_info_id) values (@contact_id, @contact_info_id)";

                command.ExecuteNonQuery();
            }
        }

        internal void InsertOrUpdateContact(Contact contact)
        {
            if (contact == null) return;

            using (new EnsuredResourceCriticalOperation(_sqlConnection))
            using (var command = _sqlConnection.CreateCommand())
            {
                if (contact.Id > 0)
                {
                    command.CommandText =
                        "update contacts set first_name = @first_name, last_name = @last_name, middle_name = @middle_name, company = @company where id = @id";

                    command.Parameters.Add(new SQLiteParameter("@id", contact.Id));
                }
                else
                {
                    command.CommandText =
                        "insert into contacts(first_name, last_name, middle_name, company) values (@first_name, @last_name, @middle_name, @company)";
                }

                command.Parameters.Add(new SQLiteParameter("@first_name", contact.FirstName));
                command.Parameters.Add(new SQLiteParameter("@last_name", contact.LastName));
                command.Parameters.Add(new SQLiteParameter("@middle_name", contact.MiddleName));
                command.Parameters.Add(new SQLiteParameter("@company", contact.Company));

                command.ExecuteNonQuery();

                if (contact.Id <= 0)
                {
                    contact.Id = GetLastInsertRowId();

                    _contacts.Add(contact.Id, contact);
                }
            }
        }
        
        internal void RemoveContact(Contact contact)
        {
            if (contact.Id <= 0) return;

            using (new EnsuredResourceCriticalOperation(_sqlConnection))
            using (var command = _sqlConnection.CreateCommand())
            {
                command.CommandText = @"delete from contacts where id = @id;";
                command.Parameters.Add(new SQLiteParameter("@id", contact.Id));

                command.ExecuteNonQuery();
            }

            if (_contacts.ContainsKey(contact.Id)) _contacts.Remove(contact.Id);
        }

        internal void InsertOrUpdateTag(TagLocal tag, bool repairDeleted)
        {
            if (tag == null) return;

            if (tag.Id <= 0)
            {
                var deletedTagId = GetRemovedTagId(tag.AddressBook.Id, tag.Key);
                if (deletedTagId > 0)
                {
                    tag.Id = deletedTagId;
                    tag.IsDeleted = !repairDeleted;
                }
            }

            using (new EnsuredResourceCriticalOperation(_sqlConnection))
            using (var command = _sqlConnection.CreateCommand())
            {
                if (tag.Id > 0)
                {
                    command.CommandText = "update tags set [name] = @name, color = @color, is_deleted = @is_deleted where id = @id";

                    command.Parameters.Add(new SQLiteParameter("@id", tag.Id));
                    command.Parameters.Add(new SQLiteParameter("@is_deleted", tag.IsDeleted ? 1 : 0));
                }
                else
                {
                    tag.IncrementVersion();

                    command.CommandText = "insert into tags([name], color, addressbook_id, [key], version_tag) values (@name, @color, @addressbook_id, @key, @version_tag)";

                    command.Parameters.Add(new SQLiteParameter("@addressbook_id", tag.AddressBook.Id));
                    command.Parameters.Add(new SQLiteParameter("@key", tag.Key));
                    command.Parameters.Add(new SQLiteParameter("@version_tag", tag.VersionKey));
                }

                command.Parameters.Add(new SQLiteParameter("@name", tag.Name));
                command.Parameters.Add(new SQLiteParameter("@color", tag.Color));

                command.ExecuteNonQuery();

                if (tag.Id <= 0)
                {
                    tag.Id = GetLastInsertRowId();

                    if (!tag.IsDeleted)
                        _tags.Add(tag.Id, tag);
                }
            }
        }

        private long GetRemovedTagId(long addressbookId, string key)
        {
            using (new EnsuredResourceCriticalOperation(_sqlConnection))
            using (var command = _sqlConnection.CreateCommand())
            {
                command.CommandText = "select id from tags where is_deleted = 1 and addressbook_id = @addressbook_id and key = @key";
                command.Parameters.Add(new SQLiteParameter("@addressbook_id", addressbookId));
                command.Parameters.Add(new SQLiteParameter("@key", key));

                try
                {
                    return (long) command.ExecuteScalar();
                }
                catch
                {
                    return -1;
                }
            }
        }

        private long GetRemovedContactInfoId(long addressbookId, string key)
        {
            using (new EnsuredResourceCriticalOperation(_sqlConnection))
            using (var command = _sqlConnection.CreateCommand())
            {
                command.CommandText = "select id from contact_infos where is_deleted = 1 and addressbook_id = @addressbook_id and key = @key";
                command.Parameters.Add(new SQLiteParameter("@addressbook_id", addressbookId));
                command.Parameters.Add(new SQLiteParameter("@key", key));

                try
                {
                    return (long)command.ExecuteScalar();
                }
                catch
                {
                    return -1;
                }
            }
        }

        internal void RemoveTag(TagLocal tag)
        {
            if (tag.Id <= 0) return;

            using (new EnsuredResourceCriticalOperation(_sqlConnection))
            using (var command = _sqlConnection.CreateCommand())
            {
                command.CommandText = @"
delete from tags_links where tag_id = @id;
update tags set is_deleted = 1 where id = @id";
                command.Parameters.Add(new SQLiteParameter("@id", tag.Id));

                command.ExecuteNonQuery();
            }

            if (_contactsTags.ContainsKey(tag.Id))
            {
                foreach (var contact in _contactsTags[tag.Id])
                    foreach (var contactInfo in contact.ContactInfos)
                    {
                        var target = contactInfo.Tags.FirstOrDefault(x => x.Id == tag.Id);

                        if (target != null)
                        {
                            contactInfo.Tags.Remove(target);
                            contactInfo.Submit();
                        }
                    }

                _contactsTags.Remove(tag.Id);
            }
        }

        public IContact GetContactByPhoneNumber(string phoneNumber)
        {
            var result = GetContactByPhoneNumberExact(phoneNumber);
            if (result != null) return result;

            if (phoneNumber.Length > 7 && phoneNumber.Length <= 10)
            {
                result = GetContactByPhoneNumberExact(phoneNumber.Substring(phoneNumber.Length - 7));
                if (result != null) return result;
            }
            else if (phoneNumber.Length > 10)
            {
                result = GetContactByPhoneNumberExact(phoneNumber.Substring(phoneNumber.Length - 10));
                if (result != null) return result;
            }

            return null;
        }

        private IContact GetContactByPhoneNumberExact(string phoneNumber)
        {
            IContact result;
            if (phoneNumber.Length > 4)
            {
                result = GetContactByPhoneNumberFromDb(phoneNumber);
                if (result != null) return result;
            }

            result = _contacts.Where(x => x.Value.ContactInfos.Any(y => y.PhoneNumbers.Any(z => string.Equals(z.Number, phoneNumber, StringComparison.InvariantCultureIgnoreCase)))).Select(x => x.Value).FirstOrDefault();
            if (result != null) return result;

            return null;
        }

        internal IContact GetContactByPhoneNumberFromDb(string phoneNumber)
        {
            using (new EnsuredResourceCriticalOperation(_sqlConnection))
            using (var command = _sqlConnection.CreateCommand())
            {
                command.CommandText = string.Format(@"select l.contact_id
from contact_info_phones p 
left join contacts_links l on l.contact_info_id = p.contact_info_id
where p.is_deleted = 0 and p.number like '%{0}'", phoneNumber);

                try
                {
                    var result = (long) command.ExecuteScalar();

                    return _contacts[result];
                }
                catch
                {
                    return null;
                }
            }
        }

        internal void InsertOrUpdateContactInfo(ContactInfoLocal contactInfo, bool repairDeleted)
        {
            if (contactInfo.Id <= 0)
            {
                var deletedContactInfoId = GetRemovedContactInfoId(contactInfo.AddressBook.Id, contactInfo.Key);
                if (deletedContactInfoId > 0)
                {
                    contactInfo.Id = deletedContactInfoId;
                    contactInfo.IsDeleted = !repairDeleted;
                }
            }

            if (contactInfo.Id > 0)
            {
                using (new EnsuredResourceCriticalOperation(_sqlConnection))
                using (var command = _sqlConnection.CreateCommand())
                {
                    var phoneNumberIds = new List<long>(contactInfo.PhoneNumbers.Where(x => x.Id > 0).Select(x => x.Id));
                    var emailIds = new List<long>(contactInfo.Emails.Where(x => x.Id > 0).Select(x => x.Id));

                    if (phoneNumberIds.Count > 0)
                    {
                        command.CommandText +=
                            string.Format(
                                @"delete from contact_info_phones where id not in ({0}) and contact_info_id = @id;",
                                string.Join(",", phoneNumberIds.Select(x => "@p" + x)));
                        command.Parameters.AddRange(
                            phoneNumberIds.Select(x => new SQLiteParameter("@p" + x, x)).ToArray());
                    }
                    if (emailIds.Count > 0)
                    {
                        command.CommandText +=
                            string.Format(
                                @"delete from contact_info_emails where id not in ({0}) and contact_info_id = @id;",
                                string.Join(",", emailIds.Select(x => "@e" + x)));
                        command.Parameters.AddRange(emailIds.Select(x => new SQLiteParameter("@e" + x, x)).ToArray());
                    }

                    if (command.Parameters.Count > 0)
                    {
                        command.Parameters.Add(new SQLiteParameter("@id", contactInfo.Id));

                        command.ExecuteNonQuery();
                    }
                }
            }

            using (new EnsuredResourceCriticalOperation(_sqlConnection))
            using (var command = _sqlConnection.CreateCommand())
            {
                if (contactInfo.Id > 0)
                {
                    command.CommandText =
                        "update contact_infos set first_name = @first_name, last_name = @last_name, middle_name = @middle_name, company = @company, job_title = @job_title, note = @note, version_tag = @version_tag, is_deleted = @is_deleted where id = @id";

                    command.Parameters.Add(new SQLiteParameter("@id", contactInfo.Id));
                    command.Parameters.Add(new SQLiteParameter("@is_deleted", contactInfo.IsDeleted ? 1 : 0));
                }
                else
                {
                    command.CommandText =
                        "insert into contact_infos(first_name, last_name, middle_name, company, job_title, note, addressbook_id, key, version_tag) values (@first_name, @last_name, @middle_name, @company, @job_title, @note, @addressbook_id, @key, @version_tag)";

                    command.Parameters.Add(new SQLiteParameter("@addressbook_id", contactInfo.AddressBook.Id));
                    command.Parameters.Add(new SQLiteParameter("@key", contactInfo.Key));
                }

                command.Parameters.Add(new SQLiteParameter("@first_name", contactInfo.FirstName));
                command.Parameters.Add(new SQLiteParameter("@last_name", contactInfo.LastName));
                command.Parameters.Add(new SQLiteParameter("@middle_name", contactInfo.MiddleName));
                command.Parameters.Add(new SQLiteParameter("@company", contactInfo.Company));
                command.Parameters.Add(new SQLiteParameter("@job_title", contactInfo.JobTitle));
                command.Parameters.Add(new SQLiteParameter("@note", contactInfo.Note));
                command.Parameters.Add(new SQLiteParameter("@version_tag", contactInfo.VersionKey.ToString()));

                command.ExecuteNonQuery();

                if (contactInfo.Id <= 0)
                    contactInfo.Id = GetLastInsertRowId();
            }

            if (contactInfo.Id > 0)
            {
                foreach (var item in contactInfo.PhoneNumbers.Where(x => x.IsChanged || x.Id <= 0))
                {
                    item.IncrementVersion();

                    using (new EnsuredResourceCriticalOperation(_sqlConnection))
                    using (var command = _sqlConnection.CreateCommand())
                    {
                        item.IncrementVersion();

                        if (item.Id > 0)
                        {
                            command.CommandText =
                                "update contact_info_phones set [number] = @number, [comment] = @comment, version_tag = @version_tag where id = @id";

                            command.Parameters.Add(new SQLiteParameter("@id", item.Id));
                        }
                        else
                        {
                            command.CommandText =
                                "insert into contact_info_phones([number], [comment], key, contact_info_id, version_tag) values (@number, @comment, @key, @contact_info_id, @version_tag)";

                            command.Parameters.Add(new SQLiteParameter("@contact_info_id", contactInfo.Id));
                            command.Parameters.Add(new SQLiteParameter("@key", item.Key));
                        }

                        command.Parameters.Add(new SQLiteParameter("@number", item.Number));
                        command.Parameters.Add(new SQLiteParameter("@comment", item.Comment));
                        command.Parameters.Add(new SQLiteParameter("@version_tag", item.VersionKey.ToString()));

                        command.ExecuteNonQuery();

                        if (item.Id <= 0 && item is ContactPhoneLocal)
                            ((ContactPhoneLocal) item).Id = GetLastInsertRowId();
                    }
                }

                foreach (var item in contactInfo.Emails.Where(x => x.IsChanged || x.Id <= 0))
                {
                    using (new EnsuredResourceCriticalOperation(_sqlConnection))
                    using (var command = _sqlConnection.CreateCommand())
                    {
                        item.IncrementVersion();

                        if (item.Id > 0)
                        {
                            command.CommandText =
                                "update contact_info_emails set [email] = @email, [comment] = @comment, version_tag = @version_tag where id = @id";

                            command.Parameters.Add(new SQLiteParameter("@id", item.Id));
                        }
                        else
                        {
                            command.CommandText =
                                "insert into contact_info_emails([email], [comment], key, contact_info_id, version_tag) values (@email, @comment, @key, @contact_info_id, @version_tag)";

                            command.Parameters.Add(new SQLiteParameter("@contact_info_id", contactInfo.Id));
                            command.Parameters.Add(new SQLiteParameter("@key", item.Key));
                        }

                        command.Parameters.Add(new SQLiteParameter("@email", item.Email));
                        command.Parameters.Add(new SQLiteParameter("@comment", item.Comment));
                        command.Parameters.Add(new SQLiteParameter("@version_tag", item.VersionKey.ToString()));

                        command.ExecuteNonQuery();

                        if (item.Id <= 0 && item is ContactEmailLocal)
                            ((ContactEmailLocal) item).Id = GetLastInsertRowId();
                    }
                }

                using (new EnsuredResourceCriticalOperation(_sqlConnection))
                using (var command = _sqlConnection.CreateCommand())
                {
                    command.CommandText = "delete from tags_links where contact_info_id = @contact_info_id;";
                    command.Parameters.Add(new SQLiteParameter("@contact_info_id", contactInfo.Id));

                    foreach (var item in contactInfo.Tags.Where(x => x.Id > 0))
                    {
                        command.CommandText += string.Format("insert into tags_links(tag_id, contact_info_id) values (@tag_id_{0}, @contact_info_id); ", item.Id);
                        command.Parameters.Add(new SQLiteParameter(string.Format("@tag_id_{0}", item.Id), item.Id));
                    }

                    command.ExecuteNonQuery();
                }
            }
        }

        internal void RemoveContactInfo(ContactInfoLocal contactInfo)
        {
            if (contactInfo.Id <= 0) return;

            using (new EnsuredResourceCriticalOperation(_sqlConnection))
            using (var command = _sqlConnection.CreateCommand())
            {
                command.CommandText = @"
delete from contact_info_phones where contact_info_id = @id;
delete from contact_info_emails where contact_info_id = @id;
delete from tags_links where contact_info_id = @id;
delete from contacts_links where contact_info_id = @id;
update contact_infos set is_deleted = 1 where id = @id;";
                command.Parameters.Add(new SQLiteParameter("@id", contactInfo.Id));

                command.ExecuteNonQuery();
            }

            if (contactInfo.Contact != null) contactInfo.Contact.UnlinkContactInfo(contactInfo);

            foreach (var tag in contactInfo.Tags)
            {
                if (!_contactsTags.ContainsKey(tag.Id)) continue;

                var target = _contactsTags[tag.Id].FirstOrDefault(x => x.Id == contactInfo.Id);

                if (target != null)
                    _contactsTags[tag.Id].Remove(target);
            }
        }

        IEnumerable<IAddressBookLocal> IContactsManager.AddressBooks
        {
            get { return _addressBooks; }
        }
    }
}
