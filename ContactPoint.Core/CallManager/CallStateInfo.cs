using System;
using ContactPoint.Common;

namespace ContactPoint.Core.CallManager
{
    class CallStateInfo : ICallStateInfo
    {
        public static readonly CallStateInfo Default = new CallStateInfo { State = CallState.NULL, Duration = TimeSpan.Zero };

        public CallState State { get; set; }
        public TimeSpan Duration { get; set; }
    }
}