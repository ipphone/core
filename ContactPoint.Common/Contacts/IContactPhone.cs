using System;
using System.Collections.Generic;
using System.Text;

namespace ContactPoint.Common.Contacts
{
	/// <summary>
	/// Phone number of the contact
	/// </summary>
	public interface IContactPhone : IVersionable
	{
        /// <summary>
        /// Unique key for phone number.
        /// </summary>
        string Key { get; }

		/// <summary>
		/// Optional phone number comment.
		/// </summary>
		string Comment { get; set; }

		/// <summary>
		/// Phone number.
		/// </summary>
		string Number { get; set; }
    }
}
