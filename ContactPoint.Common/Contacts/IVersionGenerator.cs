using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ContactPoint.Common.Contacts
{
    public enum VersionsCompareResult
    {
        Unknown = 0,
        Equals,
        Greater,
        Lower
    }

    public interface IVersionGenerator
    {
        /// <summary>
        /// Get version that will indicate initial state of object.
        /// </summary>
        /// <returns>Default version number.</returns>
        object GetDefaultVersion();

        /// <summary>
        /// Generate next version that will be newer than provided version.
        /// </summary>
        /// <param name="currentKey">Current version key.</param>
        /// <returns>Newer version key.</returns>
        object GenerateNextVersion(object currentKey);

        /// <summary>
        /// Deserialize version key from string.
        /// </summary>
        /// <param name="value">Serialized version key.</param>
        /// <returns>Strongly typed version key.</returns>
        object GetKeyFromString(string value);

        /// <summary>
        /// Serialize version key to string.
        /// </summary>
        /// <param name="value">Strongly typed version key.</param>
        /// <returns>Serialized version key.</returns>
        string ConvertKeyToString(object value);

        /// <summary>
        /// Compare current version with provided.
        /// </summary>
        /// <param name="current">Version to compare with.</param>
        /// <param name="target">Version to compare to.</param>
        /// <returns>How is current version in compare with target version.</returns>
        VersionsCompareResult CompareVersions(IVersionable current, IVersionable target);
    }
}
