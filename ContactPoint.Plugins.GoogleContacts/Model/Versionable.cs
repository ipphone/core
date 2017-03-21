using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ContactPoint.Common.Contacts;

namespace ContactPoint.Plugins.GoogleContacts.Model
{
    internal class Versionable : IVersionable
    {
        internal static int DefaultValue = 0;

        private long _versionKey = DefaultValue;

        public long VersionKey
        {
            get { return _versionKey; }
            set { _versionKey = value; }
        }

        public virtual bool IsChanged { get; set; }

        public IVersionGenerator VersionGenerator
        {
            get { return ContactPoint.Plugins.GoogleContacts.Model.VersionGenerator.Instance; }
        }

        public void IncrementVersion()
        {
            VersionKey = ContactPoint.Plugins.GoogleContacts.Model.VersionGenerator.Instance.GenerateNextVersion(this);
        }

        public void SetVersionKey(object versionKey)
        {
            if (versionKey == null) return;

            if (versionKey is long) _versionKey = (long)versionKey;

            if (!long.TryParse(versionKey.ToString(), out _versionKey))
                _versionKey = DefaultValue;
        }

        object IVersionable.VersionKey { get { return VersionKey; } }
    }
}
