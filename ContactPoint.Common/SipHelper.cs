using System;
using System.Linq;

namespace ContactPoint.Common
{
    public static class SipHelper
    {
        public static string SipCodeDecode(int code)
        {
            switch (code)
            {
                case 100: return "Trying";
                case 180: return "Ringing";
                case 181: return "Call Is Being Forwarded";
                case 182: return "Queued";
                case 183: return "Session Progress";

                case 200: return "OK";

                case 300: return "Multiple Choices";
                case 301: return "Moved Permanently";
                case 302: return "Moved Temporarily";
                case 305: return "Use Proxy";
                case 380: return "Alternative Service";

                case 400: return "Bad Request";
                case 401: return "Unauthorized";
                case 402: return "Payment Required";
                case 403: return "Forbidden";
                case 404: return "Not Found";
                case 405: return "Method Not Allowed";
                case 406: return "Not Acceptable";
                case 407: return "Proxy Authentication Required";
                case 408: return "Request Timeout";
                case 410: return "Gone";
                case 413: return "Request Entity Too Large";
                case 414: return "Requested URL Too Large";
                case 415: return "Unsupported Media Type";
                case 416: return "Unsupported URI1 Scheme";
                case 420: return "Bad Extension";
                case 421: return "Extension Required";
                case 423: return "Interval Too Brief";
                case 480: return "Temporarily Not Available";
                case 481: return "Call Leg or Transaction Does Not Exist";
                case 482: return "Loop Detected";
                case 483: return "Too Many Hops";
                case 484: return "Address Incomplete";
                case 485: return "Ambiguous";
                case 486: return "Busy Here";
                case 487: return "Request Terminated";
                case 488: return "Not Acceptable Here";
                case 491: return "Request Pending";
                case 493: return "Undecipherable";

                // Server-error
                case 500: return "Internal Server Error";
                case 501: return "Not Implemented";
                case 502: return "Bad Gateway";
                case 503: return "Service Unavailable";
                case 504: return "Server Timeout";
                case 505: return "SIP Version Not Supported";
                case 513: return "Message Too Large";

                // Global failure
                case 600: return "Busy Everywhere";
                case 603: return "Decline";
                case 604: return "Does Not Exist Anywhere";
                case 606: return "Not Acceptable";
                default: return "";
            }
        }

        //public static string SipStateDecode(EUserStatus code)
        //{
        //    switch (code)
        //    {
        //        case EUserStatus.AVAILABLE: return "Available";
        //        case EUserStatus.AWAY: return "Away";
        //        case EUserStatus.BRB: return "Be right back";
        //        case EUserStatus.BUSY: return "Busy";
        //        case EUserStatus.IDLE: return "Idle";
        //        case EUserStatus.OPT_MAX:
        //        case EUserStatus.OTP: return "On the phone";
        //        case EUserStatus.OFFLINE:
        //        default: return "Offline";
        //    }
        //}

        public static string SipCallStateDecode(CallState eStateId)
        {
            switch (eStateId)
            {
                case CallState.ACTIVE: return "Active";
                case CallState.ALERTING: return "Alerting";
                case CallState.CONNECTING: return "Connecting";
                case CallState.HOLDING: return "Holding";
                case CallState.IDLE: return "Idle";
                case CallState.INCOMING: return "Incoming";
                case CallState.NULL: return "NULL";
                case CallState.RELEASED: return "Released";
                case CallState.TERMINATED: return "Terminated";
                default: return "UNKNOWN";
            }
        }
    }
}
