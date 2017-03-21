using System;
using System.Drawing;
using ContactPoint.Common.PluginManager;

namespace ContactPoint.Core.PluginManager
{
    /// <summary>
    /// Base functionality for Plugin UI Check-enabled Element
    /// </summary>
    public abstract class PluginCheckedUIElementBase : PluginUIElementBase, IPluginCheckedUIElement
    {
        /// <summary>
        /// Checked/unchecked flag
        /// </summary>
        private bool _checked;

        /// <summary>
        /// Image showed when command state is checked
        /// </summary>
        public virtual Bitmap ImageChecked => Image;

        /// <summary>
        /// Indicates that this is "select one from list" action
        /// </summary>
        public virtual bool IsGroupCheckAction => false;

        /// <summary>
        /// Event raised when Checked property changed
        /// </summary>
        public virtual event Action<IPluginUIElement> CheckedChanged;

        /// <summary>
        /// Indicating checked state
        /// </summary>
        public virtual bool Checked
        {
            get { return _checked; }
            protected set
            {
                if (_checked != value)
                {
                    _checked = value;
                    RaiseCheckedChangedEvent();
                }
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="plugin">Owner plugin</param>
        public PluginCheckedUIElementBase(IPlugin plugin) : base(plugin)
        { }

        /// <summary>
        /// Executing plugin action and changing checked state. Use this operation when element implements this interface
        /// </summary>
        /// <param name="sender">UI object assigned with this action</param>
        /// <param name="checkedValue">New checked value</param>
        /// <param name="data">Optional data</param>
        public void ExecuteChecked(object sender, bool checkedValue, object data = null)
        {
            if (Plugin.IsStarted)
            {
                _checked = checkedValue; // Set to field, not property, because I whant to raise CommandExecuted event before CheckedChanged and thats why I raised CheckedChanged event here

                ExecuteCheckedCommand(sender, checkedValue, data);
                RaiseCheckedChangedEvent();
            }
        }

        /// <summary>
        /// Executing plugin action
        /// </summary>
        /// <param name="sender">UI object assigned with this action</param>
        /// <param name="checkedValue">New checked value</param>
        /// <param name="data">Optional data</param>
        protected abstract void ExecuteCheckedCommand(object sender, bool checkedValue, object data);

        protected sealed override void ExecuteCommand(object sender, object data)
        {
            ExecuteCheckedCommand(sender, true, data);
        }

        /// <summary>
        /// Raises CheckedChanged event
        /// </summary>
        protected void RaiseCheckedChangedEvent()
        {
            CheckedChanged?.Invoke(this);
        }
    }
}
