using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ContactPoint.Common.Contacts.Local
{
    public interface IContactPhoneLocal : IContactPhone, IEntity
    {
        void UpdateFrom(IContactPhone phoneNumber);
        void UpdateKey(string key);
    }
}
