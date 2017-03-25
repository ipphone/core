using System;
using System.Collections.Generic;
using System.Text;
using ContactPoint.Forms;

namespace ContactPoint.NotifyControls
{
    public class AudioDevicesRemovedNotifyControl : AudioDevicesNotifyControl
    {
        public AudioDevicesRemovedNotifyControl()
            : base()
        {
            IsDevicesAdded = false;

            linkLabelUseDevices.Text = CaptionStrings.CaptionStrings.ShowAudioDevicesSettings;
        }

        protected override void LinkLabelClick(object sender, EventArgs e)
        {
            var settingsForm = new SettingsForm(Core);
            settingsForm.Show(SettingsFormPart.Audio);

            base.LinkLabelClick(sender, e);
        }
    }
}
