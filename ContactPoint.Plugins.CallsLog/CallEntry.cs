using System;
using ContactPoint.Common;

namespace ContactPoint.Plugins.CallsLog
{
    internal class CallEntry
    {
        public string Name { get; set; }
        public string Number { get; set; }
        public DateTime CallDate { get; set; }
        public TimeSpan Duration { get; set; }
        public bool IsIncoming { get; set; }
        public CallRemoveReason DropReason { get; set; }
        public bool IsAnswered { get; set; }

        public CallEntry(ICall call, CallRemoveReason reason)
        {
            Name = call.Name;
            Number = call.Number;
            CallDate = DateTime.Now - call.Duration;
            Duration = call.Duration;
            IsIncoming = call.IsIncoming;
            DropReason = reason;
            IsAnswered = call.LastState == CallState.ACTIVE || call.LastState == CallState.HOLDING;
        }
    }
}
