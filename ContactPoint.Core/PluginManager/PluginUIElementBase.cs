using System;
using System.Collections.Generic;
using System.Drawing;
using ContactPoint.Common.PluginManager;

namespace ContactPoint.Core.PluginManager
{
    /// <summary>
    /// Base functionality for Plugin UI Element
    /// </summary>
    public abstract class PluginUIElementBase : IPluginUIElement
    {
        /// <summary>
        /// Event that raised when plugin command is executed
        /// </summary>
        public virtual event CommandExecutedDelegate CommandExecuted;

        /// <summary>
        /// Event raised when some UI in action element changed (graphics, text, enabled, etc...)
        /// </summary>
        public virtual event Action<IPluginUIElement> UIChanged;

        /// <summary>
        /// Unique Id of UI element.
        /// </summary>
        public Guid Id { get; } = Guid.NewGuid();

        /// <summary>
        /// Controls showing plugin command in main program menu
        /// </summary>
        public virtual bool ShowInMenu => false;

        /// <summary>
        /// Controls showing plugin command in notfy icon menu
        /// </summary>
        public virtual bool ShowInNotifyMenu => false;

        /// <summary>
        /// Controls showing plugin command in toolbar of main form
        /// </summary>
        public virtual bool ShowInToolBar => false;

        /// <summary>
        /// Enabled or disabled UI element
        /// </summary>
        public virtual bool Enabled => true;

        /// <summary>
        /// Image used as default
        /// </summary>
        public abstract Bitmap Image { get; }

        /// <summary>
        /// Text of UI element
        /// </summary>
        public abstract string Text { get; }

        /// <summary>
        /// Subitems
        /// </summary>
        public abstract IEnumerable<IPluginUIElement> Childrens { get; }

        /// <summary>
        /// Plugin which associated with this action
        /// </summary>
        public IPlugin Plugin { get; }

        /// <summary>
        /// Action code for action.
        /// </summary>
        public abstract Guid ActionCode { get; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="plugin">Owner plugin</param>
        protected PluginUIElementBase(IPlugin plugin)
        {
            Plugin = plugin;
        }

        ~PluginUIElementBase()
        {
            Dispose(false);
        }

        /// <summary>
        /// Executing plugin action
        /// </summary>
        /// <param name="sender">UI object assigned with this action</param>
        /// <param name="data">Optional data</param>
        public void Execute(object sender, object data)
        {
            if (Plugin.IsStarted)
            {
                ExecuteCommand(sender, data);
                RaiseCommandExecutedEvent(sender, this);
            }
        }

        /// <summary>
        /// Really executing plugin action
        /// </summary>
        /// <param name="sender">UI object assigned with this action</param>
        /// <param name="data">Optional data</param>
        protected abstract void ExecuteCommand(object sender, object data);

        /// <summary>
        /// Raises CommandExecuted event
        /// </summary>
        /// <param name="sender">UI object assigned with this action</param>
        /// <param name="pluginUIElement">UI element</param>
        protected void RaiseCommandExecutedEvent(object sender, IPluginUIElement pluginUIElement)
        {
            CommandExecuted?.Invoke(sender, pluginUIElement);
        }

        /// <summary>
        /// Raises UIChanged event
        /// </summary>
        protected void RaiseUIChangedEvent()
        {
            UIChanged?.Invoke(this);
        }

        #region IDisposable Members

        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposing)
            {
                GC.SuppressFinalize(this);
            }
        }

        #endregion
    }
}
