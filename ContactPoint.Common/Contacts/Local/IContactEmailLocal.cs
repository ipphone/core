using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ContactPoint.Common.Contacts.Local
{
    public interface IContactEmailLocal : IContactEmail, IEntity
    {
        void UpdateFrom(IContactEmail contactEmail);
        void UpdateKey(string key);
    }
}
