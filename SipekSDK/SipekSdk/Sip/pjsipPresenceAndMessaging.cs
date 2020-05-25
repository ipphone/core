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

namespace Sipek.Sip
{

    public class pjsipPresenceAndMessaging : IPresenceAndMessaging
    {
        [DllImport(Native.PJSIP_DLL, EntryPoint = "dll_addBuddy", CallingConvention = CallingConvention.Cdecl)]
        private static extern int dll_addBuddy(string uri, bool subscribe);
        [DllImport(Native.PJSIP_DLL, EntryPoint = "dll_removeBuddy", CallingConvention = CallingConvention.Cdecl)]
        private static extern int dll_removeBuddy(int buddyId);
        [DllImport(Native.PJSIP_DLL, EntryPoint = "dll_sendMessage", CallingConvention = CallingConvention.Cdecl)]
        private static extern int dll_sendMessage(int buddyId, string uri, string message);
        [DllImport(Native.PJSIP_DLL, EntryPoint = "dll_setStatus", CallingConvention = CallingConvention.Cdecl)]
        private static extern int dll_setStatus(int accId, int presenceState);

        delegate int OnMessageReceivedCallback(string from, string message);
        delegate int OnBuddyStatusChangedCallback(int buddyId, int status, string statusText);

        [DllImport(Native.PJSIP_DLL, CallingConvention = CallingConvention.Cdecl, BestFitMapping = true, EntryPoint = "onMessageReceivedCallback", ExactSpelling = false, PreserveSig = false, ThrowOnUnmappableChar = false, SetLastError = false)]
        private static extern int onMessageReceivedCallback(OnMessageReceivedCallback cb);
        [DllImport(Native.PJSIP_DLL, CallingConvention = CallingConvention.Cdecl, BestFitMapping = true, EntryPoint = "onBuddyStatusChangedCallback", ExactSpelling = false, PreserveSig = false, ThrowOnUnmappableChar = false, SetLastError = false)]
        private static extern int onBuddyStatusChangedCallback(OnBuddyStatusChangedCallback cb);

        static readonly OnMessageReceivedCallback MessageReceivedCallback = OnMessageReceived;
        static readonly OnBuddyStatusChangedCallback BuddyStatusChangedCallback = OnBuddyStatusChanged;

        private static pjsipPresenceAndMessaging _instance;
        public static pjsipPresenceAndMessaging Instance => _instance ?? (_instance = new pjsipPresenceAndMessaging());

        private pjsipPresenceAndMessaging()
        {
            onBuddyStatusChangedCallback(BuddyStatusChangedCallback);
            onMessageReceivedCallback(MessageReceivedCallback);
        }

        /// <summary>
        /// Add new entry in a buddy list and subscribe presence
        /// </summary>
        /// <param name="name">Buddy address (without hostname part</param>
        /// <param name="presence">subscribe presence flag</param>
        /// <returns></returns>
        public override int addBuddy(string name, bool presence)
        {
            string sipuri;
            if (!pjsipStackProxy.Instance.IsInitialized) return -1;

            // check if name contains URI
            if (name != null && name.IndexOf("sip:", StringComparison.Ordinal) == 0)
            {
                // do nothing...
                sipuri = name;
            }
            else
            {
                sipuri = "sip:" + name + "@" + Config.Account.HostName;
            }
            // check transport - if TCP add transport=TCP
            sipuri = pjsipStackProxy.Instance.SetTransport(sipuri);

            return CommonDelegates.SafeInvoke(() => dll_addBuddy(sipuri, presence));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="buddyId"></param>
        /// <returns></returns>
        public override int delBuddy(int buddyId)
        {
            return CommonDelegates.SafeInvoke(() => dll_removeBuddy(buddyId));
        }

        /// <summary>
        /// Send an instance message
        /// </summary>
        /// <param name="destAddress"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public override int sendMessage(string destAddress, string message)
        {
            if (!pjsipStackProxy.Instance.IsInitialized) return -1;

            string sipuri = "";

            // check if name contains URI
            if (destAddress != null && destAddress.IndexOf("sip:", StringComparison.Ordinal) == 0)
            {
                // do nothing...
                sipuri = destAddress;
            }
            else
            {
                sipuri = "sip:" + destAddress + "@" + Config.Account.HostName;
            }
            // set transport
            sipuri = pjsipStackProxy.Instance.SetTransport(sipuri);

            return CommonDelegates.SafeInvoke(() => dll_sendMessage(Config.Account.Index, sipuri, message));
        }

        /// <summary>
        /// Set presence status
        /// </summary>
        /// <param name="status"></param>
        /// <returns></returns>
        public override int setStatus(EUserStatus status)
        {
            if (!pjsipStackProxy.Instance.IsInitialized) return -1;

            if (Config.Account.RegState != 200) return -1;
            if (!Config.PublishEnabled) return -1;

            return CommonDelegates.SafeInvoke(() => dll_setStatus(Config.Account.Index, (int)status));
        }

        private static int OnMessageReceived(string from, string text)
        {
            Instance.BaseMessageReceived(from, text);
            return 1;
        }

        private static int OnBuddyStatusChanged(int buddyId, int status, string text)
        {
            Instance.BaseBuddyStatusChanged(buddyId, status, text);
            return 1;
        }
    }
}
