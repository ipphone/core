using System;
using System.Collections.Generic;
using System.Text;

namespace ContactPoint.Common.Contacts
{
	/// <summary>
	/// Email of the contact
	/// </summary>
	public interface IContactEmail : IVersionable
	{
        /// <summary>
        /// Unique key for email.
        /// </summary>
        string Key { get; }

		/// <summary>
		/// Optional comment for email.
		/// </summary>
		string Comment { get; set; }

		/// <summary>
		/// Email address.
		/// </summary>
		string Email { get; set; }
    }
}
