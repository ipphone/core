using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ContactPoint.Common.Contacts;

namespace ContactPoint.Plugins.GoogleContacts.Model
{
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

        public long GenerateNextVersion(Versionable version)
        {
            return DateTime.Now.Ticks;
        }

        object IVersionGenerator.GenerateNextVersion(object versionKey)
        {
            return DateTime.Now.Ticks;
        }

        object IVersionGenerator.GetKeyFromString(string value)
        {
            return GetKeyFromString(value);
        }

        long GetKeyFromString(string value)
        {
            long result;
            if (long.TryParse(value, out result))
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

        private VersionsCompareResult CompareVersions(long currentKey, long targetKey)
        {
            return targetKey == currentKey
                       ? VersionsCompareResult.Equals
                       : currentKey > targetKey
                             ? VersionsCompareResult.Greater
                             : VersionsCompareResult.Lower;
        }
    }
}
