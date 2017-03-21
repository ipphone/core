using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ContactPoint.Common.Contacts;

namespace ContactPoint.Plugins.GoogleContacts.Model
{
    internal class ContactPhone : Versionable, IContactPhone
    {
        public string Key { get; set; }
        public string Comment { get; set; }
        public string Number { get; set; }
    }
}
