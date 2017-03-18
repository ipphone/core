using System;
using System.Collections.Generic;
using System.Text;

namespace ContactPoint.Common.SIP.Account
{
    /// <summary>
    /// Connection state of SIP account
    /// </summary>
    public enum SipAccountState
    {
        Offline = -1,
        Connecting = 0,
        Online = 200
    }
}
