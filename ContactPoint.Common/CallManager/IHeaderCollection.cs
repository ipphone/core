using System;
using System.Collections.Generic;
using System.Text;

namespace ContactPoint.Common.CallManager
{
    /// <summary>
    /// Collection of headers
    /// </summary>
    public interface IHeaderCollection : IEnumerable<IHeader>
    {
        /// <summary>
        /// Get header by name (case insensitive)
        /// </summary>
        /// <param name="key">Header name</param>
        /// <returns>Header</returns>
        IHeader this[string key] { get; }

        /// <summary>
        /// Headers count
        /// </summary>
        int Count { get; }

        /// <summary>
        /// Check if header with specified name is exists
        /// </summary>
        /// <param name="key">Header name</param>
        /// <returns>True if header exists</returns>
        bool Contains(string key);

        /// <summary>
        /// Get value if exists or return empty string if not
        /// </summary>
        /// <param name="key">Header name</param>
        /// <returns>Header value</returns>
        string GetValueSafe(string key);
    }
}
