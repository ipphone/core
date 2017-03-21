using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using AudioLibrary.Interfaces;
using System.Runtime.InteropServices;
using AudioLibrary.WMME.Native;

namespace AudioLibrary.WMME
{
    internal class Audio : IAudio
    {
        public event Action<IEnumerable<IAudioDevice>> AudioDevicesAdded;
        public event Action<IEnumerable<IAudioDevice>> AudioDevicesRemoved;
        public event Action<IAudioDevice> PlaybackDeviceChanged;
        public event Action<IAudioDevice> RecordingDeviceChanged;

        private List<AudioDevice> _audioDevices = new List<AudioDevice>();
        private AudioDevice _playbackDevice = null;
        private AudioDevice _recordingDevice = null;
        private NativeWindow _callbackForm;
        private Timer _reloadTimer;
        private bool _reloadDevicesInProgress = false;

        public IEnumerable<IAudioDevice> AudioDevices => _audioDevices;

        public IAudioDevice PlaybackDevice 
        {
            get { return _playbackDevice; }
            set 
            { 
                try
                {
                    _playbackDevice = GetAudioDeviceByName(value.Name, true);

                    if (PlaybackDeviceChanged != null)
                        PlaybackDeviceChanged.BeginInvoke(_playbackDevice, null, null);
                }
                catch
                {
                    _playbackDevice = null;
                }
            }
        }

        public IAudioDevice RecordingDevice 
        {
            get { return _recordingDevice; }
            set
            {
                try
                {
                    _recordingDevice = GetAudioDeviceByName(value.Name, false);

                    if (RecordingDeviceChanged != null)
                        RecordingDeviceChanged.BeginInvoke(_recordingDevice, null, null);
                }
                catch
                {
                    _recordingDevice = null;
                }
            }
        }

        public IAudioDevice DefaultPlaybackDevice
        {
            get { return GetDefaultDevice(MixerType.Playback); }
        }

        public IAudioDevice DefaultRecordingDevice
        {
            get { return GetDefaultDevice(MixerType.Recording); }
        }

        internal IWin32Window CallbackForm
        {
            get { return _callbackForm; }
        }

        public Audio()
        {
            _callbackForm = new AudioForm(this);

            ReloadAudioDevices();

            PlaybackDevice = GetDefaultDevice(MixerType.Playback);
            RecordingDevice = GetDefaultDevice(MixerType.Recording);

            _reloadTimer = new Timer() { Interval = 2000, Enabled = true };
            _reloadTimer.Tick += new EventHandler(ReloadTimerTick);
        }

        public void Dispose()
        {
            _callbackForm.DestroyHandle();
            _reloadTimer.Stop();
        }

        internal void HandleDeviceAdding()
        {
            ReloadAudioDevices();
        }

        internal void HandleDeviceRemoving()
        {
            ReloadAudioDevices();
        }

        internal void HandleLineChanged(IntPtr hMixer, uint lineId)
        {
            var audioDevice = GetAudioDeviceByHandle(hMixer);
            if (audioDevice == null) return;

            audioDevice.HandleLineChanged(lineId);
        }

        internal void HandleLineControlChanged(IntPtr hMixer, uint controlId)
        {
            var audioDevice = GetAudioDeviceByHandle(hMixer);
            if (audioDevice == null) return;

            audioDevice.HandleLineControlChanged(controlId);
        }

