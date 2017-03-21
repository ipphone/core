using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ContactPoint.Common.Contacts
{
    public interface IContactTag : IVersionable
    {
        string Key { get; }
        string Name { get; set; }
        string Color { get; set; }
    }
}
