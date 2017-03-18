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
 * WaveLib library sources http://www.codeproject.com/KB/graphics/AudioLib.aspx
 * 
 * Visit SipekSDK page at http://voipengine.googlepages.com/
 * 
 * Visit SIPek's home page at http://sipekphone.googlepages.com/ 
 * 
 */

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Runtime.InteropServices;
using System.Media;
using System.Windows.Threading;
using Sipek.Common;
using Sipek.Common.CallControl;
using System.ComponentModel;
using System.Collections.Specialized;
using ContactPoint.Common.Audio;
using ContactPoint.Common;

namespace Sipek.Sip
{
    /// <summary>
    /// ConcreteFactory 
    /// Implementation of AbstractFactory. 
    /// </summary>
    public class SipekResources : IAbstractFactory
    {
        private readonly ICore _core; // reference to MainForm to provide timer context
        private readonly SipekConfigurator _config;
        private readonly ManualResetEvent _syncThreadFlag = new ManualResetEvent(false);
        private pjsipStackProxy _stackProxy;

        #region Constructor
        public SipekResources(ICore core)
        {
            _core = core;

            var syncThread = new Thread(SyncThread) {IsBackground = true};
            syncThread.SetApartmentState(ApartmentState.STA);
            syncThread.Start();

            if (!_syncThreadFlag.WaitOne(TimeSpan.FromSeconds(30)))
                throw new InvalidOperationException("Can't start PjSIP sync thread! Timeout exceeded");

            _config = new SipekConfigurator(_core);

            _stackProxy = pjsipStackProxy.Instance;
            _callManager = CCallManager.Instance;
            _messenger = pjsipPresenceAndMessaging.Instance;
            _registrar = pjsipRegistrar.Instance;

            // initialize sip struct at startup
            SipConfigStruct.Instance.stunServer = this.Configurator.StunServerAddress;
            SipConfigStruct.Instance.publishEnabled = this.Configurator.PublishEnabled;
            SipConfigStruct.Instance.expires = this.Configurator.Expires;
            SipConfigStruct.Instance.VADEnabled = this.Configurator.VADEnabled;
            SipConfigStruct.Instance.ECTail = this.Configurator.ECTail;
            SipConfigStruct.Instance.nameServer = this.Configurator.NameServer;

#if !DEBUG
            SipConfigStruct.Instance.logLevel = Logger.LogLevel*5;
#endif

            // initialize modules
            _callManager.StackProxy = _stackProxy;
            _callManager.Config = _config;
            _callManager.Factory = this;
            _stackProxy.Config = _config;
            _registrar.Config = _config;
            _messenger.Config = _config;

            _core.SettingsManager["cfgSipAccountState"] = "0";
            _core.SettingsManager["cfgSipAccountIndex"] = "0";
        }
        #endregion Constructor

        #region AbstractFactory methods
        public ITimer CreateTimer()
        {
            return new GUITimer();
        }

        public IStateMachine CreateStateMachine()
        {
            return new CStateMachine();
        }

        public IStateMachine createStateMachine(ISynchronizeInvoke syncInvoke)
        {
            return new CStateMachine();
        }

        #endregion

        #region Other Resources
        public ICore Core
        {
            get { return _core; }
        }

        public pjsipStackProxy StackProxy
        {
            get { return _stackProxy; }
            set { _stackProxy = value; }
        }

        public SipekConfigurator Configurator
        {
            get { return _config; }
            set { }
        }

        private IRegistrar _registrar;
        public IRegistrar Registrar
        {
            get { return _registrar; }
        }

        private IPresenceAndMessaging _messenger;
        public IPresenceAndMessaging Messenger
        {
            get { return _messenger; }
        }

        private CCallManager _callManager;
        public CCallManager CallManager
        {
            //get { return CCallManager.Instance; }
            get { return _callManager; }
        }

        #endregion

        #region Sync thread

