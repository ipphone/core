using System;
using System.Collections.Generic;
using System.Text;

namespace ContactPoint.NotifyControls
{
    public class AudioDevicesAddedNotifyControl : AudioDevicesNotifyControl
    {
        public AudioDevicesAddedNotifyControl()
            : base()
        {
            IsDevicesAdded = true;

            linkLabelUseDevices.Text = CaptionStrings.CaptionStrings.UseDevice;
        }

        protected override void LinkLabelClick(object sender, EventArgs e)
        {
            foreach (var device in AudioDevices)
                if (device.Type == Common.Audio.AudioDeviceType.Playback) Core.Audio.PlaybackDevice = device;
                else Core.Audio.RecordingDevice = device;

            base.LinkLabelClick(sender, e);
        }
    }
}
