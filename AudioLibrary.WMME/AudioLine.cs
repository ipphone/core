using System;
using System.Collections.Generic;
using AudioLibrary.Interfaces;
using System.Runtime.InteropServices;
using AudioLibrary.WMME.Native;
using System.Diagnostics;

namespace AudioLibrary.WMME
{
    public enum MixerType
    {
        Recording, 
        Playback
    }

    public class AudioLine
    {
        public const int BAL_CENTER = 50;
        public const int BAL_LEFT = 0;
        public const int BAL_RIGHT = 100;

        private AudioDevice _audioDevice;
        private List<AudioLineControl> _audioLineControls = new List<AudioLineControl>();
        private double _volumeCoef = 1;
        private int _balance = BAL_CENTER;
        private bool _mute = false;
        private int _lastMutedVolume = 0;

        /// <summary>
        /// Internal volume from 0 to 100
        /// </summary>
        private int _volume = 0;

        public event Action<AudioLine> VolumeChanged;
        public event Action<AudioLine> BalanceChanged;
        public event Action<AudioLine> MuteChanged;

        public uint Id { get; private set; }
        public string Name { get; private set; }
        public MixerType Direction { get; private set; }
        public uint Channels { get; private set; }
        public uint CControls { get; private set; }
        public uint Connections { get; private set; }
        public uint Flag { get; private set; }
        public uint Source { get; private set; }
        public uint Destination { get; private set; }
        public MIXERLINE_COMPONENTTYPE ComponentType { get; private set; }
        public Channel Channel { get; private set; }

        public IAudioDevice AudioDevice
        {
            get { return _audioDevice; }
        }

        public bool Active
        {
            get { return ((Flag & (uint)MIXERLINE_LINEFLAG.ACTIVE) > 0); }
        }

        public bool Connected
        {
            get { return ((Flag & (uint)MIXERLINE_LINEFLAG.DISCONNECTED) == 0); }
        }

        public bool HasVolume
        {
            get
            {
                var lineControl = GetControlByType(MIXERCONTROL_CONTROLTYPE.VOLUME);
                if (lineControl == null)
                    return false;
                return true;
            }
        }

        public int Volume 
        {
            get { return this._volume; }
            set
            {
                RefreshVolume(value);

                RaiseVolumeChangedEvent(this);
            }
        }

        public bool HasBalance
        {
            get { return Channels > 1; }
        }

        public int Balance
        {
            get { return this._balance; }
            set
            {
                if (Channels > 1)
                {
                    if (value > BAL_RIGHT) this._balance = BAL_RIGHT;
                    else if (value < BAL_LEFT) this._balance = BAL_LEFT;
                    else this._balance = value;

                    RefreshVolume();

                    RaiseBalanceChangedEvent(this);
                }
            }
        }

        public bool Mute
        {
            get 
            {
                var lineControl = GetControlByType(MIXERCONTROL_CONTROLTYPE.MUTE);

                if (lineControl == null)
                    return this._mute;
                else
                    return (bool)lineControl.Value;
            }
            set
            {
                var lineControl = GetControlByType(MIXERCONTROL_CONTROLTYPE.MUTE);

                if (lineControl == null)
                {
                    if (_mute != value)
                    {
                        _mute = value;

                        if (value)
                        {
                            _lastMutedVolume = Volume;
                            Volume = 0;
                        }
                        else
                        {
                            if (_lastMutedVolume > 0)
                                Volume = _lastMutedVolume;
                        }
                    }

                    RaiseMuteChangedEvent(this);
                }
                else
                    lineControl.Value = value;
            }
        }

        internal int VolumeInternal
        {
            get
            {
                var lineControl = GetControlByType(MIXERCONTROL_CONTROLTYPE.VOLUME);

                if (lineControl == null)
                    return -1;
                else
                    return (int)lineControl.Value;
            }
            set
            {
                var lineControl = GetControlByType(MIXERCONTROL_CONTROLTYPE.VOLUME);

                if (lineControl != null)
                    lineControl.Value = value;
            }
        }

