using System;
using System.Collections.Generic;
using System.Text;

namespace ContactPoint.Common.Audio
{
    public interface IAudio : IDisposable
    {
        /// <summary>
        /// Occurs when new devices added
        /// </summary>
        event Action<IEnumerable<IAudioDevice>> AudioDevicesAdded;

        /// <summary>
        /// Occurs when devices removed
        /// </summary>
        event Action<IEnumerable<IAudioDevice>> AudioDevicesRemoved;

        /// <summary>
        /// Occurs when recording audio device changed
        /// </summary>
        event Action<IAudioDevice> RecordingDeviceChanged;

        /// <summary>
        /// Occurs when playback audio device changed
        /// </summary>
        event Action<IAudioDevice> PlaybackDeviceChanged;

        /// <summary>
        /// All available audio devices in system
        /// </summary>
        IEnumerable<IAudioDevice> AudioDevices { get; }

        /// <summary>
        /// Headphones audio device
        /// </summary>
        IAudioDevice PlaybackDevice { get; set; }

        /// <summary>
        /// Microphone audio device
        /// </summary>
        IAudioDevice RecordingDevice { get; set; }

        /// <summary>
        /// Search device by name
        /// </summary>
        /// <param name="name">Device name</param>
        /// <returns>IAudioDevice object or null if not found</returns>
        IAudioDevice GetDeviceByName(string name);
    }
}