        private AudioDevice GetDefaultDevice(MixerType mixerType)
        {
            MMErrors errorCode = 0;
            IntPtr hWave = IntPtr.Zero;
            IntPtr hMixer = IntPtr.Zero;

            try
            {
                WAVEFORMATEX waveFormat = WAVEFORMATEX.Empty;

                waveFormat.formatTag = WaveFormatTag.PCM;
                waveFormat.nChannels = 1;
                waveFormat.nSamplesPerSec = 8000;
                waveFormat.wBitsPerSample = 8;
                waveFormat.nBlockAlign = (short)(waveFormat.nChannels * (waveFormat.wBitsPerSample / 8));
                waveFormat.nAvgBytesPerSec = waveFormat.nSamplesPerSec * waveFormat.nBlockAlign;
                waveFormat.cbSize = 0;

                // Get default mixer HWND
                if (mixerType == MixerType.Recording)
                    errorCode = (MMErrors)WaveNative.waveInOpen(out hWave, WaveNative.WAVE_MAPPER, ref waveFormat, CallbackForm.Handle, IntPtr.Zero, (int)CallBackFlag.CALLBACK_WINDOW);
                else if (mixerType == MixerType.Playback)
                    errorCode = (MMErrors)WaveNative.waveOutOpen(out hWave, WaveNative.WAVE_MAPPER, ref waveFormat, CallbackForm.Handle, IntPtr.Zero, (int)CallBackFlag.CALLBACK_WINDOW);
                if (errorCode != MMErrors.MMSYSERR_NOERROR)
                    // TODO: Log error!!!
                    //throw new MixerException(errorCode, Audio.GetErrorDescription(FuncName.fnWaveInOpen, errorCode));
                    return null;

                if (mixerType == MixerType.Recording)
                    errorCode = (MMErrors)MixerNative.mixerOpen(out hMixer, hWave, IntPtr.Zero, IntPtr.Zero, ((uint)MIXER_OBJECTFLAG.HWAVEIN));
                else if (mixerType == MixerType.Playback)
                    errorCode = (MMErrors)MixerNative.mixerOpen(out hMixer, hWave, IntPtr.Zero, IntPtr.Zero, ((uint)MIXER_OBJECTFLAG.HWAVEOUT));
                if (errorCode != MMErrors.MMSYSERR_NOERROR)
                    // TODO: Log error!!!
                    //throw new MixerException(errorCode, Mixers.GetErrorDescription(FuncName.fnMixerOpen, errorCode));
                    return null;

                int deviceId = -1;
                errorCode = (MMErrors)MixerNative.mixerGetID(hMixer, ref deviceId, MIXER_OBJECTFLAG.MIXER);
                if (errorCode != MMErrors.MMSYSERR_NOERROR)
                    // TODO: Log error!!!
                    //throw new MixerException(errorCode, Mixers.GetErrorDescription(FuncName.fnMixerGetID, errorCode));
                    return null;

                MIXERCAPS wic = new MIXERCAPS();
                errorCode = (MMErrors)MixerNative.mixerGetDevCaps(deviceId, ref wic, Marshal.SizeOf(wic));
                if (errorCode != MMErrors.MMSYSERR_NOERROR)
                    // TODO: Log error!!!
                    //throw new MixerException(errorCode, GetErrorDescription(FuncName.fnMixerGetDevCaps, errorCode));
                    return null;

                return GetAudioDeviceByName(wic.szPname, mixerType == MixerType.Playback);
            }
            finally
            {
                if (hMixer != IntPtr.Zero)
                    MixerNative.mixerClose(hMixer);
                if (hWave != IntPtr.Zero && mixerType == MixerType.Playback)
                    WaveNative.waveOutClose(hWave);
                if (hWave != IntPtr.Zero && mixerType == MixerType.Recording)
                    WaveNative.waveInClose(hWave);
            }
        }

        void ReloadTimerTick(object sender, EventArgs e)
        {
            if (!_reloadDevicesInProgress)
                Task.Factory.StartNew(ReloadAudioDevices);
        }

        #region Audio devices list methods

