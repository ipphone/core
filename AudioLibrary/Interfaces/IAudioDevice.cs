using System;
using System.Collections.Generic;
using System.Text;

namespace AudioLibrary.Interfaces
{
	/// <summary>
	/// Audio device hi-level representation.
	/// Each audio device can support only recording or playback!
	/// </summary>
    public interface IAudioDevice : IDisposable
    {
        event Action<IAudioDevice> VolumeChanged;
        event Action<IAudioDevice> MuteChanged;

        IAudio Audio { get; }

        string Name { get; }
        bool PlaybackSupport { get; }
        bool RecordingSupport { get; }

        int Volume { get; set; }
        bool Mute { get; set; }

        IAudioPlayer CreateAudioPlayer();
    }
}
