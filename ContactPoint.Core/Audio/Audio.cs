using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using ContactPoint.Common;
using ContactPoint.Common.Audio;

namespace ContactPoint.Core.Audio
{
    internal class Audio : IAudio
    {
        private readonly AudioLibrary.Interfaces.IAudio _internalAudio;
        private readonly List<AudioDevice> _audioDevices = new List<AudioDevice>();
        private readonly AsyncCallback _raiseDeviceChangedCallback;
        private readonly AsyncCallback _raiseDeviceCollectionChangedCallback;
        private readonly ManualResetEvent _initWaitHandle = new ManualResetEvent(false);

        public event Action<IAudioDevice> RecordingDeviceChanged;
        public event Action<IAudioDevice> PlaybackDeviceChanged;
        public event Action<IEnumerable<IAudioDevice>> AudioDevicesAdded;
        public event Action<IEnumerable<IAudioDevice>> AudioDevicesRemoved;

        public ICore Core { get; private set; }

        public IEnumerable<IAudioDevice> AudioDevices
        {
            get 
            {
                return ConvertAudioDeviceCollection(_audioDevices);
            }
        }

        private AudioDevice _playbackDevice = null;
        public IAudioDevice PlaybackDevice
        {
            get { return _playbackDevice; }
            set
            {
                _playbackDevice = value as AudioDevice;

                if (_playbackDevice != null) _internalAudio.PlaybackDevice = _playbackDevice.InternalAudioDevice;
                else _internalAudio.PlaybackDevice = null;

                RaisePlaybackDeviceChanged(_playbackDevice);
            }
        }

        private AudioDevice _recordingDevice = null;
        public IAudioDevice RecordingDevice
        {
            get { return _recordingDevice; }
            set
            {
                _recordingDevice = value as AudioDevice;

                if (_recordingDevice != null) _internalAudio.RecordingDevice = _recordingDevice.InternalAudioDevice;
                else _internalAudio.RecordingDevice = null;

                RaiseRecordingDeviceChanged(_recordingDevice);
            }
        }

        public Audio(Core core)
        {
            Logger.LogNotice("Setting up audio devices");

            Core = core;

            _raiseDeviceChangedCallback = RaiseEventCallback<IAudioDevice>;
            _raiseDeviceCollectionChangedCallback = RaiseEventCallback<List<IAudioDevice>>;

            _internalAudio = AudioLoader.LoadAudio();

            Logger.LogNotice("Loading audio devices.");
            foreach (var device in _internalAudio.AudioDevices)
                if (device != null)
                {
                    var dev = new AudioDevice(this, device);
                    Logger.LogNotice(String.Format("Audio device loaded: {0} ({1}) ", dev.Name, dev.Type));

                    _audioDevices.Add(dev);
                }

            _internalAudio.AudioDevicesAdded += OnAudioDevicesAdded;
            _internalAudio.AudioDevicesRemoved += OnAudioDevicesRemoved;

            _internalAudio.PlaybackDeviceChanged += OnPlaybackDeviceChanged;
            _internalAudio.RecordingDeviceChanged += OnRecordingDeviceChanged;

            InitializeDevices(true);
        }

        public IAudioDevice GetDeviceByName(string deviceName)
        {
            foreach (var device in _audioDevices)
                if (device.Name == deviceName)
                    return device;

            return null;
        }

        public void Dispose()
        {
            if (RecordingDevice != null) Core.SettingsManager.Set("RecordingDeviceVolume", RecordingDevice.Volume);
            if (PlaybackDevice != null) Core.SettingsManager.Set("PlaybackDeviceVolume", PlaybackDevice.Volume);

            _initWaitHandle.Dispose();
            _internalAudio.Dispose();
        }

        void InitializeDevices(bool wait = false)
        {
            var recordingDevice = GetDeviceByName(Core.SettingsManager.GetValueOrSetDefault<string>("RecordingDevice", _internalAudio.DefaultRecordingDevice != null ? _internalAudio.DefaultRecordingDevice.Name : String.Empty));
            var playbackDevice = GetDeviceByName(Core.SettingsManager.GetValueOrSetDefault<string>("PlaybackDevice", _internalAudio.DefaultPlaybackDevice != null ? _internalAudio.DefaultPlaybackDevice.Name : String.Empty));

            if (recordingDevice == null || playbackDevice == null)
            {
                if (wait && _initWaitHandle.WaitOne(TimeSpan.FromSeconds(5)))
                {
                    InitializeDevices();
                }

                return;
            }

            PlaybackDevice = playbackDevice;
            RecordingDevice = recordingDevice;

            _initWaitHandle.Set();

            if (RecordingDevice != null) RecordingDevice.Volume = Core.SettingsManager.GetValueOrSetDefault("RecordingDeviceVolume", 50);
            if (PlaybackDevice != null) PlaybackDevice.Volume = Core.SettingsManager.GetValueOrSetDefault("PlaybackDeviceVolume", 50);
        }