        private void ReloadAudioDevices()
        {
            if (_reloadDevicesInProgress) return;

            MMErrors errorCode = 0;
            IntPtr hMixer = IntPtr.Zero;

            try
            {
                _reloadDevicesInProgress = true;

                var newAudioDevices = new List<AudioDeviceDescriptor>();
                MIXERCAPS wic = new MIXERCAPS();

                int iNumDevs = MixerNative.mixerGetNumDevs();

                for (int i = 0; i < iNumDevs; i++)
                {
                    // Get info about the next device 
                    errorCode = (MMErrors)MixerNative.mixerGetDevCaps(i, ref wic, Marshal.SizeOf(wic));
                    if (errorCode != MMErrors.MMSYSERR_NOERROR)
                        throw new MixerException(errorCode, GetErrorDescription(FuncName.fnMixerGetDevCaps, errorCode));

                    errorCode = (MMErrors)MixerNative.mixerOpen(out hMixer, i, IntPtr.Zero, IntPtr.Zero, 0);
                    if (errorCode != MMErrors.MMSYSERR_NOERROR)
                        throw new MixerException(errorCode, GetErrorDescription(FuncName.fnMixerOpen, errorCode));

                    MIXERLINE mxl = new MIXERLINE();
                    mxl.cbStruct = (uint)Marshal.SizeOf(mxl);

                	var deviceDescriptor = new AudioDeviceDescriptor()
                	                       	{
                	                       		DeviceId = i,
                	                       		Name = wic.szPname
                	                       	};

					foreach (var mixerlineComponenttype in AudioDevice.RecordingLineTypes)
                	{
						mxl.dwComponentType = mixerlineComponenttype;
						if (MixerNative.mixerGetLineInfo(hMixer, ref mxl, MIXER_GETLINEINFOF.COMPONENTTYPE) == 0)
						{
							deviceDescriptor.RecordingSupport = true;
							deviceDescriptor.RecordingComponentType = mixerlineComponenttype;

							break;
						}
					}

					foreach (var mixerlineComponenttype in AudioDevice.PlaybackLineTypes)
					{
						mxl.dwComponentType = mixerlineComponenttype;
						if (MixerNative.mixerGetLineInfo(hMixer, ref mxl, MIXER_GETLINEINFOF.COMPONENTTYPE) == 0)
						{
							deviceDescriptor.PlaybackSupport = true;
							deviceDescriptor.PlaybackComponentType = mixerlineComponenttype;

							break;
						}
					}

					if (deviceDescriptor.PlaybackSupport || deviceDescriptor.RecordingSupport)
					{
						if (deviceDescriptor.PlaybackSupport && deviceDescriptor.RecordingSupport)
						{
							var playbackDeviceDescriptor = new AudioDeviceDescriptor()
							                               	{
							                               		DeviceId = deviceDescriptor.DeviceId,
							                               		Name = deviceDescriptor.Name,
							                               		PlaybackSupport = true,
							                               		RecordingSupport = false,
																PlaybackComponentType = deviceDescriptor.PlaybackComponentType
							                               	};

							var recordingDeviceDescriptor = new AudioDeviceDescriptor()
							                                	{
							                                		DeviceId = deviceDescriptor.DeviceId,
							                                		Name = deviceDescriptor.Name,
							                                		PlaybackSupport = false,
							                                		RecordingSupport = true,
																	RecordingComponentType = deviceDescriptor.RecordingComponentType
							                                	};

							newAudioDevices.Add(playbackDeviceDescriptor);
							newAudioDevices.Add(recordingDeviceDescriptor);
						}
						else
							newAudioDevices.Add(deviceDescriptor);
					}
                }

                if (newAudioDevices.Count > 0 || (_audioDevices.Count > 0 && iNumDevs == 0))
                    UpdateAudioDevicesLists(newAudioDevices);
            }
            finally
            {
                if (hMixer != IntPtr.Zero)
                    MixerNative.mixerClose(hMixer);

                _reloadDevicesInProgress = false;
            }
        }

