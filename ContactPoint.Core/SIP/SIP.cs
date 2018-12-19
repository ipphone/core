using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using ContactPoint.Common;
using ContactPoint.Common.Audio;
using ContactPoint.Common.SIP;
using ContactPoint.Common.SIP.Account;
using ContactPoint.Core.SIP.Account;
using Sipek.Common;
using System.ComponentModel;
using Sipek.Sip;

namespace ContactPoint.Core.SIP
{
    internal class SIP : ISip
    {
        private readonly object _lockObj = new object();
        private readonly Core _core;
        private readonly List<ISipCodec> _codecsList;
        private readonly ISipAccount _account;
        private readonly SipekResources _sipekResources;
        private readonly System.Timers.Timer _deferredSetAudioDevicesTimer;

        public SIP(Core core, ISynchronizeInvoke syncInvoke)
        {
            Logger.LogNotice("Initializing SIP core");
            _core = core;

            _deferredSetAudioDevicesTimer = new System.Timers.Timer() { Interval = 200, SynchronizingObject = syncInvoke };
            _deferredSetAudioDevicesTimer.Elapsed += new System.Timers.ElapsedEventHandler(_deferredSetAudioDevicesTimer_Elapsed);
            _deferredSetAudioDevicesTimer.Stop();

            // Initialize PjSIP
            _sipekResources = new SipekResources(core);

            if (!CheckUdpPort(_sipekResources.Configurator.SIPPort))
            {
                if (CheckUdpPort(5060)) _sipekResources.Configurator.SIPPort = 5060;
                else if (CheckUdpPort(5061)) _sipekResources.Configurator.SIPPort = 5061;
                else throw new InvalidOperationException("SIP port is in use");
            }
            
            if (_sipekResources.StackProxy.initialize() != 0)
                throw new InvalidOperationException("Can't initialize PjSIP proxy stack!");

            if (_sipekResources.CallManager.Initialize(_sipekResources.StackProxy) != 0)
                throw new InvalidOperationException("Can't initialize PjSIP call manager!");

            _codecsList = LoadCodecs();

            // Initialize account
            _account = new SipAccount(this);
            Messenger = new SipMessenger(this);

            NetworkChange.NetworkAddressChanged += NetworkChange_NetworkAddressChanged;
        }

        public void InitializeAudio()
        {
            SetAudioDevices();

            Core.Audio.PlaybackDeviceChanged += new Action<IAudioDevice>(Audio_PlaybackDeviceChanged);
            Core.Audio.RecordingDeviceChanged += new Action<IAudioDevice>(Audio_RecordingDeviceChanged);
        }

        bool CheckUdpPort(int port)
        {
            foreach (var listener in IPGlobalProperties.GetIPGlobalProperties().GetActiveUdpListeners())
            {
                if (listener.Port == port)
                    return false;
            }

            return true;
        }

        void Audio_PlaybackDeviceChanged(IAudioDevice obj)
        {
            SetAudioDevicesDeferred();
        }

        void Audio_RecordingDeviceChanged(IAudioDevice obj)
        {
            SetAudioDevicesDeferred();
        }

        internal SipekResources SipekResources
        {
            get { return _sipekResources; }
        }

        void _deferredSetAudioDevicesTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            lock (_lockObj)
            {
                _deferredSetAudioDevicesTimer.Stop();

                SetAudioDevices();
            }
        }

        /// <summary>
        /// Audio device changing is quite hard operation for pjmedia. So double call of this method while changing both of devices
        /// is very bad. Let's call it from small Timer
        /// </summary>
        private void SetAudioDevicesDeferred()
        {
            lock (_lockObj)
            {
                _deferredSetAudioDevicesTimer.Start();
            }
        }

        void NetworkChange_NetworkAddressChanged(object sender, EventArgs e)
        {
            _account.Renew();
        }

