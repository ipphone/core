using ContactPoint.Common.Contacts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace ContactPoint.Plugins.LocalAddressBook.Contracts
{
    [DataContract]
    internal class ContactPhone : Versionable, IContactPhone
    {
        [DataMember]
        public string Key { get; set; }
        [DataMember]
        public string Comment { get; set; }
        [DataMember]
        public string Number { get; set; }

        public ContactPhone()
        {
            Key = Guid.NewGuid().ToString("D");
        }
    }
}
