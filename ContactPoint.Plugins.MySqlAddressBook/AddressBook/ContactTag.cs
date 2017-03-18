using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ContactPoint.Common.Contacts;

namespace ContactPoint.Plugins.MySqlAddressBook.AddressBook
{
    internal class ContactTag : Versionable, IContactTag
    {
        public string Key { get; private set; }
        public string Name { get; set; }
        public string Color { get; set; }
    }
}
