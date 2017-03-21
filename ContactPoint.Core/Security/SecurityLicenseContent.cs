using System;

namespace ContactPoint.Core.Security
{
    [Serializable]
    public sealed class SecurityLicenseContent
    {
        public Guid Id { get; set; }
        public SecurityTokenContent[] Tokens { get; set; }
        public string Company { get; set; }
        public DateTime ActivationDate { get; set; }
        public byte[] MachineId { get; set; }

        public static readonly SecurityLicenseContent Empty = new SecurityLicenseContent()
        {
            Id = Guid.Empty,
            Tokens = new SecurityTokenContent[0],
            Company = "ContactPoint",
            ActivationDate = new DateTime(2005, 1, 1),
            MachineId = new byte[0]
        };
    }
}
