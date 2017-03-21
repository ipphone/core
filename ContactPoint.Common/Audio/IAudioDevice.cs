using System;
using System.Collections.Generic;
using System.Text;

namespace ContactPoint.Common.Audio
{
    /// <summary>
    /// Interface represents audio device from WaveLibMixer
    /// </summary>
    public interface IAudioDevice
    {
        /// <summary>
        /// Occurs when user or system changed volume on device
        /// </summary>
        event Action<IAudioDevice> VolumeChanged;

        /// <summary>
        /// Occurs when user or system changed mute flag on device
        /// </summary>
        event Action<IAudioDevice> MuteChanged;

        /// <summary>
        /// Audio manager object
        /// </summary>
        IAudio Audio { get; }

        /// <summary>
        /// Device name
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Device type
        /// </summary>
        AudioDeviceType Type { get; }

        /// <summary>
        /// Current device volume from 0 to 100
        /// </summary>
        int Volume { get; set; }

        /// <summary>
        /// Toggle mute of device
        /// </summary>
        bool Mute { get; set; }

        /// <summary>
        /// Create audio player for this device.
        /// If device is not support playback - returns null
        /// </summary>
        /// <returns>AudioPlayer object or null if playback not supported</returns>
        IAudioPlayer CreateAudioPlayer();
    }
}
