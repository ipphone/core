/* 
 * Copyright (C) 2008 Sasa Coh <sasacoh@gmail.com>
 *
 * This program is free software; you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation; either version 2 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA 
 * 
 * @see http://sipekphone.googlepages.com/pjsipwrapper
 * @see http://voipengine.googlepages.com/
 *  
 */

using System;

namespace Sipek.Common
{
    public delegate void DMessageReceived(string from, string text);
    public delegate void DBuddyStatusChanged(int buddyId, int status, string text);

    /// <summary>
    /// Abstract class for presence and instant messaging handling
    /// </summary>
    public abstract class IPresenceAndMessaging
    {
        #region Events
        /// <summary>
        /// Buddy status changed notifier
        /// </summary>
        public event DBuddyStatusChanged BuddyStatusChanged;
        /// <summary>
        /// Message received notifier
        /// </summary>
        public event DMessageReceived MessageReceived;
        #endregion

        #region Properties
        private IConfiguratorInterface _config = new NullConfigurator();
        public IConfiguratorInterface Config
        {
            get
            {
                return _config;
            }
            set
            {
                _config = value;
            }
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Add buddy to buddy list and subscribe presence
        /// </summary>
        /// <param name="ident">buddy identification</param>
        /// <returns></returns>
        public abstract int addBuddy(string ident, bool presence);

        /// <summary>
        /// Delete buddy with given identification
        /// </summary>
        /// <param name="buddyId">buddy identification</param>
        /// <returns></returns>
        public abstract int delBuddy(int buddyId);

        /// <summary>
        /// Send Instant Message
        /// </summary>
        /// <param name="destAddress">Destination part of URI</param>
        /// <param name="message">Message Content</param>
        /// <returns></returns>
        public abstract int sendMessage(string destAddress, string message);

        /// <summary>
        /// Set device status for default account 
        /// </summary>
        /// <param name="accId">Account id</param>
        /// <param name="presence_state">Presence state - User Status</param>
        /// <returns></returns>
        public abstract int setStatus(EUserStatus presence_state);
        #endregion

        #region Private Methods
        /// <summary>
        /// MessageReceived event trigger by VoIP stack when instant message arrived
        /// </summary>
        protected void BaseMessageReceived(string from, string text)
        {
            if (null != MessageReceived) MessageReceived(from, text);
        }
        /// <summary>
        /// BuddyStatusChanged event trigger by VoIP stack when buddy status changed
        /// </summary>
        protected void BaseBuddyStatusChanged(int buddyId, int status, string text)
        {
            if (null != BuddyStatusChanged) BuddyStatusChanged(buddyId, status, text);
        }
        #endregion
    }

}
