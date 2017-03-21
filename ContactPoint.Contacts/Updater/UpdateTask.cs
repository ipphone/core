using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using ContactPoint.Common;
using ContactPoint.Common.Contacts;
using ContactPoint.Contacts.Locals;

namespace ContactPoint.Contacts.Updater
{
    internal class UpdateTask
    {
        private readonly ContactsManager _contactsManager;
        private readonly SQLiteConnection _sqlConnection;
        private readonly AddressBookLocal _addressBook;
        private readonly int _index;
        private List<IContactInfo> _items;
        private string _currentStateString;

        public DateTime StartTime { get; private set; }
        public string CurrentStateString
        {
            get { return _currentStateString; }
            private set 
            { 
                _currentStateString = value;
                StateChanged?.Invoke();
            }
        }

        public event Action<UpdateTask> Finished;
        public event Action StateChanged;

        public AddressBookLocal AddressBook
        {
            get { return _addressBook; }
        }

        public int Index
        {
            get { return _index; }
        }

        public bool IsNotifyingUser { get; set; }
        public bool IsFinished { get; private set; }

        public UpdateTask(ContactsManager contactsManager, SQLiteConnection sqlConnection, AddressBookLocal addressBook, int index)
        {
            StartTime = DateTime.Now;
            CurrentStateString = "Initializing...";

            _contactsManager = contactsManager;
            _sqlConnection = sqlConnection;
            _addressBook = addressBook;
            _index = index;
        }

        public bool Execute()
        {
            var result = ProcessTags();
            result |= ProcessContactsInternal();

            IsFinished = true;

            return result;
        }

        internal void RaiseFinished()
        {
            if (Finished != null) Finished.Invoke(this);
        }

        private bool ProcessContactsInternal()
        {
            Logger.LogNotice(String.Format("Loading contact info entities for '{0}'. Loading database links.", _addressBook.Name));
            CurrentStateString = "Loading contact info entities.";
            var contactInfosMappings = LoadContactInfosMappings(_addressBook);

            CurrentStateString = "Downloading contacts...";
            Logger.LogNotice(String.Format("Downloading contact info entities for '{0}'.", _addressBook.Name));
            _items = new List<IContactInfo>(_addressBook.AddressBook.GetContacts());
            Logger.LogNotice(String.Format("Download contact info entities for '{0}' finished. Total downloaded: {1}", _addressBook.Name, _items.Count));

            var updatedCount = 0;
            var skippedCount = 0;
            var createdCount = 0;
            var deletedCount = 0;
            var count = _items.Count;
            for (int i = 0; i < count; i++)
            {
                var item = _items[i];
                CurrentStateString = string.Format("Processing contact {0}/{1}", i, count);
                if (contactInfosMappings.ContainsKey(item.Key))
                {
                    foreach (var contact in contactInfosMappings[item.Key])
                    {
                        var contactInfoLocal = contact.ContactInfos.FirstOrDefault(x => x.Key.Equals(item.Key));
                        if (contactInfoLocal == null)
                        {
                            skippedCount++;
                            continue;
                        }

                        if (contactInfoLocal.VersionGenerator.CompareVersions(contactInfoLocal, item) != VersionsCompareResult.Lower)
                        {
                            skippedCount++;
                            continue;
                        }

                        contactInfoLocal.UpdateFrom(item, false);
                        _contactsManager.InsertOrUpdateContactInfo(contactInfoLocal, false);

                        contactInfoLocal.RaiseChanged();
                        contact.RaiseChanged();

                        updatedCount++;
                    }
                }
                else
                {
                    var contact = new Contact(_contactsManager)
                    {
                        FirstName = item.FirstName,
                        LastName = item.LastName,
                        MiddleName = item.MiddleName,
                        Company = item.Company
                    };

                    var contactInfoLocal = new ContactInfoLocal(item, _addressBook, _contactsManager);
                    contact.LinkContactInfo(contactInfoLocal);

                    _contactsManager.InsertOrUpdateContactInfo(contactInfoLocal, false);
                    if (!contactInfoLocal.IsDeleted)
                    {
                        contact.Submit(false, false);

                        createdCount++;
                    }
                    else
                        contact.RaiseChanged();

                    _contactsManager.AddLink(contact, contactInfoLocal);
                }
            }

            CurrentStateString = "Removing deleted contacts...";
            foreach (var item in contactInfosMappings.Where(x => _items.All(y => y.Key != x.Key)))
            {
                foreach (var contact in item.Value)
                {
                    var contactInfo =
                        contact.ContactInfos.FirstOrDefault(x => x.AddressBook.Id == AddressBook.Id && x.Key == item.Key);
                    if (contactInfo == null) continue;

                    contact.UnlinkContactInfo(contactInfo);
                    _contactsManager.RemoveContactInfo(contactInfo);

                    if (contact.ContactInfos.Count == 0) _contactsManager.RemoveContact(contact);
                    else contact.Submit(false, false);

                    deletedCount++;
                }
            }

            Logger.LogNotice(string.Format("Finished contact info entities update for '{0}'. Created: {1}, updated: {2}, skipped: {3} items.", _addressBook.Name, createdCount, updatedCount, skippedCount));
            return updatedCount > 0 || createdCount > 0 || deletedCount > 0;
        }

