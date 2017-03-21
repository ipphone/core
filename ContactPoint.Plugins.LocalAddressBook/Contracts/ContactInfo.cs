using ContactPoint.Common.Contacts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace ContactPoint.Plugins.LocalAddressBook.Contracts
{
    [DataContract]
    internal class ContactInfo : Versionable, IContactInfo
    {
        [DataMember]
        private List<IContactPhone> _phoneNumbers = new List<IContactPhone>();
        [DataMember]
        private List<IContactEmail> _emails = new List<IContactEmail>();
        [DataMember]
        private List<IContactTag> _tags = new List<IContactTag>();

        [DataMember]
        public string Key { get; set; }
        [DataMember]
        public string FirstName { get; set; }
        [DataMember]
        public string MiddleName { get; set; }
        [DataMember]
        public string LastName { get; set; }
        [DataMember]
        public string Company { get; set; }
        [DataMember]
        public string JobTitle { get; set; }
        [DataMember]
        public string Note { get; set; }

        [IgnoreDataMember]
        public ICollection<IContactPhone> PhoneNumbers
        {
            get { return _phoneNumbers; }
        }

        [IgnoreDataMember]
        public ICollection<IContactEmail> Emails
        {
            get { return _emails; }
        }

        [IgnoreDataMember]
        public ICollection<IContactTag> Tags
        {
            get { return _tags; }
        }

        public ContactInfo()
        {
            Key = Guid.NewGuid().ToString("D");
        }

        internal static ContactInfo CreateFromContactInfo(IContactInfo contactInfo)
        {
            var result = new ContactInfo()
                {
                    FirstName = contactInfo.FirstName,
                    LastName = contactInfo.LastName,
                    MiddleName = contactInfo.MiddleName,
                    Company = contactInfo.Company,
                    JobTitle = contactInfo.JobTitle,
                    Note = contactInfo.Note,
                    Key = contactInfo.Key
                };

            foreach (var item in contactInfo.PhoneNumbers) result.PhoneNumbers.Add(new ContactPhone() { Number = item.Number, Comment = item.Comment, Key = item.Number });
            foreach (var item in contactInfo.Emails) result.Emails.Add(new ContactEmail() { Email = item.Email, Comment = item.Comment, Key = item.Email });
            foreach (var item in contactInfo.Tags) result.Tags.Add(new ContactTag() { Color = item.Color, Name = item.Name, Key = item.Key } );

            result.SetVersionKey(contactInfo.VersionKey);

            return result;
        }

        public static ContactInfo GetLocalObject(IContactInfo contactInfo)
        {
            var result = contactInfo as ContactInfo;
            return result ?? CreateFromContactInfo(contactInfo);
        }
    }
}
