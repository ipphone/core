using System;
using System.Collections.Generic;
using System.Text;

namespace ContactPoint.Common
{
    public enum CallRemoveReason
    {
        NULL,
        UserHangup,
        RemoteHangup,
        Transfer,
        Busy,
        InvalidData
    }
}
