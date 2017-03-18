using System;
using System.Collections.Generic;
using System.Text;

namespace ContactPoint.Common.Contacts
{
	/// <summary>
	/// Contact
	/// </summary>
	public interface IContactInfo : IVersionable
	{
        string Key { get; }
		string FirstName { get; set; }
		string MiddleName { get; set; }
		string LastName { get; set; }

		string Company { get; set; }
		string JobTitle { get; set; }

        string Note { get; set; }

		ICollection<IContactPhone> PhoneNumbers { get; }
		ICollection<IContactEmail> Emails { get; }
        ICollection<IContactTag> Tags { get; }
	}
}