        internal int VolumeMax
        {
            get
            {
                var lineControl = GetControlByType(MIXERCONTROL_CONTROLTYPE.VOLUME);
                if (lineControl == null)
                    return -1;
                else
                    return (int)lineControl.Maximum;
            }
        }

        internal int VolumeMin
        {
            get
            {
                var lineControl = GetControlByType(MIXERCONTROL_CONTROLTYPE.VOLUME);
                if (lineControl == null)
                    return -1;
                else
                    return (int)lineControl.Minimum;
            }
        }

        internal AudioDevice AudioDeviceInternal
        {
            get { return _audioDevice; }
        }

        internal List<AudioLineControl> Controls
        {
            get { return _audioLineControls; }
        }

        internal AudioLine(AudioDevice audioDevice, MIXERLINE mxl)
        {
            _audioDevice = audioDevice;

            if (AudioDevice.RecordingSupport)
                Direction = MixerType.Recording;
            else if (AudioDevice.PlaybackSupport)
                Direction = MixerType.Playback;

            Channels = mxl.cChannels;
            CControls = mxl.cControls;
            Connections = mxl.cConnections;
            Flag = mxl.fdwLine;
            Destination = mxl.dwDestination;
            Name = mxl.szName;
            Id = mxl.dwLineID;
            ComponentType = mxl.dwComponentType;
            Source = mxl.dwSource;

            ReloadAudioLineControls();
            ReloadValues();
        }

        /// <summary>
        /// Refresh volume values
        /// </summary>
        private void RefreshVolume()
        {
            RefreshVolume(Volume);
        }

        /// <summary>
        /// Set volume values with specified value
        /// </summary>
        /// <param name="newVolume">New internal volume value (0..100)</param>
        private void RefreshVolume(int newVolume)
        {
            if (newVolume > 100) newVolume = 100;
            if (newVolume < 0) newVolume = 0;

            this._volume = newVolume;

            if (Channels > 1)
            {
                if (Balance == BAL_CENTER)
                {
                    SetVolumeOnChannel(Channel.Left, this._volume);
                    SetVolumeOnChannel(Channel.Right, this._volume);
                }
                else
                {
                    // Left speaker
                    if (Balance < BAL_CENTER)
                    {
                        SetVolumeOnChannel(Channel.Left, this._volume);
                        SetVolumeOnChannel(Channel.Right, this._volume * (this.Balance / BAL_CENTER));
                    }
                    // Right speaker
                    else
                    {
                        SetVolumeOnChannel(Channel.Left, this._volume * (2 - this.Balance / BAL_CENTER));
                        SetVolumeOnChannel(Channel.Right, this._volume);
                    }
                }
            }
            else
                SetVolumeOnChannel(Channel.Uniform, this._volume);
        }

        /// <summary>
        /// Sets volume on specified channel
        /// </summary>
        /// <param name="channel">Channel where volume set</param>
        /// <param name="newVolume">Internal volume value (0..100)</param>
        private void SetVolumeOnChannel(Channel channel, int newVolume)
        {
            try
            {
                Channel = channel;

                VolumeInternal = (int)(newVolume / this._volumeCoef);
            }
            catch (Exception e)
            {
                Trace.TraceError("Unable to set volume on channel {0} #{1}: {2}", Name, Id, e.Message);
            }
        }

