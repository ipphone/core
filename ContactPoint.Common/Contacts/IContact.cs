using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ContactPoint.Common.Contacts.Local;

namespace ContactPoint.Common.Contacts
{
    public interface IContact : IEntity
    {
        event Action<IContact> Changed;

        string FirstName { get; set; }
        string LastName { get; set; }
        string MiddleName { get; set; }
        string Company { get; set; }

        /// <summary>
        /// Composition of name parts
        /// </summary>
        string ShowedName { get; }

        /// <summary>
        /// Linked contact information objects.
        /// </summary>
        IEnumerable<IContactInfoLocal> ContactInfos { get; }

        /// <summary>
        /// Link contact from specific address book.
        /// </summary>
        /// <param name="contact">Contact info to link to current.</param>
        void LinkContactInfo(IContactInfoLocal contact);

        /// <summary>
        /// Unlink contact from specific address book.
        /// </summary>
        /// <param name="contact">Contact info to remove link from current.</param>
        void UnlinkContactInfo(IContactInfoLocal contact);

        /// <summary>
        /// Save contact and related contact information objects into DB and push changes to address books implementations.
        /// </summary>
        void Submit();

        /// <summary>
        /// Remove contact from address book. Method will remove linked contac informations from writeable address books.
        /// </summary>
        void Remove();
    }
}
