using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace ContactPoint.Common.PluginManager
{
    /// <summary>
    /// Delegate for CommandExecuted event
    /// </summary>
    /// <param name="sender">UI element that initiates command</param>
    /// <param name="pluginUIElement">Plugin action element that handles command</param>
    public delegate void CommandExecutedDelegate(object sender, IPluginUIElement pluginUIElement);

    public interface IPluginUIElement : IDisposable
    {
        /// <summary>
        /// Event that raised when plugin command is executed
        /// </summary>
        event CommandExecutedDelegate CommandExecuted;

        /// <summary>
        /// Event raised when some UI in action element changed (graphics, text, etc...)
        /// </summary>
        event Action<IPluginUIElement> UIChanged;

        /// <summary>
        /// Unique id of UI element
        /// </summary>
        Guid Id { get; }

        /// <summary>
        /// Controls showing plugin command in main program menu
        /// </summary>
        bool ShowInMenu { get; }

        /// <summary>
        /// Controls showing plugin command in notfy icon menu
        /// </summary>
        bool ShowInNotifyMenu { get; }

        /// <summary>
        /// Controls showing plugin command in toolbar of main form
        /// </summary>
        bool ShowInToolBar { get; }

        /// <summary>
        /// Enabled or disabled UI element
        /// </summary>
        bool Enabled { get; }

        /// <summary>
        /// Image used as default
        /// </summary>
        Bitmap Image { get; }

        /// <summary>
        /// Text of UI element
        /// </summary>
        string Text { get; }

        /// <summary>
        /// Subitems
        /// </summary>
        IEnumerable<IPluginUIElement> Childrens { get; }

        /// <summary>
        /// Plugin which associated with this action
        /// </summary>
        IPlugin Plugin { get; }

        /// <summary>
        /// Action code for checked action. This action code is also using when checked is disabled
        /// </summary>
        Guid ActionCode { get; }
        
        /// <summary>
        /// Executing plugin action
        /// </summary>
        /// <param name="sender">UI object assigned with this action</param>
        /// <param name="data">Optional parameter to pass into PluginAction command</param>
        void Execute(object sender, object data = null);
    }
}
