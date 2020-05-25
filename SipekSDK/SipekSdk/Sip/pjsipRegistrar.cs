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
 * @see http://sites.google.com/site/sipekvoip
 *  
 */

using System;
using System.Collections.Generic;
using System.Text;
using Sipek.Common;
using System.Runtime.InteropServices;
using System.ComponentModel;

namespace Sipek.Sip
{
    public class pjsipRegistrar : IRegistrar
    {
        #region Dll declarations

        [DllImport(Native.PJSIP_DLL, EntryPoint = "dll_registerAccount", CallingConvention = CallingConvention.Cdecl)]
        private static extern int dll_registerAccount(string uri, string reguri, string domain, string username, string password, string proxy, bool isdefault);
        [DllImport(Native.PJSIP_DLL, EntryPoint = "dll_removeAccounts", CallingConvention = CallingConvention.Cdecl)]
        private static extern int dll_removeAccounts();
        [DllImport(Native.PJSIP_DLL, EntryPoint = "dll_renewAccount", CallingConvention = CallingConvention.Cdecl)]
        private static extern void dll_renewAccount(int accountId);
        [DllImport(Native.PJSIP_DLL, EntryPoint = "onRegStateCallback", CallingConvention = CallingConvention.Cdecl)]
        private static extern int onRegStateCallback(OnRegStateChanged cb);

        #endregion

        #region Constructor
        static private pjsipRegistrar _instance = null;
        static public pjsipRegistrar Instance
        {
            get
            {
                if (_instance == null) _instance = new pjsipRegistrar();
                return _instance;
            }
        }

        private pjsipRegistrar()
        {
            onRegStateCallback(rsDel);
        }
        #endregion

        #region Callback declarations
        // registration state change delegate
        delegate int OnRegStateChanged(int accountId, int regState);

        static OnRegStateChanged rsDel = new OnRegStateChanged(onRegStateChanged);
        #endregion

        #region Public methods
        /////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Register all configured accounts 
        /// </summary>
        /// <returns></returns>
        public override int registerAccounts()
        {
            if (!pjsipStackProxy.Instance.IsInitialized) return -1;

            unregisterAccounts();

            // iterate all accounts
            IAccount acc = Config.Account;
            // check if accounts available
            if (null == acc) return -1;

            // reset account Index field
            Config.Account.Index = -1;
            // reset account state
            Config.Account.RegState = -1;

            if (acc.Enabled && acc.HostName.Length > 0)
            {

                string displayName = acc.DisplayName;
                // warning:::Publish do not work if display name in uri !!!
                string uri = "sip:" + acc.UserName;
                if (acc.UserName.IndexOf("@") < 0)
                {
                    uri += "@" + acc.HostName;
                }
                string reguri = "sip:" + acc.HostName;
                // check transport - if TCP add transport=TCP
                reguri = pjsipStackProxy.Instance.SetTransport(reguri);

                string domain = acc.DomainName;
                string username = acc.UserName;
                string password = acc.Password;

                string proxy = "";
                if (acc.ProxyAddress.Length > 0)
                {
                    proxy = "sip:" + acc.ProxyAddress;
                }

                // Indicate connecting state
                BaseAccountStateChanged(0);

                // store account Id to Index field
                Config.Account.Index = CommonDelegates.SafeInvoke(() => dll_registerAccount(uri, reguri, domain, username, password, proxy, true));
            }

            return 1;
        }

        /// <summary>
        /// Unregister all accounts
        /// </summary>
        /// <returns></returns>
        public override int unregisterAccounts()
        {
            CommonDelegates.SafeInvoke(() => dll_removeAccounts());

            Instance.BaseAccountStateChanged(-1);

            return 1;
        }

        public override void renewAccount(int accountIndex)
        {
            CommonDelegates.SafeInvoke(() => dll_renewAccount(accountIndex));

            Instance.BaseAccountStateChanged(accountIndex);
        }

        /// <summary>
        /// Reception of on registration state change events. First account Id should be mapped to 
        /// account configuration sequence number.
        /// </summary>
        /// <param name="accId"></param>
        /// <param name="regState"></param>
        /// <returns></returns>
        private static int onRegStateChanged(int accountId, int regState)
        {
            // first map account index
            Instance.Config.Account.RegState = regState;
            Instance.BaseAccountStateChanged(regState);

            return 1;
        }
        #endregion
    }
}
