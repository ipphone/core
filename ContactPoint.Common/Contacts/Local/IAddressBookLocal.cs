using System;
using System.Collections.Generic;

namespace ContactPoint.Common.Contacts.Local
{
    /// <summary>
    /// Address book from plugin.
    /// </summary>
    public interface IAddressBookLocal : IEntity
    {
        /// <summary>
        /// Show if we can change contacts in address book.
        /// </summary>
        bool ReadOnly { get; }

        /// <summary>
        /// Address book name.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Order number that should be used for sorting multiple address books by UI.
        /// </summary>
        int OrderNumber { get; set; }

        /// <summary>
        /// Indicates is address book available for operations.
        /// </summary>
        bool IsOnline { get; }

        /// <summary>
        /// Internal key of address book.
        /// </summary>
        Guid Key { get; }

        /// <summary>
        /// Version generator that will be used to generate version for objects.
        /// </summary>
        IVersionGenerator VersionGenerator { get; }

        /// <summary>
        /// Initialize new Contact for specific address book.
        /// </summary>
        /// <returns>Contact object.</returns>
        IContactInfoLocal CreateContactInfo();

        /// <summary>
        /// Initialize new phone number object for specific address book.
        /// </summary>
        /// <returns>PhoneNumber object.</returns>
        IContactPhoneLocal CreatePhoneNumber();

        /// <summary>
        /// Initialize new email object for specific address book.
        /// </summary>
        /// <returns>Email object.</returns>
        IContactEmailLocal CreateEmail();

        /// <summary>
        /// Initialize new tag object for specific address book.
        /// </summary>
        /// <returns>Tag object.</returns>
        IContactTagLocal CreateTag();
    }
}
