using ContactPoint.Common.Contacts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace ContactPoint.Plugins.LocalAddressBook.Contracts
{
    [DataContract]
    internal class ContactEmail : Versionable, IContactEmail
    {
        [DataMember]
        public string Key { get; set; }
        [DataMember]
        public string Comment { get; set; }
        [DataMember]
        public string Email { get; set; }

        public ContactEmail()
        {
            Key = Guid.NewGuid().ToString("D");
        }
    }
}
