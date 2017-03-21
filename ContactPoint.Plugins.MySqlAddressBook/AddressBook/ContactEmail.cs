using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ContactPoint.Common.Contacts;

namespace ContactPoint.Plugins.MySqlAddressBook.AddressBook
{
    internal class ContactEmail : Versionable, IContactEmail
    {
        public string Key { get; private set; }
        public string Comment { get; set; }
        public string Email { get; set; }
    }
}
