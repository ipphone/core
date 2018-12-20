using System;
using AudioLibrary.Interfaces;
using AudioLibrary.PjSIP.ManagedWatcher;

namespace AudioLibrary.PjSIP
{
    internal class AudioDevice : IAudioDevice
    {
        private AudioDevicesWatcher.AudioDeviceDescriptor _audioDeviceDescriptor;

        public event Action<IAudioDevice> VolumeChanged;
        public event Action<IAudioDevice> MuteChanged;

        internal int Index { get; private set; }

        public Audio Audio { get; private set; }
        public string Name { get; private set; }
        public bool PlaybackSupport { get; private set; }
        public bool RecordingSupport { get; private set; }

        internal AudioDevicesWatcher.AudioDeviceDescriptor AudioDeviceDescriptor 
        {
            get { return _audioDeviceDescriptor; }
            set 
            { 
                _audioDeviceDescriptor = value;

                if (_audioDeviceDescriptor != null) Name = _audioDeviceDescriptor.Name;
            }
        }

        public int Volume
        {
            get { return PlaybackSupport ? Audio.SpeakerVolume : Audio.MicVolume; }
            set
            {
                if (PlaybackSupport) Audio.SpeakerVolume = value;
                else Audio.MicVolume = value;

                VolumeChanged?.Invoke(this);
            }
        }

        public bool Mute
        {
            get { return Audio.MicMute; }
            set 
            { 
                Audio.MicMute = value; 
                MuteChanged?.Invoke(this);
            }
        }

        internal AudioDevice(Audio audio, Imports.PjAudioDeviceInfo audioDeviceInfo, int index)
        {
            Audio = audio;
            Index = index;

            Name = audioDeviceInfo.Name;
            PlaybackSupport = audioDeviceInfo.OutputCount > 0;
            RecordingSupport = audioDeviceInfo.InputCount > 0;
        }

        public IAudioPlayer CreateAudioPlayer()
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        { }

        IAudio IAudioDevice.Audio 
        {
            get { return Audio; }
        }
    }
}
