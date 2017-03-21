using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ContactPoint.Common.Contacts;

namespace ContactPoint.Plugins.GoogleContacts.Model
{
    internal class Contact : Versionable, IContactInfo
    {
        public string Key { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string Company { get; set; }
        public string JobTitle { get; set; }
        public string Note { get; set; }
        public ICollection<IContactPhone> PhoneNumbers { get; private set; }
        public ICollection<IContactEmail> Emails { get; private set; }
        public ICollection<IContactTag> Tags { get; private set; }

        public Contact()
        {
            PhoneNumbers = new List<IContactPhone>();
            Emails = new List<IContactEmail>();
            Tags = new List<IContactTag>();
        }
    }
}