        /// <summary>
        /// Set audio devices to pjsip
        /// </summary>
        private void SetAudioDevices()
        {
            if (Core.Audio.PlaybackDevice != null) Core.SettingsManager["PlaybackDevice"] = Core.Audio.PlaybackDevice.Name;
            if (Core.Audio.RecordingDevice != null) Core.SettingsManager["RecordingDevice"] = Core.Audio.RecordingDevice.Name;

            try
            {
                if (Core.Audio.PlaybackDevice == null || Core.Audio.RecordingDevice == null)
                {
                    _sipekResources.StackProxy.setSoundDevice(
                        String.Empty,
                        String.Empty
                        );

                    Logger.LogWarn(String.Format("Setting null audio devices"));
                }
                else
                {
                    _sipekResources.StackProxy.setSoundDevice(
                        Core.Audio.PlaybackDevice.Name.Substring(0, Core.Audio.PlaybackDevice.Name.Length > 31 ? 31 : Core.Audio.PlaybackDevice.Name.Length), // 31 is the max length of sound device name in PjSIP
                        Core.Audio.RecordingDevice.Name.Substring(0, Core.Audio.RecordingDevice.Name.Length > 31 ? 31 : Core.Audio.RecordingDevice.Name.Length)
                        );

                    Logger.LogNotice(String.Format("Setting audio devices {0}/{1}",
                        Core.Audio.PlaybackDevice != null ? Core.Audio.PlaybackDevice.Name : "none",
                        Core.Audio.RecordingDevice != null ? Core.Audio.RecordingDevice.Name : "none"));
                }
            }
            catch (Exception e)
            {
                Logger.LogWarn(e);
            }
        }

        void codec_EnabledChanged(ISipCodec codec)
        {
            try
            {
                Logger.LogNotice(String.Format("Setting codec priority on {0} to {1}", codec.Name, codec.Priority));
                SipekResources.StackProxy.setCodecPriority(codec.Name, codec.Priority);
            }
            catch (Exception e)
            {
                Logger.LogWarn(e);
            }
        }

        #region Loading internal structures

        private List<ISipCodec> LoadCodecs()
        {
            List<ISipCodec> codecList = new List<ISipCodec>();

            int noOfCodecs = _sipekResources.StackProxy.getNoOfCodecs();

            Logger.LogNotice("Loading codecs");
            for (int i = 0; i < noOfCodecs; i++)
            {
                ISipCodec codec = new SipCodec(this, _sipekResources.StackProxy.getCodec(i));

                codec.EnabledChanged += new Action<ISipCodec>(codec_EnabledChanged);

                codecList.Add(codec);

                Logger.LogNotice(String.Format("Codec {0} loaded", codec.Name));
            }

            Logger.LogNotice("Setting codecs priorities");
            foreach (ISipCodec codec in codecList)
                codec.Enabled = Core.SettingsManager.GetValueOrSetDefault<bool>(codec.Name, codec.Name.Contains("speex")); // To auto-enable speex codecs

            return codecList;
        }

        #endregion

        #region ISip Members

        public event Action<IAudioDevice> RecordingDeviceChanged;

        public event Action<IAudioDevice> PlaybackDeviceChanged;

        public ISipAccount Account
        {
            get { return _account; }
        }

        public ISipMessenger Messenger { get; }

        public List<ISipCodec> Codecs
        {
            get { return _codecsList; }
        }

        public SipTransportType TransportType
        {
            get { return (_sipekResources.Configurator.Account.TransportMode == ETransportMode.TM_TCP ? SipTransportType.TCP : SipTransportType.UDP); }
            set { _sipekResources.Configurator.Account.TransportMode = (value == SipTransportType.TCP ? ETransportMode.TM_TCP : ETransportMode.TM_UDP); }
        }

        public SipDTMFMode DTMFMode
        {
            get { return ConvertSipekDTMFMode(_sipekResources.Configurator.DtmfMode); }
            set { _sipekResources.Configurator.DtmfMode = ConvertSipDTMFMode(value); }
        }

        public int EchoCancelationTimeout
        {
            get { return _sipekResources.Configurator.ECTail; }
            set { _sipekResources.Configurator.ECTail = value; }
        }

        public bool VoiceActiveDetection
        {
            get { return _sipekResources.Configurator.VADEnabled; }
            set { _sipekResources.Configurator.VADEnabled = value; }
        }

        public ICore Core
        {
            get { return _core; }
        }

        #endregion

        #region Helper

        private SipDTMFMode ConvertSipekDTMFMode(EDtmfMode dtmfMode)
        {
            switch (dtmfMode)
            {
                case EDtmfMode.DM_Outband: return SipDTMFMode.OutOfBand;
                case EDtmfMode.DM_Transparent: return SipDTMFMode.Transparent;
                default: return SipDTMFMode.RFC2833;
            }
        }

        private EDtmfMode ConvertSipDTMFMode(SipDTMFMode dtmfMode)
        {
            switch (dtmfMode)
            {
                case SipDTMFMode.OutOfBand: return EDtmfMode.DM_Outband;
                case SipDTMFMode.Transparent: return EDtmfMode.DM_Transparent;
                default: return EDtmfMode.DM_Inband;
            }
        }

        #endregion
    }
}