        private void SyncThread()
        {
            CommonDelegates.Initialize(Dispatcher.CurrentDispatcher);

            _syncThreadFlag.Set();
            Dispatcher.Run();
        }

        #endregion
    }

    #region Concrete implementations

    public class GUITimer : ITimer
    {
        System.Timers.Timer _guiTimer;

        public GUITimer()
        {
            _guiTimer = new System.Timers.Timer();
            if (this.Interval > 0) _guiTimer.Interval = this.Interval;
            _guiTimer.Interval = 100;
            _guiTimer.Enabled = true;
            _guiTimer.Elapsed += new ElapsedEventHandler(_guiTimer_Tick);
        }

        void _guiTimer_Tick(object sender, EventArgs e)
        {
            _guiTimer.Stop();
            //_elapsed(sender, e);
            // Synchronize thread with GUI because SIP stack works with GUI thread only

            // TODO: check! May occur disposing problems
            //if (/*(_form.Form.IsDisposed) || (_form.Form.Disposing) ||*/ (!_core.IsInitialized))
            //    return;

            CommonDelegates.SafeBeginInvoke(() => { if (_elapsed != null) _elapsed.Invoke(sender, e); });
        }

        public bool Start()
        {
            _guiTimer.Start();
            return true;
        }

        public bool Stop()
        {
            _guiTimer.Stop();
            return true;
        }

        private int _interval;
        public int Interval
        {
            get { return _interval; }
            set { _interval = value; _guiTimer.Interval = value; }
        }

        private TimerExpiredCallback _elapsed;
        public TimerExpiredCallback Elapsed
        {
            set
            {
                _elapsed = value;
            }
        }
    }


    // Accounts
    public class SipekAccount : IAccount
    {
        private readonly ICore _core;

        public bool Enabled
        {
            get
            {
                // Deperecated option
                return true;

                //bool value;
                //if (this._core.SettingsManager["cfgSipAccountEnabled"] != null && Boolean.TryParse(this._core.SettingsManager["cfgSipAccountEnabled"].ToString(), out value))
                //{
                //    return value;
                //}
                //return false;
            }

            set
            {
                //this._core.SettingsManager["cfgSipAccountEnabled"] = value;
            }
        }

