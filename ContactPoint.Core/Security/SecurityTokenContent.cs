using System;

namespace ContactPoint.Core.Security
{
    [Serializable]
    public sealed class SecurityTokenContent
    {
        public Guid Id { get; set; }
        public DateTime? ExpireDate { get; set; }
        public byte[] AssemblyKey { get; set; }
        public Guid[] AllowedOperations { get; set; }
    }
}
