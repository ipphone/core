using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ContactPoint.Common.Contacts.Local
{
    /// <summary>
    /// Local representation of address book tag.
    /// </summary>
    public interface IContactTagLocal : IContactTag, IEntity
    {
        event Action<IContactTagLocal> Changed;

        /// <summary>
        /// Address book that tag belongs to.
        /// </summary>
        IAddressBookLocal AddressBook { get; }

        /// <summary>
        /// Update internal information from give tag.
        /// </summary>
        /// <param name="source">Source object to update from.</param>
        void UpdateFrom(IContactTag source);

        /// <summary>
        /// Update only tag address book specific key.
        /// </summary>
        /// <param name="key">New key value.</param>
        void UpdateKey(string key);

        /// <summary>
        /// Submit changes to database and address book.
        /// </summary>
        void Submit();

        /// <summary>
        /// Remove tag from database and address book.
        /// </summary>
        void Remove();
    }
}
