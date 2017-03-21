using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ContactPoint.Common;
using ContactPoint.Common.Contacts;
using ContactPoint.Plugins.GoogleContacts.Model;
using Google.GData.Client;
using Google.GData.Contacts;

namespace ContactPoint.Plugins.GoogleContacts
{
    internal class GoogleAddressBook : IAddressBook
    {
        private GoogleContactsPlugin _plugin;
        private ContactsService _googleService;
        private static readonly Guid _key = Guid.Parse("{7761EB6D-2491-40CF-9FA6-E5213374E0F8}");
        private DateTime _lastSyncTime;

        public Guid Key { get { return _key; } }
        public bool ReadOnly { get { return false; } }
        public string Name { get { return "Google"; } }
        public bool IsOnline { get; private set; }
        public IVersionGenerator VersionGenerator { get { return Model.VersionGenerator.Instance; } }

        public GoogleAddressBook(GoogleContactsPlugin plugin)
        {
            _plugin = plugin;
            _lastSyncTime = plugin.PluginManager.Core.SettingsManager.GetValueOrSetDefault("LastSync", DateTime.MinValue);
            _googleService = new ContactsService("IP Phone");
            _googleService.Credentials = new GDataCredentials("alexander@artpoint.com.ua", "123qwe"); // TODO: Use client token. Generate it in settings form and store.

            IsOnline = true;
        }

        public IEnumerable<IContactInfo> GetContacts()
        {
            var query = new ContactsQuery("https://www.google.com/m8/feeds/contacts/default/full");
            query.NumberToRetrieve = int.MaxValue;
            query.ModifiedSince = _lastSyncTime;

            var feed = _googleService.Query(query);
            var result = feed.Entries.OfType<ContactEntry>()
                .Where(x => !x.Deleted && x.Phonenumbers != null && x.Phonenumbers.Count > 0)
                .Select(CreateContact).Where(x => x != null)
                .ToArray();

            _lastSyncTime = DateTime.Now;
            _plugin.PluginManager.Core.SettingsManager.Set("LastSync", _lastSyncTime);

            return result;
        }

        public IContactInfo GetContact(string key)
        {
            throw new NotImplementedException();
        }

        public IContactInfo CreateContact()
        {
            return new Contact();
        }

        public string InsertOrUpdateContact(IContactInfo contact)
        {
            return String.Empty;
        }

        public void RemoveContact(string key)
        {
            
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
            var query = new GroupsQuery("https://www.google.com/m8/feeds/groups/default/full");
            query.NumberToRetrieve = int.MaxValue;

            var feed = _googleService.Query(query);
            var result = feed.Entries.OfType<GroupEntry>()
                .Where(x => !x.Deleted && x.SystemGroup == null)
                .Select(CreateTag).Where(x => x != null)
                .ToArray();

            return result;
        }

        public IContactTag GetTag(string key)
        {
            throw new NotImplementedException();
        }

        public IContactTag CreateTag()
        {
            return new ContactTag();
        }

        public string InsertOrUpdateTag(IContactTag tag)
        {
            return string.Empty;
        }

        public void RemoveTag(string key)
        {
            
        }

        private Contact CreateContact(ContactEntry contactEntry)
        {
            try
            {
                var version = contactEntry.Updated.Ticks;
                var key = contactEntry.Id.AbsoluteUri.Split('/').LastOrDefault();

                if (string.IsNullOrEmpty(key)) return null;

                var contact = new Contact()
                    {
                        Company =
                            contactEntry.Organizations != null
                                ? contactEntry.Organizations.Select(x => x.Name).FirstOrDefault()
                                : string.Empty,
                        Key = key,
                        VersionKey = version
                    };

                if (contactEntry.Name != null)
                {
                    contact.FirstName = contactEntry.Name.GivenName;
                    contact.LastName = contactEntry.Name.FamilyName;
                    contact.MiddleName = contactEntry.Name.AdditionalName;
                }
                else if (string.IsNullOrEmpty(contact.Company)) {
                    if (contactEntry.PrimaryEmail != null) contact.FirstName = contactEntry.PrimaryEmail.Address;
                    else if (contactEntry.Emails != null && contactEntry.Emails.Any()) contact.FirstName = contactEntry.Emails.First().Address;
                }

                if (contactEntry.Emails != null)
                    foreach (var x in contactEntry.Emails)
                        contact.Emails.Add(new ContactEmail()
                            {
                                Email = x.Address,
                                Comment = String.Empty,
                                Key = x.Address,
                                VersionKey = version
                            });

                if (contactEntry.Phonenumbers != null)
                    foreach (var x in contactEntry.Phonenumbers)
                        contact.PhoneNumbers.Add(new ContactPhone()
                            {
                                Number = x.Value,
                                Comment = String.Empty,
                                Key = x.Uri,
                                VersionKey = version
                            });

                if (contactEntry.GroupMembership != null && contactEntry.GroupMembership.Any())
                    foreach (var x in contactEntry.GroupMembership)
                    {
                        var tagKey = x.HRef.Split('/').LastOrDefault();
                        if (string.IsNullOrEmpty(tagKey)) continue;

                        var tag = _plugin.PluginManager.Core.ContactsManager.Tags.FirstOrDefault(y => y.Key == tagKey && y.AddressBook.Key == Key);
                        if (tag == null) continue;
                        
                        contact.Tags.Add(tag);
                    }

                contact.IsChanged = false;

                return contact;
            }
            catch (Exception e)
            {
                Logger.LogWarn(e, "Can't parse google contact.");
            }

            return null;
        }

        private ContactTag CreateTag(GroupEntry groupEntry)
        {
            if (groupEntry.Title == null || string.IsNullOrEmpty(groupEntry.Title.Text)) return null;

            var key = groupEntry.Id.AbsoluteUri.Split('/').LastOrDefault();
            if (string.IsNullOrEmpty(key)) return null;

            var tag = new ContactTag() {Name = groupEntry.Title.Text, VersionKey = groupEntry.Updated.Ticks, Key = key, Color = "#000000"};
            tag.IsChanged = false;

            return tag;
        }
    }
}
