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
using System.Collections.Generic;
using System.ComponentModel;

namespace Sipek.Common
{
    /// <summary>
    /// Account state change delegate
    /// </summary>
    /// <param name="accountId">account identification</param>
    /// <param name="accState">account status</param>
    public delegate void DAccountStateChanged(int accState);

    /// <summary>
    /// Abstract class for registration handling. Contains API method to register accounts and 
    /// event for registration state change notification.
    /// </summary>
    public abstract class IRegistrar
    {
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
        /// Register accounts. Account data taken from Config property
        /// </summary>
        /// <returns></returns>
        public abstract int registerAccounts();

        /// <summary>
        /// Unregister all accounts
        /// </summary>
        /// <returns></returns>
        public abstract int unregisterAccounts();

        /// <summary>
        /// Renew account.
        /// </summary>
        /// <param name="accountIndex">Account index.</param>
        public abstract void renewAccount(int accountIndex);

        #endregion

        #region Events
        /// <summary>
        /// Event AccountStateChanged informs clients about account state changed
        /// </summary>
        public event DAccountStateChanged AccountStateChanged;

        /// <summary>
        /// AccountStateChanged event trigger by VoIP stack when registration state changed
        /// </summary>
        protected void BaseAccountStateChanged(int accState)
        {
            if (null != AccountStateChanged) AccountStateChanged(accState);
        }

        #endregion
    }
}
