using System;
using System.Collections.Generic;
using ContactPoint.Common.CallManager;
using ContactPoint.Common.Contacts;

namespace ContactPoint.Common
{
    public interface ICall
    {
        Guid Id { get; }
        int SessionId { get; }
        int Line { get; }
        CallState State { get; }
        CallState LastState { get; }
        IEnumerable<ICallStateInfo> States { get; }
        CallAction LastUserAction { get; }
        TimeSpan Duration { get; }
        TimeSpan ActiveDuration { get; }
        string Name { get; set; }
        string Number { get; }
        IContact Contact { get; }
        string Info { get; }
        string Codec { get; }
        bool IsInConference { get; }
        bool IsIncoming { get; }
        bool IsDisposed { get; }
        IHeaderCollection Headers { get; }
        IDictionary<string, object> Tags { get; }

        /// <summary>
        /// Occurs when call state is changed
        /// </summary>
        event EmptyDelegate OnStateChanged;

        /// <summary>
        /// Occurs when information about call is changed
        /// </summary>
        event EmptyDelegate OnInfoChanged;

        /// <summary>
        /// Occurs when call dropped
        /// </summary>
        event Action<CallRemoveReason> OnRemoved;

        /// <summary>
        /// Duration increased
        /// </summary>
        event EmptyDelegate OnDurationChanged;

        /// <summary>
        /// Drop this call
        /// </summary>
        void Drop();

        /// <summary>
        /// Answer this call
        /// </summary>
        void Answer();

        /// <summary>
        /// Hold this call
        /// </summary>
        void Hold();

        /// <summary>
        /// UnHold this call
        /// </summary>
        void UnHold();

        /// <summary>
        /// Toggle hold state on call
        /// </summary>
        void ToggleHold();

        /// <summary>
        /// Send DTMF to this call
        /// </summary>
        /// <param name="digits">Digits to send</param>
        void SendDTMF(string digits);

        /// <summary>
        /// Transfer this call
        /// </summary>
        /// <param name="number">Destination number</param>
        void Transfer(string number);

        /// <summary>
        /// Transfer this call to another active call
        /// </summary>
        /// <param name="destCall">Destination call</param>
        void TransferAttended(ICall destCall);
    }
}
