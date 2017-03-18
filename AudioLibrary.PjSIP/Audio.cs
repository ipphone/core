using System;
using System.Collections.Generic;
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

            IntPtr ptr = IntPtr.Zero;
            try
            {
                var sz = Marshal.SizeOf(new Imports.PjAudioDeviceInfo());
                ptr = Marshal.AllocCoTaskMem((int) (sz*cnt));

                cnt = (uint)Imports.dll_enumerateSoundDevices(ptr, cnt);

                var devices = new Imports.PjAudioDeviceInfo[cnt];
                var longPtr = ptr.ToInt64();
                for (int i = 0; i < cnt; i++)
                {
                    var currentPtr = new IntPtr(longPtr);
                    devices[i] = (Imports.PjAudioDeviceInfo)Marshal.PtrToStructure(currentPtr, typeof (Imports.PjAudioDeviceInfo));

                    longPtr += sz;

                    if (devices[i].IsNull == 0 && i > 0) // 0-item is default mapper. Let's hide it from users eyes.
                    {
                        lock (_audioDevicesWatcher.Devices)
                        {
                            audioDevices.Add(new AudioDevice(this, devices[i], i)
                            {
                                AudioDeviceDescriptor = _audioDevicesWatcher.Devices.FirstOrDefault(x => x.Name.StartsWith(devices[i].Name))
                            });
                        }
                    }
                }

                AudioDevices = audioDevices;
            }
            finally
            {
                if (ptr != IntPtr.Zero) Marshal.FreeCoTaskMem(ptr);
            }
        }

        public void Dispose()
        {
            _audioDevicesWatcher.Dispose();
        }

        IEnumerable<IAudioDevice> IAudio.AudioDevices => AudioDevices;
    }
}
