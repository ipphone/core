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

namespace Sipek.Common
{
    /// <summary>
    /// IConfiguratorInterface defines data access interface.
    /// </summary>
    public interface IConfiguratorInterface
    {
        /// <summary>
        /// Do Not Disturb Property
        /// </summary>
        bool DNDFlag { get; set; }
        /// <summary>
        /// Auto Answer property
        /// </summary>
        bool AAFlag { get; set; }
        /// <summary>
        /// Call Forwarding Unconditional property
        /// </summary>
        bool CFUFlag { get; set; }
        /// <summary>
        /// Call Forwarding Unconditional Number property
        /// </summary>
        string CFUNumber { get; set; }
        /// <summary>
        /// Call Forwarding No Reply property
        /// </summary>
        bool CFNRFlag { get; set; }
        /// <summary>
        /// Call Forwarding No Reply Number property
        /// </summary>
        string CFNRNumber { get; set; }
        /// <summary>
        /// Call Forwarding Busy property
        /// </summary>
        bool CFBFlag { get; set; }
        /// <summary>
        /// Call Forwarding Busy Number property
        /// </summary>
        string CFBNumber { get; set; }
        /// <summary>
        /// Sip listening port property
        /// </summary>
        int SIPPort { get; set; }
        /// <summary>
        /// Starting RTP port
        /// </summary>
        int RTPPort { get; set; }
        /// <summary>
        /// Internal representation of account identification. Assigned by voip stack.
        /// </summary>
        int DefaultAccountIndex { get; }
        /// <summary>
        /// List of all codecs
        /// </summary>
        List<string> CodecList { get; set; }
        /// <summary>
        /// Flag to enable publish method (user status)
        /// </summary>
        bool PublishEnabled { get; set; }

        /// <summary>
        /// Play audio file on incoming call
        /// </summary>
        bool AudioPlayOnIncoming { get; set; }

        /// <summary>
        /// Play audio file on incoming call while active call
        /// </summary>
        bool AudioPlayOnIncomingAndActive { get; set; }

        /// <summary>
        /// Play audio file for outgoing calls. Note that this sound should be played by remote endpoint
        /// </summary>
        bool AudioPlayOutgoing { get; set; }

        IAccount Account { get; }

        bool IsNull { get; }

        /// <summary>
        /// Indicate that softphone is on pause and all incoming calls should be dropped
        /// </summary>
        bool PauseFlag { get; set; }

        #region Public Methods

        /// <summary>
        /// Save settings 
        /// </summary>
        void Save();
        #endregion Methods
    }

    /// <summary>
    /// IAccount interface
    /// </summary>
    public interface IAccount
    {
        /// <summary>
        /// Account enabled/disabled flag
        /// </summary>
        bool Enabled { get; set; }
        /// <summary>
        /// Represents a value assigned to an account by a sip stack.   
        /// </summary>
        int Index { get; set; }
        /// <summary>
        /// Account name
        /// </summary>
        string AccountName { get; set; }
        /// <summary>
        /// Account host name
        /// </summary>
        string HostName { get; set; }
        /// <summary>
        /// Account Id = Username
        /// </summary>
        string Id { get; set; }
        /// <summary>
        /// Account username
        /// </summary>
        string UserName { get; set; }
        /// <summary>
        /// Account password
        /// </summary>
        string Password { get; set; }
        /// <summary>
        /// Account display
        /// </summary>
        string DisplayName { get; set; }
        /// <summary>
        /// Account Domain name
        /// </summary>
        string DomainName { get; set; }
        /// <summary>
        /// Account current state (temporary data)
        /// </summary>
        int RegState { get; set; }
        /// <summary>
        /// Account Proxy Address (optional)
        /// </summary>
        string ProxyAddress { get; set; }
        /// <summary>
        /// VoIP Transport mode
        /// </summary>
        ETransportMode TransportMode { get; set; }
    }


    #region Null Pattern
    /// <summary>
    /// 
    /// </summary>
    internal class NullConfigurator : IConfiguratorInterface
    {
        public NullConfigurator()
        {
            // add 1 account
            //_accountList.Add(new NullAccount());
        }

        public class NullAccount : IAccount
        {
            public bool Enabled { get { return false; } set { } }

            public int Index
            {
                get { return 0; }
                set { }
            }


            public string AccountName
            {
                get { return ""; }
                set { }
            }

            public string HostName
            {
                get { return ""; }
                set { }
            }

            public string Id
            {
                get { return ""; }
                set { }
            }

            public string UserName
            {
                get { return ""; }
                set { }
            }

            public string Password
            {
                get { return ""; }
                set { }
            }

            public string DisplayName
            {
                get { return ""; }
                set { }
            }

            public string DomainName
            {
                get { return ""; }
                set { }
            }

            public int Port
            {
                get { return 0; }
                set { }
            }

            public int RegState
            {
                get { return 0; }
                set { }
            }

            public string ProxyAddress
            {
                get { return ""; }
                set { }
            }

            public ETransportMode TransportMode { get { return ETransportMode.TM_UDP; } set { } }
        }

        public bool AudioPlayOnIncoming
        {
            get { return false; }
            set { }
        }

        public bool AudioPlayOnIncomingAndActive
        {
            get { return false; }
            set { }
        }

        public bool AudioPlayOutgoing
        {
            get { return false; }
            set { }
        }

        #region IConfiguratorInterface Properties

        public bool IsNull { get { return true; } }
        
        public bool PauseFlag
        {
            get { return false; }
            set { }
        }

        public bool CFUFlag
        {
            get { return false; }
            set { }
        }

        public string CFUNumber
        {
            get { return ""; }
            set { }
        }

        public bool CFNRFlag
        {
            get { return false; }
            set { }
        }

        public string CFNRNumber
        {
            get { return ""; }
            set { }
        }

        public bool CFBFlag
        {
            get { return false; }
            set { }
        }

        public string CFBNumber
        {
            get { return ""; }
            set { }
        }

        public bool DNDFlag
        {
            get { return false; }
            set { }
        }

        public bool AAFlag
        {
            get { return false; }
            set { }
        }
        public int SIPPort
        {
            get { return 5060; }
            set { }
        }
        public int RTPPort
        {
            get { return 4000; }
            set { }
        }
        public int DefaultAccountIndex
        {
            get { return 0; }
            set { }
        }

        public bool PublishEnabled
        {
            get { return true; }
            set { }
        }

        public IAccount Account
        {
            get { return new NullAccount(); }
        }

        public void Save()
        { }

        public List<string> CodecList { get { return null; } set { } }

        #endregion
    }
    #endregion

}