        private void UpdateAudioDevicesLists(ICollection<AudioDeviceDescriptor> audioDevicesDescriptors)
        {
            var audioDevices = new List<AudioDevice>();
            var removedDevices = new List<AudioDevice>();
            var addedDevices = new List<AudioDevice>();

            foreach (var deviceDescriptor in audioDevicesDescriptors)
                if (GetAudioDeviceByName(deviceDescriptor.Name, deviceDescriptor.PlaybackSupport) == null)
                    addedDevices.Add(deviceDescriptor.CreateAudioDevice(this));

            foreach (var device in _audioDevices)
                if (!ContainsAudioDeviceDescriptor(device.Name, audioDevicesDescriptors))
                    removedDevices.Add(device);

            foreach (var device in addedDevices)
                audioDevices.Add(device);

            foreach (var device in removedDevices)
                audioDevices.Remove(device);

            _audioDevices = audioDevices;

            if (AudioDevicesAdded != null && addedDevices.Count > 0)
            {
                AudioDevicesAdded.BeginInvoke(CastAudioDeviceCollection(addedDevices), null, null);

                // Try to set device.
                // Don't be confused on this lines - we setting default audio device in Device setter when assigning null.
                //if (PlaybackDevice == null) PlaybackDevice = null;
                //if (RecordingDevice == null) RecordingDevice = null;
            }
            if (AudioDevicesRemoved != null && removedDevices.Count > 0)
            {
                if (PlaybackDevice != null && GetAudioDeviceByName(PlaybackDevice.Name, true) == null) PlaybackDevice = null;
                if (RecordingDevice != null && GetAudioDeviceByName(RecordingDevice.Name, false) == null) RecordingDevice = null;

                AudioDevicesRemoved.BeginInvoke(CastAudioDeviceCollection(removedDevices), null, null);
            }
        }

        private IEnumerable<IAudioDevice> CastAudioDeviceCollection(IEnumerable<AudioDevice> collection)
        {
            var resultList = new List<IAudioDevice>();

            foreach (var el in collection)
                resultList.Add(el);

            return resultList;
        }

        private AudioDevice GetAudioDeviceByName(string name, bool isPlayback)
        {
			return GetAudioDeviceByName(name, isPlayback, _audioDevices);
        }

        private AudioDevice GetAudioDeviceByName(string name, bool isPlayback, IEnumerable<AudioDevice> audioDevices)
        {
            foreach (var audioDevice in audioDevices)
                if (audioDevice.Name == name && audioDevice.PlaybackSupport == isPlayback)
                    return audioDevice;

            return null;
        }

        private AudioDevice GetAudioDeviceByHandle(IntPtr handle)
        {
            foreach (var audioDevice in _audioDevices)
                if (audioDevice.Handle == handle)
                    return audioDevice;

            return null;
        }

        private bool ContainsAudioDeviceDescriptor(string name, IEnumerable<AudioDeviceDescriptor> audioDevicesDescriptors)
        {
            foreach (var audioDeviceDescriptor in audioDevicesDescriptors)
                if (audioDeviceDescriptor.Name == name)
                    return true;

            return false;
        }

        #endregion

        #region Helper methods

