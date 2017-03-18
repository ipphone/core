using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.Specialized;
using ContactPoint.Common.Audio;
using ContactPoint.Common.SIP.Account;

namespace ContactPoint.Common.SIP
{
    /// <summary>
    /// SIP wrapper interface
    /// </summary>
    public interface ISip
    {
        /// <summary>
        /// SIP account
        /// </summary>
        ISipAccount Account { get; }

        /// <summary>
        /// SIP messanger
        /// </summary>
        ISipMessenger Messenger { get; }

        /// <summary>
        /// All available codecs
        /// </summary>
        List<ISipCodec> Codecs { get; }

        /// <summary>
        /// Network transport type
        /// </summary>
        SipTransportType TransportType { get; set; }

        /// <summary>
        /// Used DTMF conventions type
        /// </summary>
        SipDTMFMode DTMFMode { get; set; }

        /// <summary>
        /// Echo cancelation timeout in ms
        /// </summary>
        int EchoCancelationTimeout { get; set; }

        /// <summary>
        /// Enable or disable voice active detection
        /// </summary>
        bool VoiceActiveDetection { get; set; }
    }
}
