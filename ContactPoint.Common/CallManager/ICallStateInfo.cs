using System;

namespace ContactPoint.Common
{
    public interface ICallStateInfo
    {
        CallState State { get; }
        TimeSpan Duration { get; }
    }
}