using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using ContactPoint.Common.Contacts;
using ContactPoint.Plugins.LocalAddressBook.Contracts;

namespace ContactPoint.Plugins.LocalAddressBook
{
    [DataContract]
    internal class VersionGenerator : IVersionGenerator
    {
        private static readonly VersionGenerator _instance = new VersionGenerator();
        internal static VersionGenerator Instance
        {
            get { return _instance; }
        }

        public object GetDefaultVersion()
        {
            return Versionable.DefaultValue;
        }

        public int GenerateNextVersion(Versionable version)
        {
            return version.VersionKey + 1;
        }

        object IVersionGenerator.GenerateNextVersion(object versionKey)
        {
            if (versionKey == null) return Versionable.DefaultValue;
            if (versionKey is int) return (int)versionKey + 1;

            return Versionable.DefaultValue;
        }

        object IVersionGenerator.GetKeyFromString(string value)
        {
            return GetKeyFromString(value);
        }

        int GetKeyFromString(string value)
        {
            int result;
            if (int.TryParse(value, out result))
                return result;

            return Versionable.DefaultValue;
        }

        public string ConvertKeyToString(object versionKey)
        {
            return versionKey.ToString();
        }

        public VersionsCompareResult CompareVersions(IVersionable current, IVersionable target)
        {
            var targetLocal = target as Versionable;
            var currentLocal = current as Versionable;

            if (targetLocal != null && currentLocal != null)
                CompareVersions(currentLocal.VersionKey, targetLocal.VersionKey);

            return CompareVersions(GetKeyFromString(current.VersionKey.ToString()),
                                   GetKeyFromString(target.VersionKey.ToString()));
        }

        private VersionsCompareResult CompareVersions(int currentKey, int targetKey)
        {
            return targetKey == currentKey
                       ? VersionsCompareResult.Equals
                       : currentKey > targetKey
                             ? VersionsCompareResult.Greater
                             : VersionsCompareResult.Lower;
        }
    }
}
