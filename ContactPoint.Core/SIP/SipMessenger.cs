using System;
using System.Linq;
using ContactPoint.Common;
using ContactPoint.Common.SIP;

namespace ContactPoint.Core.SIP
{
    class SipMessenger : ISipMessenger
    {
        public event Action<string, string> MessageReceived;

        public SipMessenger(SIP sip)
        {
            sip.SipekResources.Messenger.MessageReceived += MessengerOnMessageReceived;
        }

        private void MessengerOnMessageReceived(string from, string message)
        {
            Logger.LogNotice($"SIP message received from '{from}': {message}");
            foreach (var handler in MessageReceived?.GetInvocationList() ?? Enumerable.Empty<Delegate>())
            {
                try
                {
                    handler.DynamicInvoke(from, message);
                }
                catch (Exception e)
                {
                    Logger.LogWarn(e, "Exception occured while processing 'MessageReceived' event");
                }
            }
        }
    }
}
