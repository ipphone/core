using System.Drawing;

namespace ContactPoint.Common.SIP.Account
{
    /// <summary>
    /// Presence status
    /// </summary>
    public class PresenceStatus
    {
        /// <summary>
        /// Human description of presence status
        /// </summary>
        public string Message { get; }

        /// <summary>
        /// Status code
        /// </summary>
        public PresenceStatusCode Code { get; }

        /// <summary>
        /// Custom presence icon
        /// 
        /// Can bu NULL
        /// </summary>
        public Bitmap Icon { get; }

        /// <summary>
        /// Contrsuctor. Creating unknown presence status without text message
        /// </summary>
        public PresenceStatus() : this(PresenceStatusCode.Unknown)
        { }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="code">Presence code</param>
        /// <param name="message">Human description of presence status</param>
        public PresenceStatus(PresenceStatusCode code, string message = null) : this(code, message, null)
        { }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="code">Presence code</param>
        /// <param name="message">Human description of presence status</param>
        /// <param name="icon">Custom presence status icon</param>
        public PresenceStatus(PresenceStatusCode code, string message, Bitmap icon)
        {
            Code = code;
            Message = message;
            Icon = icon;
        }
    }
}
