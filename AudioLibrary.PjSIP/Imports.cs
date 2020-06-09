using System;
using System.Runtime.InteropServices;

namespace AudioLibrary.PjSIP
{
    internal class Imports
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct PjAudioDeviceInfo
        {
            /** 
             * The device name 
             */
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
            public string Name;

            /** 
             * Maximum number of input channels supported by this device. If the
             * value is zero, the device does not support input operation (i.e.
             * it is a playback only device). 
             */
            public Int32 InputCount;

            /** 
             * Maximum number of output channels supported by this device. If the
             * value is zero, the device does not support output operation (i.e. 
             * it is an input only device).
             */
            public Int32 OutputCount;

            /** 
             * Default sampling rate.
             */
            public Int32 DefaultSamplesPerSec;

            /** 
             * The underlying driver name 
             */
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string Driver;

            /** 
             * Device capabilities, as bitmask combination of #pjmedia_aud_dev_cap.
             */
            public Int32 Caps;

            /** 
             * Supported audio device routes, as bitmask combination of 
             * #pjmedia_aud_dev_route. The value may be zero if the device
             * does not support audio routing.
             */
            public Int32 Routes;

            /** 
             * Number of audio formats supported by this device. The value may be
             * zero if the device does not support non-PCM format.
             */
            public Int32 ExtFmtCnt;

            /**
             * If it is bigger than 0 then object is not initialized
             */
            public Int32 IsNull;
        }

        [DllImport(Sipek.Native.PJSIP_DLL, EntryPoint = "dll_enumerateSoundDevicesCount", CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe int dll_enumerateSoundDevicesCount();

        [DllImport(Sipek.Native.PJSIP_DLL, EntryPoint = "dll_enumerateSoundDevices", CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe int dll_enumerateSoundDevices([In, Out] IntPtr audioDeviceInfos, UInt32 count);

        [DllImport(Sipek.Native.PJSIP_DLL, EntryPoint = "dll_getMicLevel", CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe float dll_getMicLevel();

        [DllImport(Sipek.Native.PJSIP_DLL, EntryPoint = "dll_setMicLevel", CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe void dll_setMicLevel(float value);

        [DllImport(Sipek.Native.PJSIP_DLL, EntryPoint = "dll_getSpeakerLevel", CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe float dll_getSpeakerLevel();

        [DllImport(Sipek.Native.PJSIP_DLL, EntryPoint = "dll_setSpeakerLevel", CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe void dll_setSpeakerLevel(float value);

        [DllImport(Sipek.Native.PJSIP_DLL, EntryPoint = "dll_getSoundDevices", CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe void dll_getSoundDevices(ref int playbackDevice, ref int recordingDevice);
    }
}