        void OnAudioDevicesAdded(IEnumerable<AudioLibrary.Interfaces.IAudioDevice> addedDevices)
        {
            var tempList = new List<AudioDevice>();

            foreach (var device in addedDevices)
                if (device != null)
                    tempList.Add(new AudioDevice(this, device));

            _audioDevices.AddRange(tempList);

            RaiseAudioDevicesAdded(ConvertAudioDeviceCollection(tempList));

            _initWaitHandle.Set();
        }

        void OnAudioDevicesRemoved(IEnumerable<AudioLibrary.Interfaces.IAudioDevice> removedDevices)
        {
            var tempList = new List<AudioDevice>();

            foreach (var device in removedDevices)
                tempList.Add(FindAudioDevice(device));

            foreach (var device in tempList)
                _audioDevices.Remove(device);

            RaiseAudioDevicesRemoved(ConvertAudioDeviceCollection(tempList));
        }

        void OnPlaybackDeviceChanged(AudioLibrary.Interfaces.IAudioDevice device)
        {
            if (device != null && (_playbackDevice != null && device != _playbackDevice.InternalAudioDevice || _playbackDevice == null && device != null))
            {
                var localDevice = FindAudioDevice(device);

                if (localDevice == null) localDevice = new AudioDevice(this, device);

                PlaybackDevice = localDevice;
            }
            else if (device == null && _playbackDevice != null)
                PlaybackDevice = null;
        }

        void OnRecordingDeviceChanged(AudioLibrary.Interfaces.IAudioDevice device)
        {
            if (device != null && (_recordingDevice != null && device != _recordingDevice.InternalAudioDevice || _recordingDevice == null && device != null))
            {
                var localDevice = FindAudioDevice(device);

                if (localDevice == null) localDevice = new AudioDevice(this, device);

                RecordingDevice = localDevice;
            }
            else if (device == null && _recordingDevice != null)
                RecordingDevice = null;
        }

        private void RaiseAudioDevicesAdded(List<IAudioDevice> deviceCollection)
        {
            SafeRaiseEvent(AudioDevicesAdded, deviceCollection, _raiseDeviceCollectionChangedCallback);
        }

        private void RaiseAudioDevicesRemoved(List<IAudioDevice> deviceCollection)
        {
            SafeRaiseEvent(AudioDevicesRemoved, deviceCollection, _raiseDeviceCollectionChangedCallback);

            if (deviceCollection.Contains(RecordingDevice)) RecordingDevice = FindAudioDevice(_internalAudio.DefaultRecordingDevice);
            if (deviceCollection.Contains(PlaybackDevice)) PlaybackDevice = FindAudioDevice(_internalAudio.DefaultPlaybackDevice);
        }

        private void RaiseRecordingDeviceChanged(IAudioDevice device)
        {
            SafeRaiseEvent(RecordingDeviceChanged, device, _raiseDeviceChangedCallback);
        }

        private void RaisePlaybackDeviceChanged(IAudioDevice device)
        {
            SafeRaiseEvent(PlaybackDeviceChanged, device, _raiseDeviceChangedCallback);
        }

        AudioDevice FindAudioDevice(AudioLibrary.Interfaces.IAudioDevice internalAudioDevice)
        {
            foreach (var audioDevice in _audioDevices)
                if (audioDevice.InternalAudioDevice == internalAudioDevice)
                    return audioDevice;

            return null;
        }

        List<IAudioDevice> ConvertAudioDeviceCollection(IEnumerable<AudioDevice> collection)
        {
            var resultList = new List<IAudioDevice>();

            foreach (var device in collection)
                resultList.Add(device);

            return resultList;
        }

        static void SafeRaiseEvent<T>(Action<T> del, T obj, AsyncCallback asyncCallback)
        {
            if (del == null) return;

            var invocationList = del.GetInvocationList();

            foreach (var current in invocationList)
                ((Action<T>)current).BeginInvoke(obj, asyncCallback, current);
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
    }
}
