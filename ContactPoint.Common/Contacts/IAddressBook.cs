using System;
using System.Collections.Generic;
using System.Text;

namespace ContactPoint.Common.Contacts
{
	/// <summary>
	/// Readonly address book
	/// </summary>
	public interface IAddressBook
	{
        /// <summary>
        /// Unique address book Identifier.
        /// </summary>
        Guid Key { get; }

        /// <summary>
        /// Show if we can change contacts in address book.
        /// </summary>
        bool ReadOnly { get; }

		/// <summary>
		/// Address book name.
		/// </summary>
		string Name { get; }

        /// <summary>
        /// Indicates is address book available for operations.
        /// </summary>
        bool IsOnline { get; }

        /// <summary>
        /// Version generator that will be used to generate version for objects.
        /// </summary>
        IVersionGenerator VersionGenerator { get; }

		/// <summary>
		/// Get all contacts.
		/// </summary>
		/// <returns>Collection of contacts.</returns>
		IEnumerable<IContactInfo> GetContacts();
        
        /// <summary>
        /// Get latest version of specific contact.
        /// </summary>
        /// <param name="key">Contact key on target system.</param>
        /// <returns>Latest contact details.</returns>
	    IContactInfo GetContact(string key);

        /// <summary>
        /// Initialize new Contact for specific address book.
        /// </summary>
        /// <returns>Contact object.</returns>
	    IContactInfo CreateContact();

        /// <summary>
        /// Save specific contact in address book.
        /// </summary>
        /// <param name="contact">Contact to save in address book</param>
        /// <returns>Contact key.</returns>
        string InsertOrUpdateContact(IContactInfo contact);

        /// <summary>
        /// Remove contact from address book.
        /// </summary>
        /// <param name="key">Contact key to remove.</param>
        void RemoveContact(string key);

        /// <summary>
        /// Initialize new phone number object for specific address book.
        /// </summary>
        /// <returns>PhoneNumber object.</returns>
	    IContactPhone CreatePhoneNumber();

        /// <summary>
        /// Initialize new email object for specific address book.
        /// </summary>
        /// <returns>Email object.</returns>
	    IContactEmail CreateEmail();

        /// <summary>
        /// Get all tags.
        /// </summary>
        /// <returns>Collection of tags.</returns>
        IEnumerable<IContactTag> GetTags();

        /// <summary>
        /// Get latest version of specific tag.
        /// </summary>
        /// <param name="key">Tag key to retrieve.</param>
        /// <returns>Latest version of tag.</returns>
	    IContactTag GetTag(string key);

        /// <summary>
        /// Initialize new tag object for specific address book.
        /// </summary>
        /// <returns>Tag object.</returns>
	    IContactTag CreateTag();

        /// <summary>
        /// Save specific tag in address book.
        /// </summary>
        /// <param name="tag">Tag to save in address book.</param>
        /// <returns>Tag key.</returns>
	    string InsertOrUpdateTag(IContactTag tag);

        /// <summary>
        /// Remove tag from address book.
        /// </summary>
        /// <param name="key">Tag key to remove.</param>
	    void RemoveTag(string key);
	}
}
