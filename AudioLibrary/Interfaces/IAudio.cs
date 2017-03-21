using System;
using System.Collections.Generic;
using System.Text;

namespace AudioLibrary.Interfaces
{
    public interface IAudio : IDisposable
    {
        event Action<IEnumerable<IAudioDevice>> AudioDevicesAdded;
        event Action<IEnumerable<IAudioDevice>> AudioDevicesRemoved;
        event Action<IAudioDevice> PlaybackDeviceChanged;
        event Action<IAudioDevice> RecordingDeviceChanged;

        IEnumerable<IAudioDevice> AudioDevices { get; }
        IAudioDevice PlaybackDevice { get; set; }
        IAudioDevice RecordingDevice { get; set; }

        IAudioDevice DefaultPlaybackDevice { get; }
        IAudioDevice DefaultRecordingDevice { get; }
    }
}
