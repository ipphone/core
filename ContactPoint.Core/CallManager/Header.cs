using System;
using System.Collections.Generic;
using System.Text;
using ContactPoint.Common.CallManager;

namespace ContactPoint.Core.CallManager
{
    internal class Header : IHeader
    {
        public string Name { get; private set; }
        public string Value { get; private set; }

        public Header(string name, string value)
        {
            Name = name;
            Value = value;
        }
    }
}
