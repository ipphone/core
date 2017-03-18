using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using AudioLibrary.WMME.Native;

namespace AudioLibrary.WMME
{
    internal class AudioForm : NativeWindow
    {
        private delegate void EmptyDelegate();
        private IntPtr _deviceEventHandle = IntPtr.Zero;
        private Audio _audio;

        public AudioForm(Audio audio)
        {
            _audio = audio;

            CreateHandle(new CreateParams());

            //MinimizeBox = false;
            //MaximizeBox = false;
            //ShowInTaskbar = false;
            //ShowIcon = false;
            //Visible = false;
            //FormBorderStyle = FormBorderStyle.None;

            //Activated += new EventHandler(AudioForm_Activated);

            //RegisterDeviceNotification();
        }

        void AudioForm_Activated(object sender, EventArgs e)
        {
            //Visible = false;

            //RegisterDeviceNotification();
        }

        ~AudioForm()
        {
            if (_deviceEventHandle != IntPtr.Zero)
                Win32.UnregisterDeviceNotification(_deviceEventHandle);
        }

        protected override void WndProc(ref Message m)
        {
            switch (m.Msg)
            {
                case MixerNative.MM_MIXM_LINE_CHANGE:
                    _audio.HandleLineChanged(m.WParam, (uint)m.LParam);
                    break;
                case MixerNative.MM_MIXM_CONTROL_CHANGE:
                    _audio.HandleLineControlChanged(m.WParam, (uint)m.LParam);
                    break;
                case Win32.WM_DEVICECHANGE:
                    Win32.DEV_BROADCAST_HDR structDecription;
                    switch ((int)m.WParam)
                    {
                        case Win32.DBT_DEVICEARRIVAL:
                            structDecription = (Win32.DEV_BROADCAST_HDR)m.GetLParam(typeof(Win32.DEV_BROADCAST_HDR));

                            if (structDecription.dbcc_devicetype == Win32.DBT_DEVTYP_DEVICEINTERFACE)
                            {
                                Win32.DEV_BROADCAST_DEVICEINTERFACE devInterface = (Win32.DEV_BROADCAST_DEVICEINTERFACE)m.GetLParam(typeof(Win32.DEV_BROADCAST_DEVICEINTERFACE));

                                if (devInterface.dbcc_classguid == Win32.MEDIA_CLASS_GUID)
                                    new EmptyDelegate(_audio.HandleDeviceAdding).BeginInvoke(null, null);
                            }
                            break;
                        case Win32.DBT_DEVICEREMOVECOMPLETE:
                            structDecription = (Win32.DEV_BROADCAST_HDR)m.GetLParam(typeof(Win32.DEV_BROADCAST_HDR));

                            if (structDecription.dbcc_devicetype == Win32.DBT_DEVTYP_DEVICEINTERFACE)
                            {
                                Win32.DEV_BROADCAST_DEVICEINTERFACE devInterface = (Win32.DEV_BROADCAST_DEVICEINTERFACE)m.GetLParam(typeof(Win32.DEV_BROADCAST_DEVICEINTERFACE));

                                if (devInterface.dbcc_classguid == Win32.MEDIA_CLASS_GUID)
                                    new EmptyDelegate(_audio.HandleDeviceRemoving).BeginInvoke(null, null);
                            }
                            break;
                    }
                    break;
            }

            base.WndProc(ref m);
        }

        private void RegisterDeviceNotification()
        {
            Win32.DEV_BROADCAST_DEVICEINTERFACE deviceInterface = new Win32.DEV_BROADCAST_DEVICEINTERFACE();

            var size = Marshal.SizeOf(deviceInterface);
            deviceInterface.dbcc_devicetype = Win32.DBT_DEVTYP_DEVICEINTERFACE;
            deviceInterface.dbcc_reserved = 0;
            deviceInterface.dbcc_classguid = Win32.MEDIA_CLASS_GUID;
            deviceInterface.dbcc_size = size;

            IntPtr buffer = Marshal.AllocHGlobal(size);
            Marshal.StructureToPtr(deviceInterface, buffer, true);

            _deviceEventHandle = Win32.RegisterDeviceNotification(Handle, buffer, Win32.DEVICE_NOTIFY_WINDOW_HANDLE);
            if (_deviceEventHandle == IntPtr.Zero)
                throw new InvalidOperationException("Failed to register for notifications about devices changes");
        }
    }
}
