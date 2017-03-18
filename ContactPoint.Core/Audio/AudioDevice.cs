using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using ContactPoint.Common;
using ContactPoint.Common.Audio;

namespace ContactPoint.Core.Audio
{
    internal class AudioDevice : IAudioDevice
    {
        private AudioLibrary.Interfaces.IAudioDevice _internalAudioDevice;
        private AsyncCallback _valueChangedCallback;

        public event Action<IAudioDevice> VolumeChanged;
        public event Action<IAudioDevice> MuteChanged;

        public IAudio Audio { get; private set; }

        public string Name
        {
            get { return _internalAudioDevice.Name; }
        }

        public AudioDeviceType Type
        {
            get { return _internalAudioDevice.PlaybackSupport ? AudioDeviceType.Playback : AudioDeviceType.Recording; }
        }

        public int Volume
        {
            get { return _internalAudioDevice.Volume; }
            set { _internalAudioDevice.Volume = value; }
        }

        public bool Mute
        {
            get { return _internalAudioDevice.Mute; }
            set { _internalAudioDevice.Mute = value; }
        }

        internal AudioLibrary.Interfaces.IAudioDevice InternalAudioDevice
        {
            get { return _internalAudioDevice; }
        }

        public AudioDevice(IAudio audio, AudioLibrary.Interfaces.IAudioDevice audioDevice)
        {
            _internalAudioDevice = audioDevice;
            _valueChangedCallback = new AsyncCallback(RaiseEventCallback<IAudioDevice>);
            Audio = audio;

            _internalAudioDevice.VolumeChanged += new Action<AudioLibrary.Interfaces.IAudioDevice>(DefaultLineVolumeChanged);
            _internalAudioDevice.MuteChanged += new Action<AudioLibrary.Interfaces.IAudioDevice>(DefaultLineMuteChanged);
        }

        public IAudioPlayer CreateAudioPlayer()
        {
            var player = _internalAudioDevice.CreateAudioPlayer();

            if (player != null) return new AudioPlayerProxy(player);
            else return null;
        }

        void DefaultLineVolumeChanged(AudioLibrary.Interfaces.IAudioDevice obj)
        {
            RaiseVolumeChanged();
        }

        void DefaultLineMuteChanged(AudioLibrary.Interfaces.IAudioDevice obj)
        {
            RaiseMuteChanged();
        }

        void RaiseVolumeChanged()
        {
            SafeRaiseEvent<IAudioDevice>(VolumeChanged, this, _valueChangedCallback);
        }

        void RaiseMuteChanged()
        {
            SafeRaiseEvent<IAudioDevice>(MuteChanged, this, _valueChangedCallback);
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
