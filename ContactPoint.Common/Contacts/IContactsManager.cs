using System;
using System.Collections.Generic;
using ContactPoint.Common.Contacts.Local;

namespace ContactPoint.Common.Contacts
{
    /// <summary>
    /// Contacts manager.
    /// </summary>
    public interface IContactsManager : IDisposable
    {
        /// <summary>
        /// Fired when Address book reloaded.
        /// </summary>
        event Action<IAddressBookLocal> AddressBookReloaded;

        /// <summary>
        /// Link to core object.
        /// </summary>
        ICore Core { get; }

        /// <summary>
        /// Possible address books.
        /// </summary>
        IEnumerable<IAddressBookLocal> AddressBooks { get; }
            
        /// <summary>
        /// Contacts collection.
        /// </summary>
        ICollection<IContact> Contacts { get; }
            
        /// <summary>
        /// All possible tags.
        /// </summary>
        IEnumerable<IContactTagLocal> Tags { get; }
            
        /// <summary>
        /// Create generic contact.
        /// </summary>
        /// <returns>Contact object.</returns>
        IContact CreateContact();

        /// <summary>
        /// Search for contact by given phone number.
        /// </summary>
        /// <param name="phoneNumber">Phone number to search.</param>
        /// <returns>Contact.</returns>
        IContact GetContactByPhoneNumber(string phoneNumber);
    }
}
