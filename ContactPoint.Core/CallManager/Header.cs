using ContactPoint.Common.CallManager;
using Sipek.Sip;

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

    public static class HeaderExtensions
    {
        public static SipHeader ToSipHeader(this IHeader header)
        {
            return new SipHeader { name = header.Name.Substring(0, 64), value = header.Value.Substring(0, 255) };
        }
    }
}
