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
        private readonly System.Timers.Timer _deferredSetAudioDevicesTimer;
        
        internal SipekResources SipekResources { get; private set; }

        public SIP(Core core, ISynchronizeInvoke syncInvoke)
        {
            Logger.LogNotice("Initializing SIP core");
            _core = core;

            _deferredSetAudioDevicesTimer = new System.Timers.Timer() { Interval = 200, SynchronizingObject = syncInvoke };
            _deferredSetAudioDevicesTimer.Elapsed += new System.Timers.ElapsedEventHandler(_deferredSetAudioDevicesTimer_Elapsed);
            _deferredSetAudioDevicesTimer.Stop();

            // Initialize PjSIP
            SipekResources = new SipekResources(core);

            if (!CheckUdpPort(SipekResources.Configurator.SIPPort))
            {
                if (CheckUdpPort(5060)) SipekResources.Configurator.SIPPort = 5060;
                else if (CheckUdpPort(5061)) SipekResources.Configurator.SIPPort = 5061;
                else throw new InvalidOperationException("SIP port is in use");
            }
            
            if (SipekResources.StackProxy.initialize() != 0)
                throw new InvalidOperationException("Can't initialize PjSIP proxy stack!");

            if (SipekResources.CallManager.Initialize(SipekResources.StackProxy) != 0)
                throw new InvalidOperationException("Can't initialize PjSIP call manager!");

            Codecs = LoadCodecs();

            // Initialize account
            Account = new SipAccount(this);
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
            Account.Renew();
        }

        /// <summary>
        /// Set audio devices to pjsip
        /// </summary>
        private void SetAudioDevices()
        {
            var playbackDevice = Core.Audio.PlaybackDevice;
            var recordingDevice = Core.Audio.RecordingDevice;
            
            Logger.LogNotice(string.Format("Setting audio devices {0}/{1}", playbackDevice?.Name ?? "none", recordingDevice?.Name ?? "none"));

            try
            {
                if (playbackDevice == null || recordingDevice == null)
                {
                    Logger.LogWarn("One of audio devices is invalid or not found - reseting audio devices settings!");
                    SipekResources.StackProxy.setSoundDevice(string.Empty, string.Empty);

                    playbackDevice = null;
                    recordingDevice = null;
                }
                else
                {
                    var playbackDeviceName = GetPjSipDeviceName(playbackDevice);
                    var recordingDeviceName = GetPjSipDeviceName(recordingDevice);

                    SipekResources.StackProxy.setSoundDevice(playbackDeviceName, recordingDeviceName);
                
                    if (Core.Audio.PlaybackDevice != null) Core.SettingsManager["PlaybackDevice"] = playbackDeviceName;
                    if (Core.Audio.RecordingDevice != null) Core.SettingsManager["RecordingDevice"] = recordingDeviceName;
                }
            }
            catch (Exception e)
            {
                Logger.LogWarn(e);
            }

            PlaybackDeviceChanged?.Invoke(playbackDevice);
            RecordingDeviceChanged?.Invoke(recordingDevice);
        }

        /// <summary>
        /// 31 is the max length of sound device name in PjSIP
        /// </summary>
        /// <param name="device">Device</param>
        /// <returns></returns>
        string GetPjSipDeviceName(IAudioDevice device)
        {
            if (device.Name.Length > 31)
            {
                return device.Name.Substring(0, 31);
            }

            return device.Name;
        }

        void codec_EnabledChanged(ISipCodec codec)
        {
            try
            {
                Logger.LogNotice(string.Format("Setting codec priority on {0} to {1}", codec.Name, codec.Priority));
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

            int noOfCodecs = SipekResources.StackProxy.getNoOfCodecs();

            Logger.LogNotice("Loading codecs");
            for (int i = 0; i < noOfCodecs; i++)
            {
                ISipCodec codec = new SipCodec(this, SipekResources.StackProxy.getCodec(i));

                codec.EnabledChanged += new Action<ISipCodec>(codec_EnabledChanged);

                codecList.Add(codec);

                Logger.LogNotice(string.Format("Codec {0} loaded", codec.Name));
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

        public ISipAccount Account { get; }

        public ISipMessenger Messenger { get; }

        public List<ISipCodec> Codecs { get; }

        public SipTransportType TransportType
        {
            get { return (SipekResources.Configurator.Account.TransportMode == ETransportMode.TM_TCP ? SipTransportType.TCP : SipTransportType.UDP); }
            set { SipekResources.Configurator.Account.TransportMode = (value == SipTransportType.TCP ? ETransportMode.TM_TCP : ETransportMode.TM_UDP); }
        }

        public SipDTMFMode DTMFMode
        {
            get { return ConvertSipekDTMFMode(SipekResources.Configurator.DtmfMode); }
            set { SipekResources.Configurator.DtmfMode = ConvertSipDTMFMode(value); }
        }

        public int EchoCancelationTimeout
        {
            get { return SipekResources.Configurator.ECTail; }
            set { SipekResources.Configurator.ECTail = value; }
        }

        public bool VoiceActiveDetection
        {
            get { return SipekResources.Configurator.VADEnabled; }
            set { SipekResources.Configurator.VADEnabled = value; }
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
