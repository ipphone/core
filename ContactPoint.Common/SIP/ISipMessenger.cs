using System;

namespace ContactPoint.Common.SIP
{
    public interface ISipMessenger
    {
        event Action<string, string> MessageReceived;
    }
}
