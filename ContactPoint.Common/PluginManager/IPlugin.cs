using System.Collections.Generic;
using ContactPoint.Common.Contacts;

namespace ContactPoint.Common.PluginManager
{
    public interface IPlugin : IService
    {
        /// <summary>
        /// Plugin manager that handles this plugin
        /// </summary>
        IPluginManager PluginManager { get; }

		/// <summary>
		/// Plugin address book.
		/// 
		/// Plugins can expose their own address books.
		/// </summary>
    	IAddressBook AddressBook { get; }

        /// <summary>
        /// Elements exported to UI
        /// </summary>
        IEnumerable<IPluginUIElement> UIElements { get; }

        /// <summary>
        /// Show settings dialog if it exists
        /// </summary>
        void ShowSettingsDialog();
    }
}
