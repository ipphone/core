using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ContactPoint.Common.Contacts;

namespace ContactPoint.Plugins.MySqlAddressBook.AddressBook
{
    internal class Versionable : IVersionable
    {
        internal static int DefaultValue = 0;

        public Versionable()
        {
            VersionKey = DefaultValue;
        }

        public int VersionKey { get; set; }
        public virtual bool IsChanged { get; set; }

        public IVersionGenerator VersionGenerator
        {
            get { return MySqlAddressBook.AddressBook.VersionGenerator.Instance; }
        }

        public void IncrementVersion()
        {
            VersionKey = MySqlAddressBook.AddressBook.VersionGenerator.Instance.GenerateNextVersion(this);
        }

        object IVersionable.VersionKey { get { return VersionKey; } }
    }
}
