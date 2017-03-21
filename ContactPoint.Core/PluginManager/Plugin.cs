using System.Collections.Generic;
using ContactPoint.Common;
using ContactPoint.Common.PluginManager;
using ContactPoint.Common.Contacts;

namespace ContactPoint.Core.PluginManager
{
    /// <summary>
    /// Base Plugin implementation
    /// </summary>
    public abstract class Plugin : IPlugin
    {
        private bool _isStarted;

        /// <summary>
        /// Raised when plugin started
        /// </summary>
        public event ServiceStartedDelegate Started;

        /// <summary>
        /// Raised when plugin stopped
        /// </summary>
        public event ServiceStoppedDelegate Stopped;

        /// <summary>
        /// Plugin manager that handles this plugin
        /// </summary>
        public IPluginManager PluginManager { get; }

        /// <summary>
        /// Elements exported to UI
        /// </summary>
        public virtual IEnumerable<IPluginUIElement> UIElements { get; } = null;

		/// <summary>
		/// Plugins address book
		/// </summary>
		public virtual IAddressBook AddressBook => null;

        /// <summary>
        /// Is plugin started
        /// </summary>
        public virtual bool IsStarted => _isStarted;

        /// <summary>
        /// Plugin constructor.
        /// </summary>
        /// <param name="pluginManager">PluginManager object</param>
        protected Plugin(IPluginManager pluginManager)
        {
            PluginManager = pluginManager;
        }

        /// <summary>
        /// Start plugin
        /// </summary>
        public virtual void Start()
        {
            _isStarted = true;
            RaiseStartedEvent();
        }

        /// <summary>
        /// Stop plugin
        /// </summary>
        public virtual void Stop()
        {
            _isStarted = false;
            RaiseStoppedEvent("Normal stop");
        }

        /// <summary>
        /// Settings dialog for plugin. Do nothing by default.
        /// </summary>
        public virtual void ShowSettingsDialog()
        { }

        /// <summary>
        /// Raises Plugin Started event
        /// </summary>
        protected void RaiseStartedEvent()
        {
            Started?.Invoke(this);
        }

        /// <summary>
        /// Raises Plugin Stopped event
        /// </summary>
        protected void RaiseStoppedEvent(string message)
        {
            Stopped?.Invoke(this, message);
        }
    }
}
