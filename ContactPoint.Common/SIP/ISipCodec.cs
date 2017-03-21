using System;
using System.Collections.Generic;
using System.Text;

namespace ContactPoint.Common.SIP
{
    /// <summary>
    /// Interface representing SIP codec
    /// </summary>
    public interface ISipCodec
    {
        /// <summary>
        /// Occurs when IsEnabled property has been changed
        /// </summary>
        event Action<ISipCodec> EnabledChanged;

        /// <summary>
        /// Codec name
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Indicating that we use or not this codec
        /// </summary>
        bool Enabled { get; set; }

        /// <summary>
        /// Codec priority
        /// </summary>
        int Priority { get; }
    }
}
