using System;
using System.Collections.Generic;
using System.Text;
using System.Net;

namespace ContactPoint.Common.SIP.Account
{
    /// <summary>
    /// SIP account
    /// </summary>
    public interface ISipAccount
    {
        /// <summary>
        /// Occurs when account registration state is changed
        /// </summary>
        event Action<ISipAccount> RegisterStateChanged;

        /// <summary>
        /// Occurs when account presence status is changed
        /// </summary>
        event Action<ISipAccount> PresenceStatusChanged;

        /// <summary>
        /// SIP wrapper object
        /// </summary>
        ISip Sip { get; }

        /// <summary>
        /// Friendly name of account
        /// </summary>
        string FullName { get; set; }

        /// <summary>
        /// User login
        /// </summary>
        string UserName { get; set; }

        /// <summary>
        /// User password
        /// </summary>
        string Password { get; set; }
        
        /// <summary>
        /// Server to connect
        /// </summary>
        string Server { get; set; }

        /// <summary>
        /// Server's SIP realm
        /// </summary>
        string Realm { get; set; }

        /// <summary>
        /// SIP port to use
        /// </summary>
        int Port { get; set; }

        /// <summary>
        /// RTP starting port. We use 64 ports starting from given
        /// </summary>
        int RtpPort { get; set; }

        /// <summary>
        /// Account register timeout
        /// </summary>
        int RegisterTimeout { get; set; }

        /// <summary>
        /// Current registration process state
        /// </summary>
        SipAccountState RegisterState { get; }

        /// <summary>
        /// Duration of current registration state
        /// </summary>
        TimeSpan RegisterStateDuration { get; }

        /// <summary>
        /// Current registration process state code
        /// </summary>
        int RegisterStateCode { get; }

        /// <summary>
        /// Current presence status of account
        /// </summary>
        PresenceStatus PresenceStatus { get; set; }

        /// <summary>
        /// Duration of current presence status
        /// </summary>
        TimeSpan PresenceStatusDuration { get; }

        /// <summary>
        /// Register account on the server
        /// </summary>
        void Register();

        /// <summary>
        /// Unregister account
        /// </summary>
        void UnRegister();

        /// <summary>
        /// Refresh account state.
        /// </summary>
        void Renew();
    }
}
