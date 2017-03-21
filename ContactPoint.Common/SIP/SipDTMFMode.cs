using System;
using System.Collections.Generic;
using System.Text;

namespace ContactPoint.Common.SIP
{
    public enum SipDTMFMode : int
    {
        RFC2833 = 0,
        Transparent = 1,
        OutOfBand = 2
    }
}
