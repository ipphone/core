using System;
using System.Collections.Generic;
using System.Text;

namespace ContactPoint.Common.PluginManager
{
    /// <summary>
    /// Plugin attribute. Indicates that class must be loaded as plugin
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class PluginAttribute : Attribute
    {
        /// <summary>
        /// Plugin ID. Must be GUID
        /// </summary>
        public Guid ID { get; private set; }

        /// <summary>
        /// Plugin name
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Indicates that plugin support or not setting dialog
        /// </summary>
        public bool HaveSettingsForm { get; set; }

        /// <summary>
        /// Some other public information about plugin
        /// </summary>
        public string Info { get; set; }

        /// <summary>
        /// Plugin attribute. Indicates that class must be loaded as plugin
        /// </summary>
        /// <param name="id">Plugin ID. Must be GUID.</param>
        /// <param name="name">Plugin user-friendly name</param>
        public PluginAttribute(string id, string name)
        {
            ID = new Guid(id);
            Name = name;
        }
    }
}
