using System.Collections.Generic;
using System.Reflection;

namespace ContactPoint.Common.PluginManager
{
    /// <summary>
    /// Plugin information provider
    /// </summary>
    public interface IPluginInformationProvider
    {
        /// <summary>
        /// List of plugin information descriptors that can be  supported by provider
        /// </summary>
        IEnumerable<IPluginInformation> GetPluginInformations(Assembly assembly);
    }
}
