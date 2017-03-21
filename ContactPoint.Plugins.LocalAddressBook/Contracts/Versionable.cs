using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using ContactPoint.Common.Contacts;

namespace ContactPoint.Plugins.LocalAddressBook.Contracts
{
    [DataContract]
    internal class Versionable : IVersionable
    {
        internal static int DefaultValue = 0;

        [IgnoreDataMember]
        private int _versionKey = DefaultValue;

        [DataMember]
        public int VersionKey
        {
            get { return _versionKey; }
            private set { _versionKey = value; }
        }

        [IgnoreDataMember]
        public virtual bool IsChanged { get { return false; } }

        [IgnoreDataMember]
        public IVersionGenerator VersionGenerator
        {
            get { return ContactPoint.Plugins.LocalAddressBook.VersionGenerator.Instance; }
        }

        public void IncrementVersion()
        {
            VersionKey = ContactPoint.Plugins.LocalAddressBook.VersionGenerator.Instance.GenerateNextVersion(this);
        }

        public void SetVersionKey(object versionKey)
        {
            if (versionKey == null) return;

            if (versionKey is int) _versionKey = (int) versionKey;

            if (!int.TryParse(versionKey.ToString(), out _versionKey))
                _versionKey = DefaultValue;
        }

        [IgnoreDataMember]
        object IVersionable.VersionKey { get { return VersionKey; } }
    }
}
