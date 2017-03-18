using AudioLibrary.WMME.Native;

namespace AudioLibrary.WMME
{
    internal class AudioDeviceDescriptor
    {
        public int DeviceId { get; set; }
        public string Name { get; set; }
        public bool PlaybackSupport { get; set; }
        public bool RecordingSupport { get; set; }
		public MIXERLINE_COMPONENTTYPE PlaybackComponentType { get; set; }
		public MIXERLINE_COMPONENTTYPE RecordingComponentType { get; set; }

        public AudioDevice CreateAudioDevice(Audio audio)
        {
            return new AudioDevice(audio, this);
        }
    }
}