        /// <summary>
        /// Temp storage!
        /// The account index assigned by voip stack
        /// </summary>
        public int Index
        {
            get
            {
                int value;
                if (this._core.SettingsManager["cfgSipAccountIndex"] != null && Int32.TryParse(this._core.SettingsManager["cfgSipAccountIndex"].ToString(), out value))
                {
                    return value;
                }
                return -1;
            }
            set 
            {
                this._core.SettingsManager["cfgSipAccountIndex"] = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index">the account identification used by configuration (values 0..4)</param>
        public SipekAccount(ICore core)
        {
            _core = core;
        }

        #region Properties

        public string AccountName
        {
            get
            {
                if (this._core.SettingsManager["cfgSipAccountName"] != null)
                    return this._core.SettingsManager["cfgSipAccountName"].ToString();
                else
                    return "";
            }
            set
            {
                this._core.SettingsManager["cfgSipAccountName"] = value;
            }
        }

        public string HostName
        {
            get
            {
                if (this._core.SettingsManager["cfgSipAccountAddress"] != null)
                    return this._core.SettingsManager["cfgSipAccountAddress"].ToString();
                else
                    return "";
            }
            set
            {
                this._core.SettingsManager["cfgSipAccountAddress"] = value;
            }
        }

        public string Id
        {
            get
            {
                if (this._core.SettingsManager["cfgSipAccountId"] != null)
                    return this._core.SettingsManager["cfgSipAccountId"].ToString();
                else
                    return "";
            }
            set
            {
                this._core.SettingsManager["cfgSipAccountId"] = value;
            }
        }

        public string UserName
        {
            get
            {
                if (this._core.SettingsManager["cfgSipAccountUsername"] != null)
                    return this._core.SettingsManager["cfgSipAccountUsername"].ToString();
                else
                    return "";
            }
            set
            {
                this._core.SettingsManager["cfgSipAccountUsername"] = value;
            }
        }

        public string Password
        {
            get
            {
                if (this._core.SettingsManager["cfgSipAccountPassword"] != null)
                    return this._core.SettingsManager["cfgSipAccountPassword"].ToString();
                else
                    return "";
            }
            set
            {
                this._core.SettingsManager["cfgSipAccountPassword"] = value;
            }
        }

        public string DisplayName
        {
            get
            {
                if (this._core.SettingsManager["cfgSipAccountDisplayName"] != null)
                    return this._core.SettingsManager["cfgSipAccountDisplayName"].ToString();

                return "";
            }
            set
            {
                this._core.SettingsManager["cfgSipAccountDisplayName"] = value;
            }
        }

        public string DomainName
        {
            get
            {
                if (this._core.SettingsManager["cfgSipAccountDomain"] != null)
                    return this._core.SettingsManager["cfgSipAccountDomain"].ToString();

                return "";
            }
            set
            {
                this._core.SettingsManager["cfgSipAccountDomain"] = value;
            }
        }

        public int RegState
        {
            get
            {
                int value;
                if (this._core.SettingsManager["cfgSipAccountState"] != null && int.TryParse(this._core.SettingsManager["cfgSipAccountState"].ToString(), out value))
                {
                    return value;
                }
                return 0;
            }
            set
            {
                this._core.SettingsManager["cfgSipAccountState"] = value;
            }
        }

        public string ProxyAddress
        {
            get
            {
                if (this._core.SettingsManager["cfgSipAccountProxyAddress"] != null)
                    return this._core.SettingsManager["cfgSipAccountProxyAddress"].ToString();
                else
                    return "";
            }
            set
            {
                this._core.SettingsManager["cfgSipAccountProxyAddress"] = value;
            }
        }

        public ETransportMode TransportMode
        {
            get
            {
                int value;
                if (this._core.SettingsManager["cfgSipAccountTransport"] != null && int.TryParse(this._core.SettingsManager["cfgSipAccountTransport"].ToString(), out value))
                {
                    return (ETransportMode)value;
                }
                return (ETransportMode.TM_UDP); // default
            }
            set
            {
                this._core.SettingsManager["cfgSipAccountTransport"] = (int)value;
            }
        }
        #endregion

    }

    /// <summary>
    /// 
    /// </summary>
    public class SipekConfigurator : IConfiguratorInterface
    {
        private ICore _core;
        private int _defaultAccountIndex = -1;

        public SipekConfigurator(ICore core)
        {
            this._core = core;
        }

        public int DefaultAccountIndex
        {
            get { return _defaultAccountIndex; }
        }

        public bool IsNull { get { return false; } }

        public bool PauseFlag { get; set; }

        public bool CFUFlag
        {
            get 
            {
                bool ret;
                if (this._core.SettingsManager["cfgCFUFlag"] != null)
                    if (Boolean.TryParse(this._core.SettingsManager["cfgCFUFlag"].ToString(), out ret))
                        return ret;

                return false;
            }
            set { this._core.SettingsManager["cfgCFUFlag"] = value; }
        }
        
        public string CFUNumber
        {
            get 
            {
                if (this._core.SettingsManager["cfgCFUNumber"] != null)
                    return this._core.SettingsManager["cfgCFUNumber"].ToString();
                return "";
            }
            set { this._core.SettingsManager["cfgCFUNumber"] = value; }
        }
        
        public bool CFNRFlag
        {
            get
            {
                bool ret;
                if (this._core.SettingsManager["cfgCFNRFlag"] != null)
                    if (Boolean.TryParse(this._core.SettingsManager["cfgCFNRFlag"].ToString(), out ret))
                        return ret;

                return false;
            }
            set { this._core.SettingsManager["cfgCFNRFlag"] = value; }
        }
        
        public string CFNRNumber
        {
            get 
            {
                if (this._core.SettingsManager["cfgCFNRNumber"] != null)
                    return this._core.SettingsManager["cfgCFNRNumber"].ToString();
                return "";
            }
            set { this._core.SettingsManager["cfgCFNRNumber"] = value; }
        }
        
        public bool DNDFlag
        {
            get
            {
                bool ret;
                if (this._core.SettingsManager["cfgDNDFlag"] != null)
                    if (Boolean.TryParse(this._core.SettingsManager["cfgDNDFlag"].ToString(), out ret))
                        return ret;

                return false;
            }
            set { this._core.SettingsManager["cfgDNDFlag"] = value; }
        }
        
        public bool AAFlag
        {
            get
            {
                bool ret;
                if (this._core.SettingsManager["cfgAAFlag"] != null)
                    if (Boolean.TryParse(this._core.SettingsManager["cfgAAFlag"].ToString(), out ret))
                        return ret;

                return false;
            }
            set { this._core.SettingsManager["cfgAAFlag"] = value; }
        }

        public bool CFBFlag
        {
            get
            {
                bool ret;
                if (this._core.SettingsManager["cfgCFBFlag"] != null)
                    if (Boolean.TryParse(this._core.SettingsManager["cfgCFBFlag"].ToString(), out ret))
                        return ret;

                return false;
            }
            set { this._core.SettingsManager["cfgCFBFlag"] = value; }
        }

        public string CFBNumber
        {
            get 
            {
                if (this._core.SettingsManager["cfgCFBNumber"] != null)
                    return this._core.SettingsManager["cfgCFBNumber"].ToString();
                return "";
            }
            set { this._core.SettingsManager["cfgCFBNumber"] = value; }
        }

        public int SIPPort
        {
            get
            {
                Int32 ret;
                if (this._core.SettingsManager["cfgSipPort"] != null)
                    if (Int32.TryParse(this._core.SettingsManager["cfgSipPort"].ToString(), out ret))
                        return ret;

                return 5060;
            }
            set { this._core.SettingsManager["cfgSipPort"] = value; }
        }

        public int RTPPort
        {
            get
            {
                Int32 ret;
                if (this._core.SettingsManager["cfgRtpPort"] != null)
                    if (Int32.TryParse(this._core.SettingsManager["cfgRtpPort"].ToString(), out ret))
                        return ret;

                return 4000;
            }
            set { this._core.SettingsManager["cfgRtpPort"] = value; }
        }

        public bool PublishEnabled
        {
            get
            {
                bool ret = false;
                if (this._core.SettingsManager["cfgSipPublishEnabled"] != null)
                    Boolean.TryParse(this._core.SettingsManager["cfgSipPublishEnabled"].ToString(), out ret);

                SipConfigStruct.Instance.publishEnabled = ret;
                return ret;
            }
            set
            {
                SipConfigStruct.Instance.publishEnabled = value;
                this._core.SettingsManager["cfgSipPublishEnabled"] = value;
            }
        }

        public string StunServerAddress
        {
            get
            {
                if (this._core.SettingsManager["cfgStunServerAddress"] != null)
                {
                    SipConfigStruct.Instance.stunServer = this._core.SettingsManager["cfgStunServerAddress"].ToString();
                    return this._core.SettingsManager["cfgStunServerAddress"].ToString();
                }
                return "";
            }
            set
            {
                this._core.SettingsManager["cfgStunServerAddress"] = value;
                SipConfigStruct.Instance.stunServer = value;
            }
        }

        public EDtmfMode DtmfMode
        {
            get
            {
                Int32 ret;
                if (this._core.SettingsManager["cfgDtmfMode"] != null)
                    if (Int32.TryParse(this._core.SettingsManager["cfgDtmfMode"].ToString(), out ret))
                        return (EDtmfMode)ret;

                return EDtmfMode.DM_Transparent;
            }
            set
            {
                this._core.SettingsManager["cfgDtmfMode"] = (int)value;
            }
        }

        public int Expires
        {
            get
            {
                Int32 ret = 3600;
                if (this._core.SettingsManager["cfgRegistrationTimeout"] != null)
                    Int32.TryParse(this._core.SettingsManager["cfgRegistrationTimeout"].ToString(), out ret);

                SipConfigStruct.Instance.expires = ret;
                return ret;
            }
            set
            {
                this._core.SettingsManager["cfgRegistrationTimeout"] = value;
                SipConfigStruct.Instance.expires = value;
            }
        }

        public int ECTail
        {
            get
            {
                Int32 ret = 3600;
                if (this._core.SettingsManager["cfgECTail"] != null)
                    Int32.TryParse(this._core.SettingsManager["cfgECTail"].ToString(), out ret);

                SipConfigStruct.Instance.ECTail = ret;
                return ret;
            }
            set
            {
                this._core.SettingsManager["cfgECTail"] = value;
                SipConfigStruct.Instance.ECTail = value;
            }
        }

        public bool VADEnabled
        {
            get
            {
                bool ret = false;
                if (this._core.SettingsManager["cfgVAD"] != null)
                    Boolean.TryParse(this._core.SettingsManager["cfgVAD"].ToString(), out ret);

                SipConfigStruct.Instance.VADEnabled = ret;
                return ret;
            }
            set
            {
                this._core.SettingsManager["cfgVAD"] = value;
                SipConfigStruct.Instance.VADEnabled = value;
            }
        }

        public string NameServer
        {
            get
            {
                if (this._core.SettingsManager["cfgNameServer"] != null)
                {
                    SipConfigStruct.Instance.nameServer = this._core.SettingsManager["cfgNameServer"].ToString();
                    return this._core.SettingsManager["cfgNameServer"].ToString();
                }
                return "";
            }
            set
            {
                this._core.SettingsManager["cfgNameServer"] = value;
                SipConfigStruct.Instance.nameServer = value;
            }
        }

        private IAccount _account;
        public IAccount Account
        {
            get 
            {
                if (_account == null)
                    _account = new SipekAccount(this._core);

                return _account; 
            }
        }

        public void Save()
        {
            // save properties
            this._core.SettingsManager.Save();
        }

        public List<string> CodecList
        {
            get
            {
                List<string> codecList = new List<string>();

                if (this._core.SettingsManager["cfgCodecList"] != null)
                    foreach (string item in (StringCollection)this._core.SettingsManager["cfgCodecList"])
                        codecList.Add(item);

                return codecList;
            }
            set
            {
                this._core.SettingsManager["cfgCodecList"] = new StringCollection();
                List<string> cl = value;
                foreach (string item in cl)
                    ((StringCollection)this._core.SettingsManager["cfgCodecList"]).Add(item);
            }
        }

        public bool AudioPlayOnIncoming
        {
            get { return _core.SettingsManager.GetValueOrSetDefault<bool>("AudioPlayOnIncoming", true); }
            set { _core.SettingsManager.Set("AudioPlayOnIncoming", value); }
        }

        public bool AudioPlayOnIncomingAndActive
        {
            get { return _core.SettingsManager.GetValueOrSetDefault<bool>("AudioPlayOnIncomingAndActive", false); }
            set { _core.SettingsManager.Set("AudioPlayOnIncomingAndActive", value); }
        }

        public bool AudioPlayOutgoing
        {
            get { return _core.SettingsManager.GetValueOrSetDefault<bool>("AudioPlayOutgoing", true); }
            set { _core.SettingsManager.Set("AudioPlayOutgoing", value); }
        }
    }

    #endregion Concrete Implementations
}