        internal static string GetErrorDescription(FuncName funcName, MMErrors errorCode)
        {
            string errorDesc = "";

            switch (funcName)
            {
                case FuncName.fnWaveOutOpen:
                case FuncName.fnWaveInOpen:
                    switch (errorCode)
                    {
                        case MMErrors.MMSYSERR_ALLOCATED:
                            errorDesc = "Specified resource is already allocated.";
                            break;
                        case MMErrors.MMSYSERR_BADDEVICEID:
                            errorDesc = "Specified device identifier is out of range.";
                            break;
                        case MMErrors.MMSYSERR_NODRIVER:
                            errorDesc = "No device driver is present.";
                            break;
                        case MMErrors.MMSYSERR_NOMEM:
                            errorDesc = "Unable to allocate or lock memory.";
                            break;
                        case MMErrors.WAVERR_BADFORMAT:
                            errorDesc = "Attempted to open with an unsupported waveform-audio format.";
                            break;
                        case MMErrors.WAVERR_SYNC:
                            errorDesc = "The device is synchronous but waveOutOpen was called without using the WAVE_ALLOWSYNC flag.";
                            break;
                    }
                    break;
                case FuncName.fnMixerOpen:
                    switch (errorCode)
                    {
                        case MMErrors.MMSYSERR_ALLOCATED:
                            errorDesc = "The specified resource is already allocated by the maximum number of clients possible.";
                            break;
                        case MMErrors.MMSYSERR_BADDEVICEID:
                            errorDesc = "The uMxId parameter specifies an invalid device identifier.";
                            break;
                        case MMErrors.MMSYSERR_INVALFLAG:
                            errorDesc = "One or more flags are invalid.";
                            break;
                        case MMErrors.MMSYSERR_INVALHANDLE:
                            errorDesc = "The uMxId parameter specifies an invalid handle.";
                            break;
                        case MMErrors.MMSYSERR_INVALPARAM:
                            errorDesc = "One or more parameters are invalid.";
                            break;
                        case MMErrors.MMSYSERR_NODRIVER:
                            errorDesc = "No device driver is present.";
                            break;
                        case MMErrors.MMSYSERR_NOMEM:
                            errorDesc = "Unable to allocate or lock memory.";
                            break;
                    }
                    break;
                case FuncName.fnMixerGetID:
                case FuncName.fnMixerGetLineInfo:
                case FuncName.fnMixerGetLineControls:
                case FuncName.fnMixerGetControlDetails:
                case FuncName.fnMixerSetControlDetails:
                    switch (errorCode)
                    {
                        case MMErrors.MIXERR_INVALCONTROL:
                            errorDesc = "The control reference is invalid.";
                            break;
                        case MMErrors.MIXERR_INVALLINE:
                            errorDesc = "The audio line reference is invalid.";
                            break;
                        case MMErrors.MMSYSERR_BADDEVICEID:
                            errorDesc = "The hmxobj parameter specifies an invalid device identifier.";
                            break;
                        case MMErrors.MMSYSERR_INVALFLAG:
                            errorDesc = "One or more flags are invalid.";
                            break;
                        case MMErrors.MMSYSERR_INVALHANDLE:
                            errorDesc = "The hmxobj parameter specifies an invalid handle.";
                            break;
                        case MMErrors.MMSYSERR_INVALPARAM:
                            errorDesc = "One or more parameters are invalid.";
                            break;
                        case MMErrors.MMSYSERR_NODRIVER:
                            errorDesc = "No device driver is present.";
                            break;
                    }
                    break;
                case FuncName.fnMixerClose:
                    switch (errorCode)
                    {
                        case MMErrors.MMSYSERR_INVALHANDLE:
                            errorDesc = "Specified device handle is invalid.";
                            break;
                    }
                    break;
                case FuncName.fnWaveOutClose:
                case FuncName.fnWaveInClose:
                case FuncName.fnWaveInGetDevCaps:
                case FuncName.fnWaveOutGetDevCaps:
                case FuncName.fnMixerGetDevCaps:
                    switch (errorCode)
                    {
                        case MMErrors.MMSYSERR_BADDEVICEID:
                            errorDesc = "Specified device identifier is out of range.";
                            break;
                        case MMErrors.MMSYSERR_INVALHANDLE:
                            errorDesc = "Specified device handle is invalid.";
                            break;
                        case MMErrors.MMSYSERR_NODRIVER:
                            errorDesc = "No device driver is present.";
                            break;
                        case MMErrors.MMSYSERR_NOMEM:
                            errorDesc = "Unable to allocate or lock memory.";
                            break;
                        case MMErrors.WAVERR_STILLPLAYING:
                            errorDesc = "There are still buffers in the queue.";
                            break;
                    }
                    break;
                case FuncName.fnCustom:
                    switch (errorCode)
                    {
                        case (MMErrors)1000:
                            errorDesc = "Device Not Found.";
                            break;
                    }
                    break;
            }
            return errorDesc;
        }

        #endregion
    }

    internal enum FuncName
    {
        fnWaveInOpen,
        fnWaveInClose,
        fnWaveInGetDevCaps,
        fnWaveOutOpen,
        fnWaveOutClose,
        fnWaveOutGetDevCaps,
        fnMixerOpen,
        fnMixerGetID,
        fnMixerClose,
        fnMixerGetLineInfo,
        fnMixerGetLineControls,
        fnMixerGetControlDetails,
        fnMixerSetControlDetails,
        fnMixerGetDevCaps,
        fnCustom
    };
}
