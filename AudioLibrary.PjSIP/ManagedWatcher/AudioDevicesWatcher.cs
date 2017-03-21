using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using ContactPoint.Common;

namespace AudioLibrary.PjSIP.ManagedWatcher
{
    class AudioDevicesWatcher : IDisposable
    {
        internal class AudioDeviceDescriptor
        {
            public Guid Id { get; set; }
            public string Name { get; set; }
        }

        private bool _isWorking = true;

        private readonly Audio _audio;
        private readonly List<AudioDeviceDescriptor> _devices = new List<AudioDeviceDescriptor>();

        public IEnumerable<AudioDeviceDescriptor> Devices => _devices;

        public AudioDevicesWatcher(Audio audio)
        {
            _audio = audio;

            ThreadPool.QueueUserWorkItem(x => WatcherThread());
        }

        ~AudioDevicesWatcher()
        {
            Dispose();
        }

        private void WatcherThread()
        {
            var retry = 0;
            while (_isWorking)
            {
                var devices = retry < 10 ? GetDevices() : new List<AudioDeviceDescriptor>();
                if (devices != null)
                {
                    retry = 0;

                    var removed = _devices.Where(device => devices.FirstOrDefault(x => x.Id == device.Id) == null).ToArray();
                    var added = devices.Where(device => _devices.FirstOrDefault(x => x.Id == device.Id) == null).ToArray();

                    if (removed.Any() || added.Any())
                    {
                        lock (_devices)
                        {
                            _devices.Clear();
                            _devices.AddRange(devices);
                        }

                        if (removed.Any()) _audio.HandleDevicesRemoved(removed);
                        if (added.Any()) _audio.HandleDevicesAdded(added);

                        Thread.Sleep(100);
                    }
                    else
                    {
                        Thread.Sleep(1000);
                    }
                }
                else
                {
                    retry++;
                    Thread.Sleep(100);
                }
            }
        }

        private List<AudioDeviceDescriptor> GetDevices()
        {
            try
            { 
                var result = new List<AudioDeviceDescriptor>();

                result.AddRange(SharpDX.DirectSound.DirectSound.GetDevices().Select(x => new AudioDeviceDescriptor { Id = x.DriverGuid, Name = x.Description }));
                result.AddRange(SharpDX.DirectSound.DirectSoundCapture.GetDevices().Select(x => new AudioDeviceDescriptor { Id = x.DriverGuid, Name = x.Description }));

                return result;
            }
            catch (IOException e)
            {
                Logger.LogWarn(e);
                return null;
            }
        }

        public void Dispose()
        {
            _isWorking = false;
            GC.SuppressFinalize(this);
        }
    }
}
