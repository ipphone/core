using System;
using ContactPoint.Common;
using ContactPoint.Common.SIP;
using ContactPoint.Common.SIP.Account;

namespace ContactPoint.Core.SIP.Account
{
    internal class SipAccount : ISipAccount
    {
        private readonly SIP _sip;

        private int _registrationState = -1;
        private PresenceStatus _presenceStatus = new PresenceStatus();
        private DateTime _registrationStateLastUpdateTime = DateTime.UtcNow;
        private DateTime _presenceStatusLastUpdateTime = DateTime.UtcNow;

        public SipAccount(ISip sip)
        {
            _sip = (SIP)sip;
            _valueChangedCallback = RaiseEventCallback<ISipAccount>;

            _sip.SipekResources.Registrar.AccountStateChanged += OnAccountStateChanged;
        }

        void OnAccountStateChanged(int accState)
        {
            Logger.LogNotice($"Registration state changed from {_registrationState} to {accState}");
            _registrationState = accState;

            RaiseRegistrationStateChanged();

            if (RegisterState == SipAccountState.Online &&
                (PresenceStatus.Code == PresenceStatusCode.Unknown || PresenceStatus.Code == PresenceStatusCode.Offline))
            {
                PresenceStatus = new PresenceStatus(PresenceStatusCode.Available);
            }

            if (RegisterState == SipAccountState.Offline && PresenceStatus.Code != PresenceStatusCode.Offline)
            {
                PresenceStatus = new PresenceStatus(PresenceStatusCode.Offline);
            }
        }

        #region ISipAccount Members

        public event Action<ISipAccount> RegisterStateChanged;

        public event Action<ISipAccount> PresenceStatusChanged;

        public ISip Sip => _sip;

        public string FullName
        {
            get { return _sip.SipekResources.Configurator.Account.DisplayName; }
            set { _sip.SipekResources.Configurator.Account.DisplayName = value; }
        }

        public string UserName
        {
            get { return _sip.SipekResources.Configurator.Account.UserName; }
            set { _sip.SipekResources.Configurator.Account.UserName = value; }
        }

        public string Password
        {
            get { return _sip.SipekResources.Configurator.Account.Password; }
            set { _sip.SipekResources.Configurator.Account.Password = value; }
        }

        public string Server
        {
            get { return _sip.SipekResources.Configurator.Account.HostName; }
            set { _sip.SipekResources.Configurator.Account.HostName = value; }
        }

        public string Realm
        {
            get { return _sip.SipekResources.Configurator.Account.DomainName; }
            set { _sip.SipekResources.Configurator.Account.DomainName = value; }
        }

        public int Port
        {
            get { return _sip.SipekResources.Configurator.SIPPort; }
            set { _sip.SipekResources.Configurator.SIPPort = value; }
        }

        public int RtpPort
        {
            get { return _sip.SipekResources.Configurator.RTPPort; }
            set { _sip.SipekResources.Configurator.RTPPort = value; }
        }

        public int RegisterTimeout
        {
            get { return _sip.SipekResources.Configurator.Expires; }
            set { _sip.SipekResources.Configurator.Expires = value; }
        }

        public SipAccountState RegisterState
        {
            get 
            {
                switch (_registrationState)
                {
                    case 0: return SipAccountState.Connecting;
                    case 200: return SipAccountState.Online;
                    default: return SipAccountState.Offline;
                }
            }
        }

        public TimeSpan RegisterStateDuration => DateTime.UtcNow - _registrationStateLastUpdateTime;

        public int RegisterStateCode => _registrationState;

        public PresenceStatus PresenceStatus
        {
            get { return _presenceStatus; }
            set
            {
                _presenceStatus = value;

                if (_presenceStatus != null)
                {
                    _sip.SipekResources.Configurator.PauseFlag = _presenceStatus.Code == PresenceStatusCode.NotAvailable;
                }

                OnPresenceStatusChanged();
            }
        }

        public TimeSpan PresenceStatusDuration
            => DateTime.UtcNow - _presenceStatusLastUpdateTime;

        public void Register()
        {
            Logger.LogNotice($"Registering on SIP server {Server} as {UserName}");
            _sip.SipekResources.Registrar.registerAccounts();
        }

        public void UnRegister()
        {
            Logger.LogNotice("Unregistering from SIP server");
            _sip.SipekResources.Registrar.unregisterAccounts();
        }

        public void Renew()
        {
            Logger.LogNotice("Renewing account state");
            _sip.SipekResources.Registrar.renewAccount(_sip.SipekResources.Configurator.Account.Index);
        }

        #endregion

        #region Event raising

        private readonly AsyncCallback _valueChangedCallback;
        void OnPresenceStatusChanged()
        {
            _presenceStatusLastUpdateTime = DateTime.UtcNow;
            SafeRaiseEvent(PresenceStatusChanged, this, _valueChangedCallback);
        }

        void RaiseRegistrationStateChanged()
        {
            _registrationStateLastUpdateTime = DateTime.UtcNow;
            SafeRaiseEvent(RegisterStateChanged, this, _valueChangedCallback);
        }

        static void SafeRaiseEvent<T>(Action<T> del, T obj, AsyncCallback asyncCallback)
        {
            if (del == null) return;

            var invocationList = del.GetInvocationList();

            foreach (var current in invocationList)
            {
                ((Action<T>)current).BeginInvoke(obj, asyncCallback, current);
            }
        }

        static void RaiseEventCallback<T>(IAsyncResult result)
        {
            try
            {
                ((Action<T>)result.AsyncState).EndInvoke(result);
            }
            catch (Exception ex)
            {
                Logger.LogWarn(ex);
            }
        }

        #endregion
    }
}
