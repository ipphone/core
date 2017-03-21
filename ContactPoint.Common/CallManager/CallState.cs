namespace ContactPoint.Common
{
    public enum CallState : int
    {
        NULL = 0x0,
        IDLE = 0x1,
        CONNECTING = 0x2,
        ALERTING = 0x4,
        ACTIVE = 0x8,
        RELEASED = 0x10,
        INCOMING = 0x20,
        HOLDING = 0x40,
        TERMINATED = 0x80
    }
}
