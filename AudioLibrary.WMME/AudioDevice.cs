using System;
using System.Collections.Generic;
using AudioLibrary.Interfaces;
using System.Runtime.InteropServices;
using AudioLibrary.WMME.Native;

namespace AudioLibrary.WMME
{
	/// <summary>
	/// Audio device hi-level representation.
	/// Each audio device can support only recording or playback!
	/// </summary>
    internal class AudioDevice : IAudioDevice
    {
    	internal static MIXERLINE_COMPONENTTYPE[] RecordingLineTypes = new MIXERLINE_COMPONENTTYPE[]
    	                                                               	{
    	                                                               		MIXERLINE_COMPONENTTYPE.DST_WAVEIN,
    	                                                               		MIXERLINE_COMPONENTTYPE.SRC_MICROPHONE,
    	                                                              		MIXERLINE_COMPONENTTYPE.SRC_LINE,
    	                                                               		MIXERLINE_COMPONENTTYPE.DST_TELEPHONE,
    	                                                               		MIXERLINE_COMPONENTTYPE.DST_VOICEIN,
    	                                                               		MIXERLINE_COMPONENTTYPE.SRC_UNDEFINED
    	                                                               	};

    	internal static MIXERLINE_COMPONENTTYPE[] PlaybackLineTypes = new MIXERLINE_COMPONENTTYPE[]
    	                                                              	{
    	                                                              		MIXERLINE_COMPONENTTYPE.DST_SPEAKERS,
    	                                                              		MIXERLINE_COMPONENTTYPE.SRC_WAVEOUT,
    	                                                              		MIXERLINE_COMPONENTTYPE.DST_HEADPHONES,
    	                                                              		MIXERLINE_COMPONENTTYPE.SRC_AUXILIARY,
    	                                                               		MIXERLINE_COMPONENTTYPE.DST_LINE,
    	                                                               		MIXERLINE_COMPONENTTYPE.SRC_ANALOG,
    	                                                              		MIXERLINE_COMPONENTTYPE.SRC_TELEPHONE,
    	                                                              		MIXERLINE_COMPONENTTYPE.SRC_SYNTHESIZER,
    	                                                              		MIXERLINE_COMPONENTTYPE.SRC_PCSPEAKER,
    	                                                              		MIXERLINE_COMPONENTTYPE.DST_UNDEFINED
    	                                                              	};

        private IntPtr _hMixer = IntPtr.Zero;
        private readonly Audio _audio;
        private readonly List<AudioLine> _audioLines = new List<AudioLine>();
		private readonly MIXERLINE_COMPONENTTYPE _componentType;

        public event Action<IAudioDevice> VolumeChanged;
        public event Action<IAudioDevice> MuteChanged;

        public string Name { get; private set; }
        public bool PlaybackSupport { get; private set; }
		public bool RecordingSupport { get; private set; }
	    public AudioLine DefaultLine { get; private set; }

        public int Volume
        {
            get { return DefaultLine.Volume; }
            set { DefaultLine.Volume = value; }
        }

        public bool Mute
        {
            get { return DefaultLine.Mute; }
            set { DefaultLine.Mute = value; }
        }

	    public IAudio Audio 
        {
            get { return _audio; }
        }

        public IEnumerable<AudioLine> Lines
        {
            get { return _audioLines; }
        }

        internal List<AudioLine> LinesInternal
        {
            get { return _audioLines; }
        }

        internal int DeviceId { get; set; }

        internal IntPtr Handle
        {
            get { return _hMixer; }
        }

        internal Audio AudioInternal
        {
            get { return _audio; }
        }

        public AudioDevice(Audio audio, AudioDeviceDescriptor deviceDescriptor)
        {
            _audio = audio;

            DeviceId = deviceDescriptor.DeviceId;
            Name = deviceDescriptor.Name;
            PlaybackSupport = deviceDescriptor.PlaybackSupport;
            RecordingSupport = deviceDescriptor.RecordingSupport;

        	_componentType = deviceDescriptor.PlaybackSupport
        	                 	? deviceDescriptor.PlaybackComponentType
        	                 	: deviceDescriptor.RecordingComponentType;

            Open();

            DefaultLine.VolumeChanged += new Action<AudioLine>(DefaultLineVolumeChanged);
            DefaultLine.MuteChanged += new Action<AudioLine>(DefaultLineMuteChanged);
        }

