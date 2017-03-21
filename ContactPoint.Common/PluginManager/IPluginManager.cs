using System;
using System.Collections.Generic;

namespace ContactPoint.Common.PluginManager
{
    /// <summary>
    /// Plugin manager
    /// </summary>
    public interface IPluginManager : IService
    {
        /// <summary>
        /// Core object
        /// </summary>
        ICore Core { get; }

        /// <summary>
        /// List of all available plugins
        /// </summary>
        IEnumerable<IPluginInformation> Plugins { get; }

        /// <summary>
        /// Execute plugin action
        /// </summary>
        /// <param name="actionId">Target action ID</param>
        /// <param name="pluginId">Target plugin ID</param>
        /// <param name="data">Optional data</param>
        /// <returns>Return checked/unchecked action result or NULL</returns>
        bool? ExecuteAction(Guid actionId, Guid? pluginId = null, object data = null);
    }
}
