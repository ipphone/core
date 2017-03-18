using System;
using System.Collections.Generic;
using System.Text;

namespace ContactPoint.Common
{
    public delegate void EmptyDelegate();
    public delegate void CallDelegate(ICall call);
    public delegate void CallRemovedDelegate(ICall call, CallRemoveReason reason);

    public interface ICallManager : IEnumerable<ICall>
    {
        /// <summary>
        /// Occurs when incoming call
        /// </summary>
        event CallDelegate OnIncomingCall;

        /// <summary>
        /// Occurs when call state is changed
        /// </summary>
        event CallDelegate OnCallStateChanged;

        /// <summary>
        /// Occurs when information about call is changed
        /// </summary>
        event CallDelegate OnCallInfoChanged;

        /// <summary>
        /// Occurs when call dropped
        /// </summary>
        event CallRemovedDelegate OnCallRemoved;

        /// <summary>
        /// Call indexer
        /// </summary>
        /// <param name="sessionId">Call session Id</param>
        /// <returns>Call object or null if not found</returns>
        ICall this[int sessionId] { get; }

        /// <summary>
        /// Currently active call. Must be set by UI because this property indicating "currently used call in UI"! 
        /// </summary>
        ICall ActiveCall { get; set; }

        /// <summary>
        /// Count of currently calls
        /// </summary>
        int Count { get; }

        #region Call control functions

        /// <summary>
        /// Drop specified call
        /// </summary>
        /// <param name="call">Call to drop</param>
        void DropCall(ICall call);

        /// <summary>
        /// Answer specified call
        /// </summary>
        /// <param name="call">Call to answer</param>
        void AnswerCall(ICall call);

        /// <summary>
        /// Put call on hold
        /// </summary>
        /// <param name="call">Call to put on hold</param>
        void HoldCall(ICall call);

        /// <summary>
        /// Retrive call from hold
        /// </summary>
        /// <param name="call">Call to retrive from hold</param>
        void UnHoldCall(ICall call);

        /// <summary>
        /// Toggle hold state on call
        /// </summary>
        /// <param name="call">Call to hold or unhold</param>
        void ToggleHoldCall(ICall call);

        /// <summary>
        /// Send DTMF to call with default DTMF mode
        /// </summary>
        /// <param name="call">Call to send DTMF</param>
        /// <param name="digits">Digits to send</param>
        void SendDTMF(ICall call, string digits);

        /// <summary>
        /// Create new call to specified number
        /// </summary>
        /// <param name="number">Number to call</param>
        /// <returns>New call</returns>
        ICall MakeCall(string number);

        /// <summary>
        /// Create new call to specified number
        /// </summary>
        /// <param name="number">Number to call</param>
        /// <param name="headers">Headers to pass with call</param>
        /// <returns>New call</returns>
        ICall MakeCall(string number, IEnumerable<KeyValuePair<string, string>> headers);

        /// <summary>
        /// Transfer call to number
        /// </summary>
        /// <param name="call">Call to transfer</param>
        /// <param name="number">Destination number</param>
        void TransferCall(ICall call, string number);

        /// <summary>
        /// Transfer call to active call
        /// </summary>
        /// <param name="call">Call to transfer</param>
        /// <param name="destCall">Destination call</param>
        void TransferAttendedCall(ICall call, ICall destCall);

        /// <summary>
        /// Make conference between all calls that in holding and active states
        /// </summary>
        void DoMakeConference();

        #endregion
    }
}
