using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace ContactPoint.Common.PluginManager
{
    public interface IPluginCheckedUIElement : IPluginUIElement
    {
        /// <summary>
        /// Event raised when Checked property changed
        /// </summary>
        event Action<IPluginUIElement> CheckedChanged;

        /// <summary>
        /// Indicates that this is "select one from list" action
        /// </summary>
        bool IsGroupCheckAction { get; }

        /// <summary>
        /// Indicating checked state
        /// </summary>
        bool Checked { get; }

        /// <summary>
        /// Image showed when command state is checked
        /// </summary>
        Bitmap ImageChecked { get; }

        /// <summary>
        /// Executing plugin action and changing checked state. Use this operation when element implements this interface
        /// </summary>
        /// <param name="sender">UI object assigned with this action</param>
        /// <param name="checkedValue">New checked value</param>
        void ExecuteChecked(object sender, bool checkedValue, object data = null);
    }
}
