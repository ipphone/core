using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Management;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Threading;
using AudioLibrary.Interfaces;
using AudioLibrary.PjSIP.ManagedWatcher;
using Sipek;

namespace AudioLibrary.PjSIP
{
    public class Audio : IAudio
    {
        private float _speakerVolume = 1F;
        private float _micVolume = 1F;
        private float _lastMicVolume = 1F;

        private AudioDevice _playbackDevice = null;
        private AudioDevice _recordingDevice = null;
        private AudioDevicesWatcher _audioDevicesWatcher;

        public event Action<IEnumerable<IAudioDevice>> AudioDevicesAdded;
        public event Action<IEnumerable<IAudioDevice>> AudioDevicesRemoved;
        public event Action<IAudioDevice> PlaybackDeviceChanged;
        public event Action<IAudioDevice> RecordingDeviceChanged;

        internal List<AudioDevice> AudioDevices { get; private set; }

        public IAudioDevice DefaultPlaybackDevice { get; private set; }
        public IAudioDevice DefaultRecordingDevice { get; private set; }

        public IAudioDevice PlaybackDevice
        {
            get { return _playbackDevice; }
            set
            {
                try
                {
                    _playbackDevice = AudioDevices.FirstOrDefault(x => x.Name == value.Name);

                    if (PlaybackDeviceChanged != null)
                        PlaybackDeviceChanged.BeginInvoke(_playbackDevice, null, null);
                }
                catch
                {
                    _playbackDevice = null;
                }
            }
        }

        public IAudioDevice RecordingDevice
        {
            get { return _recordingDevice; }
            set
            {
                try
                {
                    _recordingDevice = AudioDevices.FirstOrDefault(x => x.Name == value.Name);

                    if (RecordingDeviceChanged != null)
                        RecordingDeviceChanged.BeginInvoke(_recordingDevice, null, null);
                }
                catch
                {
                    _recordingDevice = null;
                }
            }
        }
        public int SpeakerVolume
        {
            get { return (int)(_speakerVolume * 50); }
            set 
            { 
                _speakerVolume = value / 50F;

                CommonDelegates.SafeInvoke(() => Imports.dll_setSpeakerLevel(_speakerVolume));
            }
        }

        public int MicVolume
        {
            get { return (int)(_micVolume * 50); }
            set
            {
                _micVolume = value / 50F;

                CommonDelegates.SafeInvoke(() => Imports.dll_setMicLevel(_micVolume));
            }
        }

        public bool MicMute
        {
            get { return _micVolume < 0.01; }
            set
            {
                if (value)
                {
                    _lastMicVolume = _micVolume;
                    _micVolume = 0;
                }
                else _micVolume = _lastMicVolume;

                CommonDelegates.SafeInvoke(() => Imports.dll_setMicLevel(_micVolume));
            }
        }

        public Audio()
        {
            AudioDevices = new List<AudioDevice>();

            _audioDevicesWatcher = new AudioDevicesWatcher(this);

            ReloadAudioValues();
            ReloadAudioDevices();

            int speakersDevice = 0, micDevice = 0;

            Imports.dll_getSoundDevices(ref speakersDevice, ref micDevice);

            DefaultPlaybackDevice = AudioDevices.FirstOrDefault(x => x.Index == speakersDevice);
            DefaultRecordingDevice = AudioDevices.FirstOrDefault(x => x.Index == micDevice);
        }

        internal void HandleDevicesAdded(IEnumerable<AudioDevicesWatcher.AudioDeviceDescriptor> deviceDescriptors)
        {
            // Call sync
            CommonDelegates.SafeInvoke<int>(() => { ReloadAudioDevices(); return 0; });

            var devices = ConvertDeviceDescriptors(deviceDescriptors);

            if (devices != null && AudioDevicesAdded != null) AudioDevicesAdded(devices);
        }

        internal void HandleDevicesRemoved(IEnumerable<AudioDevicesWatcher.AudioDeviceDescriptor> deviceDescriptors)
        {
            var devices = ConvertDeviceDescriptors(deviceDescriptors);

            // Call sync
            CommonDelegates.SafeInvoke<int>(() => { ReloadAudioDevices(); return 0; });

            if (devices != null && AudioDevicesRemoved != null) AudioDevicesRemoved(devices);
        }

        private IEnumerable<AudioDevice> ConvertDeviceDescriptors(IEnumerable<AudioDevicesWatcher.AudioDeviceDescriptor> audioDeviceDescriptors)
        {
            return audioDeviceDescriptors.Select(desc => AudioDevices.FirstOrDefault(x => x.AudioDeviceDescriptor != null && string.Equals(desc.Id, x.AudioDeviceDescriptor.Id))).Where(x => x != null).ToArray();
        }

        private void ReloadAudioValues()
        {
            _speakerVolume = Imports.dll_getSpeakerLevel();
            _micVolume = Imports.dll_getMicLevel();
        }

        private void ReloadAudioDevices()
        {
            var audioDevices = new List<AudioDevice>();
            var cnt = (uint)Imports.dll_enumerateSoundDevicesCount();

            var sz = Marshal.SizeOf<Imports.PjAudioDeviceInfo>();
            var memPtr = Marshal.AllocCoTaskMem((int)(sz * cnt));
            try
            {
                var ptr = memPtr;
                cnt = (uint)Imports.dll_enumerateSoundDevices(ptr, cnt);
                var devices = new List<AudioDevice>();
                for (int i = 0; i < cnt; i++)
                {
                    var device = Marshal.PtrToStructure<Imports.PjAudioDeviceInfo>(ptr);
                    if (device.IsNull == 0 && !string.IsNullOrEmpty(device.Name) && i > 0) // 0-item is default mapper. Let's hide it from users eyes.
                    {
                        devices.Add(new AudioDevice(this, device, i));
                    }

                    ptr = IntPtr.Add(ptr, sz);
                }

                var deviceGroups = devices.Select(x => new { Device = x, Devices = devices.Where(y => y.Name.StartsWith(x.Name)) }).ToArray();
                var uniqueDevices = deviceGroups.Select(x => x.Devices.OrderByDescending(y => y.Name.Length).FirstOrDefault()).Where(x => x != null).ToArray();
                
                lock (_audioDevicesWatcher.Devices)
                {
                    foreach (var device in uniqueDevices)
                    {
                        device.AudioDeviceDescriptor = _audioDevicesWatcher.Devices.FirstOrDefault(x => x.Name.StartsWith(device.Name));
                        audioDevices.Add(device);
                    }
                }

                AudioDevices = audioDevices;
            }
            finally
            {
                Marshal.FreeCoTaskMem(memPtr);
            }
        }

        public void Dispose()
        {
            _audioDevicesWatcher.Dispose();
        }

        IEnumerable<IAudioDevice> IAudio.AudioDevices => AudioDevices;
    }
}
