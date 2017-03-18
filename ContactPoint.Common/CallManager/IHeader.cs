using System;
using System.Collections.Generic;
using System.Text;

namespace ContactPoint.Common.CallManager
{
    public interface IHeader
    {
        string Name { get; }
        string Value { get; }
    }
}