        internal void ReloadValues()
        {
            // Setup volume coeficient
            if (this.VolumeMax > 0) _volumeCoef = 100F / VolumeMax;
            else this._volumeCoef = 0;

            // Load volume and balance values
            try
            {
                if (this.Channels > 1)
                {
                    int volumeLeft = 0;
                    int volumeRight = 0;

                    Channel = Channel.Left;
                    volumeLeft = (int)(VolumeInternal * _volumeCoef);

                    Channel = Channel.Right;
                    volumeRight = (int)(VolumeInternal * _volumeCoef);

                    if (volumeLeft == volumeRight)
                        this._volume = volumeLeft;
                    else
                    {
                        if (volumeLeft > volumeRight)
                        {
                            this._volume = volumeLeft;
                            this._balance = BAL_CENTER * volumeRight / volumeLeft;
                        }
                        else
                        {
                            this._volume = volumeRight;
                            this._balance = BAL_RIGHT - BAL_CENTER * volumeLeft / volumeRight;
                        }
                    }
                }
                else
                    this._volume = (int)(VolumeInternal * this._volumeCoef);
            }
            catch (Exception e)
            {
                Trace.TraceError("Unable to reload values on audio line {0} #{1}: {2}", Name, Id, e.Message);
            }

            RaiseVolumeChangedEvent(this);
            RaiseBalanceChangedEvent(this);

            // Set mute here because I am set to property, which may raise event about changing
            _mute = !(_volume > 0);
        }

        private unsafe void ReloadAudioLineControls()
        {
            MMErrors errorCode = 0;
            MIXERLINECONTROLS mlc = new MIXERLINECONTROLS();
            IntPtr pmc = IntPtr.Zero;

            try
            {
                _audioLineControls.Clear();

                if (CControls == 0)
                    return;

                pmc = Marshal.AllocHGlobal((int)(Marshal.SizeOf(typeof(MIXERCONTROL)) * CControls));
                mlc.cbStruct = (uint)sizeof(MIXERLINECONTROLS);
                mlc.dwLineID = Id;
                mlc.cControls = CControls;
                mlc.pamxctrl = pmc;
                mlc.cbmxctrl = (uint)(Marshal.SizeOf(typeof(MIXERCONTROL)));

                errorCode = (MMErrors)MixerNative.mixerGetLineControls(AudioDeviceInternal.Handle, ref mlc, MIXER_GETLINECONTROLSFLAG.ALL);
                if (errorCode != MMErrors.MMSYSERR_NOERROR)
                    throw new MixerException(errorCode, Audio.GetErrorDescription(FuncName.fnMixerGetLineControls, errorCode));

                for (int i = 0; i < mlc.cControls; i++)
                {
                    var mc = (MIXERCONTROL)Marshal.PtrToStructure((IntPtr)(((byte*)pmc) + (Marshal.SizeOf(typeof(MIXERCONTROL)) * i)), typeof(MIXERCONTROL));

                    AudioLineControl lineControl;
                    switch ((MIXERCONTROL_CONTROLTYPE)mc.dwControlType)
                    {
                        case MIXERCONTROL_CONTROLTYPE.MUTE: lineControl = new MuteAudioLineControl(this, mc, new Action<AudioLine>(RaiseMuteChangedEvent)); break;
                        default: lineControl = new VolumeAudioLineControl(this, mc, new Action<AudioLine>(RaiseVolumeChangedEvent)); break;
                    }

                    _audioLineControls.Add(lineControl);
                }
            }
            finally
            {
                if (pmc != IntPtr.Zero)
                    Marshal.FreeHGlobal((IntPtr)pmc);
            }
        }

        private AudioLineControl GetControlByType(MIXERCONTROL_CONTROLTYPE type)
        {
            foreach (var control in Controls)
                if (control.ControlType == type)
                    return control;

            return null;
        }

        #region Event raising

        void RaiseVolumeChangedEvent(AudioLine audioLine)
        {
            if (this.VolumeChanged != null)
                this.VolumeChanged(audioLine);
        }

        void RaiseBalanceChangedEvent(AudioLine audioLine)
        {
            if (this.BalanceChanged != null)
                this.BalanceChanged(audioLine);
        }

        void RaiseMuteChangedEvent(AudioLine audioLine)
        {
            if (this.MuteChanged != null)
                this.MuteChanged(audioLine);
        }

        #endregion
    }
}