        public IAudioPlayer CreateAudioPlayer()
        {
            if (!PlaybackSupport) return null;

            return new AudioPlayer(this);
        }

        internal void Close()
        {
            if (_hMixer != IntPtr.Zero)
                MixerNative.mixerClose(_hMixer);

            _audioLines.Clear();
        }

        internal void Open()
        {
            var errorCode = (MMErrors)MixerNative.mixerOpen(out _hMixer, DeviceId, _audio.CallbackForm.Handle, IntPtr.Zero, MixerNative.CALLBACK_WINDOW);
            if (errorCode != MMErrors.MMSYSERR_NOERROR)
                throw new MixerException(errorCode, WMME.Audio.GetErrorDescription(FuncName.fnMixerOpen, errorCode));

            ReloadLines();

			if (RecordingSupport) DefaultLine = GetAudioLine(RecordingLineTypes);
			else if (PlaybackSupport) DefaultLine = GetAudioLine(PlaybackLineTypes);
        }

        internal void HandleLineChanged(uint lineId)
        {
            var line = GetLineById(lineId);
            if (line == null) return;

            line.ReloadValues();
        }

        internal AudioLine GetAudioLine(params MIXERLINE_COMPONENTTYPE[] componentTypes)
        {
            foreach (var componentType in componentTypes)
            {
                var line = LinesInternal.Find(x => x.ComponentType == componentType);

                if (line != null)
                    return line;
            }

            return null;
        }

        internal void HandleLineControlChanged(uint lineControlId)
        {
            var audioLineControl = FindAudioLineControlById(lineControlId);
            if (audioLineControl == null) return;

            audioLineControl.HandleChanged();
        }

        private AudioLineControl FindAudioLineControlById(uint lineControlId)
        {
            foreach (var line in _audioLines)
                foreach (var control in line.Controls)
                    if (control.Id == lineControlId)
                        return control;

            return null;
        }

        private void ReloadLines()
        {
            MMErrors errorCode = 0;

            _audioLines.Clear();

            MIXERLINE mxl = new MIXERLINE();
            uint dwDestination;
            unchecked
            {
                dwDestination = (uint)-1;
            }

            mxl.cbStruct = (uint)Marshal.SizeOf(mxl);
            mxl.dwComponentType = _componentType;

            errorCode = (MMErrors)MixerNative.mixerGetLineInfo(_hMixer, ref mxl, MIXER_GETLINEINFOF.COMPONENTTYPE);
            if (errorCode != MMErrors.MMSYSERR_NOERROR)
                throw new MixerException(errorCode, WMME.Audio.GetErrorDescription(FuncName.fnMixerGetLineInfo, errorCode));

            dwDestination = mxl.dwDestination;

            var mixLine = new AudioLine(this, mxl);

            if (mixLine.CControls != 0 && !(mixLine.CControls == 1 && mixLine.Controls[0].ControlType == MIXERCONTROL_CONTROLTYPE.MUX))
                _audioLines.Add(mixLine);

            int cConnections = (int)mxl.cConnections;
            for (int i = 0; i < cConnections; i++)
            {
                mxl.cbStruct = (uint)Marshal.SizeOf(mxl);
                mxl.dwDestination = dwDestination;
                mxl.dwSource = (uint)i;

                errorCode = (MMErrors)MixerNative.mixerGetLineInfo(Handle, ref mxl, MIXER_GETLINEINFOF.SOURCE);
                if (errorCode != MMErrors.MMSYSERR_NOERROR)
                    throw new MixerException(errorCode, WMME.Audio.GetErrorDescription(FuncName.fnMixerGetLineInfo, errorCode));

                var mixLineNew = new AudioLine(this, mxl);

                if (mixLineNew.CControls != 0)
                    _audioLines.Add(mixLineNew);
            }
        }

        private AudioLine GetLineById(uint lineId)
        {
            foreach (var line in _audioLines)
                if (line.Id == lineId)
                    return line;

            return null;
        }

        void DefaultLineVolumeChanged(AudioLine obj)
        {
            if (VolumeChanged != null)
                VolumeChanged(this);
        }

        void DefaultLineMuteChanged(AudioLine obj)
        {
            if (MuteChanged != null)
                MuteChanged(this);
        }

        public void Dispose()
        {
            Close();
        }
    }
}
