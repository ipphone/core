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
using System.Runtime.InteropServices;

namespace Sipek.Sip
{
    #region Config Structure

    /// <summary>
    /// Sip Config structure. 
    /// BE CAREFUL!
    /// SYNCHRONIZE FIELDS WITH C-STRUCTURE IN PJSIPDLL.H!!!!!
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public class SipConfigStruct
    {
        private static SipConfigStruct _instance = null;
        public static SipConfigStruct Instance
        {
            get
            {
                if (_instance == null) _instance = new SipConfigStruct();
                return _instance;
            }
        }

        public int listenPort = 5060;
        [MarshalAs(UnmanagedType.I1)]   // warning:::Marshal managed bool type to unmanaged (C) bool !!!!
        public bool noUDP = false;
        [MarshalAs(UnmanagedType.I1)]
        public bool noTCP = true;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 255)]
        public string stunServer;
        [MarshalAs(UnmanagedType.I1)]
        public bool publishEnabled = false;

        public int expires = 3600;

        [MarshalAs(UnmanagedType.I1)]
        public bool VADEnabled = true;

        public int ECTail = 200;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 255)]
        public string nameServer;

        [MarshalAs(UnmanagedType.I1)]
        public bool pollingEventsEnabled = false;

#if DEBUG
        public int logLevel = 10;
#else
        public int logLevel = 0;
#endif

        // IMS specifics
        [MarshalAs(UnmanagedType.I1)]
        public bool imsEnabled = false; // secAgreement rfc 3329
        [MarshalAs(UnmanagedType.I1)]
        public bool imsIPSecHeaders = false;
        [MarshalAs(UnmanagedType.I1)]
        public bool imsIPSecTransport = false;

        public int rtpPort = 10000;
    }

    #endregion
}
