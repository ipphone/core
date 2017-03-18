using System;
using System.Collections.Generic;
using System.Text;

namespace ContactPoint.Common.PluginManager
{
    public interface IPluginAction
    {
        /// <summary>
        /// Unique code of action
        /// </summary>
        Guid ActionCode { get; }

        /// <summary>
        /// Optional object
        /// </summary>
        object Object { get; }
    }
}
