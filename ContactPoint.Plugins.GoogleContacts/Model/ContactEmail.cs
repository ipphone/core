using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ContactPoint.Common.Contacts;

namespace ContactPoint.Plugins.GoogleContacts.Model
{
    internal class ContactEmail : Versionable, IContactEmail
    {
        public string Key { get; set; }
        public string Comment { get; set; }
        public string Email { get; set; }
    }
}