        private bool ProcessTags()
        {
            CurrentStateString = "Downloading tags...";
            Logger.LogNotice(String.Format("Downloading tags entities for '{0}'.", _addressBook.Name));
            var tags = _addressBook.AddressBook.GetTags().ToArray();
            Logger.LogNotice(String.Format("Download tags for '{0}' finished. Total downloaded: {1}", _addressBook.Name, tags.Length));

            CurrentStateString = "Processing tags...";
            
            var updatedCount = 0;
            var skippedCount = 0;
            var createdCount = 0;
            var count = tags.Length;
            for (int i = 0; i < count; i++)
            {
                var item = tags[i];
                CurrentStateString = string.Format("Processing tag {0}/{1}", i, count);

                var tagLocal = _contactsManager.TagsDictionary.Values.FirstOrDefault(x => x.Key == item.Key && x.AddressBook.Id == _addressBook.Id);
                var newTag = tagLocal == null;
                if (tagLocal == null)
                {
                    tagLocal = new TagLocal(_addressBook, _contactsManager);
                    createdCount++;
                }
                else if (tagLocal.VersionGenerator.CompareVersions(tagLocal, item) != VersionsCompareResult.Lower)
                {
                    skippedCount++;
                    continue;
                }

                if (tagLocal.Id > 0) updatedCount++;

                tagLocal.UpdateFrom(item);
                _contactsManager.InsertOrUpdateTag(tagLocal, false);

                if (newTag && tagLocal.IsDeleted) createdCount--;

                tagLocal.RaiseChanged();
            }

            Logger.LogNotice(string.Format("Finished tags update for '{0}'. Created: {1}, updated: {2}, skipped: {3} items.", _addressBook.Name, createdCount, updatedCount, skippedCount));
            return updatedCount > 0 || createdCount > 0;
        }

        private Dictionary<string, LinkedList<Contact>> LoadContactInfosMappings(AddressBookLocal addressBook)
        {
            var result = new Dictionary<string, LinkedList<Contact>>();

            using (new EnsuredResourceCriticalOperation(_sqlConnection))
            using (var command = _sqlConnection.CreateCommand())
            {
                command.CommandText = "select ci.`key`, cl.contact_id  from contact_infos ci inner join contacts_links cl on ci.id=cl.contact_info_id where addressbook_id = @addressbook_id";
                command.Parameters.Add(new SQLiteParameter("@addressbook_id", addressBook.Id));

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var key = reader.GetString(0);
                        var contactId = reader.GetInt32(1);

                        if (!_contactsManager.ContactsDictionary.ContainsKey(contactId)) continue;
                        var contact = _contactsManager.ContactsDictionary[contactId];

                        if (!result.ContainsKey(key))
                            result.Add(key, new LinkedList<Contact>());

                        result[key].AddFirst(contact);
                    }
                }
            }

            return result;
        }
    }
}
