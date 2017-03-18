using System.ComponentModel;

namespace ContactPoint.Common
{
    /// <summary>
    /// Interface to allow some actions on UI from plugins.
    /// </summary>
    public interface IUi : ISynchronizeInvoke
    {
        /// <summary>
        /// Make specific call active.
        /// </summary>
        /// <param name="call">Target call.</param>
        void ActivateCall(ICall call);

        /// <summary>
        /// Gets phone number that has been typed in the textbox
        /// </summary>
        /// <returns>Phone number</returns>
        string GetPhoneNumber();
    }
}
