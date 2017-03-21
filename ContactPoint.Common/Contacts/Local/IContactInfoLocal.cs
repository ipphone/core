using System;
using System.Collections.Generic;

namespace ContactPoint.Common.Contacts.Local
{
    public interface IContactInfoLocal : IContactInfo, IEntity
    {
        event Action<IContactInfoLocal> Changed;
        
        IAddressBookLocal AddressBook { get; }
        IContact Contact { get; }

        new ICollection<IContactTagLocal> Tags { get; }
        new ICollection<IContactPhoneLocal> PhoneNumbers { get; }
        new ICollection<IContactEmailLocal> Emails { get; }

        void Submit();
        void Remove();
    }
}
