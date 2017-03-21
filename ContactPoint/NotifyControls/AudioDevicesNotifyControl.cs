using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using ContactPoint.Common.Audio;
using ContactPoint.Common;
using ContactPoint.BaseDesign.BaseNotifyControls;

namespace ContactPoint.NotifyControls
{
    public partial class AudioDevicesNotifyControl : NotifyControl
    {
        private IEnumerable<IAudioDevice> _audioDevices = null;
        public IEnumerable<IAudioDevice> AudioDevices
        {
            get { return _audioDevices; }
            set 
            { 
                _audioDevices = value;

                labelDevicesList.Text = "";

                foreach (var device in _audioDevices)
                    labelDevicesList.Text += String.Format("{0} ({1})\r\n", device.Name, device.Type == AudioDeviceType.Playback ? ContactPoint.CaptionStrings.CaptionStrings.AudioDevicePlayback : ContactPoint.CaptionStrings.CaptionStrings.AudioDeviceRecording);
            }
        }

        private bool _isDevicesAdded;
        public bool IsDevicesAdded
        {
            get { return _isDevicesAdded; }
            set 
            { 
                _isDevicesAdded = value;

                labelAction.Text = _isDevicesAdded ? ContactPoint.CaptionStrings.CaptionStrings.DevicesAdded : ContactPoint.CaptionStrings.CaptionStrings.DevicesRemoved;
            }
        }

        public ICore Core { get; set; }

        public AudioDevicesNotifyControl()
            : base()
        {
            InitializeComponent();

            linkLabelClose.Text = CaptionStrings.CaptionStrings.Cancel;
        }

        protected virtual void LinkLabelClick(object sender, EventArgs e)
        {
            Close();
        }

        private void linkLabelUseDevices_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            LinkLabelClick(sender, e);
        }

        private void linkLabelClose_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Close();
        }
    }
}
