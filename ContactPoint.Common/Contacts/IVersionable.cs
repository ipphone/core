using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ContactPoint.Common.Contacts
{
    /// <summary>
    /// Implements versions for objects.
    /// </summary>
    public interface IVersionable
    {
        /// <summary>
        /// Version generator assigned to current version container.
        /// </summary>
        IVersionGenerator VersionGenerator { get; }

        /// <summary>
        /// Version key.
        /// </summary>
        object VersionKey { get; }

        /// <summary>
        /// Indicates is object changed since initial load.
        /// </summary>
        bool IsChanged { get; }

        /// <summary>
        /// Generate new version that is greater than current.
        /// </summary>
        void IncrementVersion();
    }
}
