using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ContactPoint.Common.Contacts;

namespace ContactPoint.Plugins.GoogleContacts.Model
{
    internal class ContactTag : Versionable, IContactTag
    {
        public string Key { get; set; }
        public string Name { get; set; }
        public string Color { get; set; }
    }
}
