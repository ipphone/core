using System;
using System.Collections.Generic;
using ContactPoint.BaseDesign;
using ContactPoint.Common;
using ContactPoint.NotifyControls;

namespace ContactPoint.Services
{
    internal class AudioService : IDisposable
    {
        private readonly ICore _core;

        public AudioService(ICore core)
        {
            _core = core;

            _core.Audio.AudioDevicesAdded += AudioDevicesAdded;
            _core.Audio.AudioDevicesRemoved += AudioDevicesRemoved;
        }

        void AudioDevicesAdded(IEnumerable<Common.Audio.IAudioDevice> obj)
        {
            NotifyManager.NotifyUser(new AudioDevicesAddedNotifyControl { AudioDevices = obj, Core = _core });
        }

        void AudioDevicesRemoved(IEnumerable<Common.Audio.IAudioDevice> obj)
        {
            NotifyManager.NotifyUser(new AudioDevicesRemovedNotifyControl { AudioDevices = obj, Core = _core });
        }

        public void Dispose()
        {
            _core.Audio.AudioDevicesAdded -= AudioDevicesAdded;
            _core.Audio.AudioDevicesRemoved -= AudioDevicesRemoved;

            _core.Audio.Dispose();
        }
    }
}
