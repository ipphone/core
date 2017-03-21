using System;
using System.Collections.Generic;
using System.Text;
using ContactPoint.Common.Audio;

namespace ContactPoint.Core.Audio
{
    internal class AudioPlayerProxy : IAudioPlayer
    {
        private AudioLibrary.Interfaces.IAudioPlayer _audioPlayer;

        public AudioPlayerProxy(AudioLibrary.Interfaces.IAudioPlayer audioPlayer)
        {
            _audioPlayer = audioPlayer;
        }

        public void Play(string filename)
        {
            _audioPlayer.Play(filename);
        }

        public void Stop()
        {
            _audioPlayer.Stop();
        }

        public void Dispose()
        {
            _audioPlayer.Dispose();
        }
    }
}
