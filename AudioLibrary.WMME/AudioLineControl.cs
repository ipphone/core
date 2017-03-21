using System;
using System.Runtime.InteropServices;
using AudioLibrary.Interfaces;
using AudioLibrary.WMME.Native;

namespace AudioLibrary.WMME
{
    internal abstract class AudioLineControl
    {
        private Action<AudioLine> _raiseEventDelegate = null;

        public AudioLine AudioLine { get; private set; }
        public string Name { get; private set; }
        public uint Id { get; private set; }
        public MIXERCONTROL_CONTROLTYPE ControlType { get; private set; }
        public MIXERCONTROL_CONTROLFLAG ControlFlag { get; private set; }
        public uint MultipleItems { get; private set; }
        public uint Minimum { get; private set; }
        public uint Maximum { get; private set; }
        public uint Steps { get; private set; }

        public abstract unsafe object Value { get; set; }

        protected unsafe int ValueAsSigned
        {
            get
            {
                MMErrors errorCode = 0;
                IntPtr pUnsigned = IntPtr.Zero;

                try
                {
                    uint cChannels = AudioLine.Channels;
                    if ((((uint)MIXERCONTROL_CONTROLFLAG.UNIFORM) & (uint)ControlFlag) != 0)
                        cChannels = 1;

                    pUnsigned = Marshal.AllocHGlobal((int)(cChannels * sizeof(MIXERCONTROLDETAILS_SIGNED)));

                    if (IntPtr.Size == 4)
                    {
                        MIXERCONTROLDETAILS mxcd = new MIXERCONTROLDETAILS();
                        mxcd.cbStruct = (uint)sizeof(MIXERCONTROLDETAILS);
                        mxcd.dwControlID = Id;
                        mxcd.cChannels = cChannels;
                        mxcd.hwndOwner = IntPtr.Zero;
                        mxcd.cbDetails = (uint)sizeof(MIXERCONTROLDETAILS_SIGNED);
                        mxcd.paDetails = pUnsigned;

                        errorCode = (MMErrors)MixerNative.mixerGetControlDetails(AudioLine.AudioDeviceInternal.Handle, ref mxcd, MIXER_SETCONTROLDETAILSFLAG.VALUE);
                    }
                    else
                    {
                        MIXERCONTROLDETAILS64 mxcd = new MIXERCONTROLDETAILS64();
                        mxcd.cbStruct = (uint)sizeof(MIXERCONTROLDETAILS64);
                        mxcd.dwControlID = Id;
                        mxcd.cChannels = cChannels;
                        mxcd.hwndOwner = IntPtr.Zero;
                        mxcd.cbDetails = (uint)sizeof(MIXERCONTROLDETAILS_SIGNED);
                        mxcd.paDetails = pUnsigned;

                        errorCode = (MMErrors)MixerNative.mixerGetControlDetails(AudioLine.AudioDeviceInternal.Handle, ref mxcd, MIXER_SETCONTROLDETAILSFLAG.VALUE);
                    }
                    if (errorCode != MMErrors.MMSYSERR_NOERROR)
                        throw new MixerException(errorCode, Audio.GetErrorDescription(FuncName.fnMixerGetControlDetails, errorCode));

                    MIXERCONTROLDETAILS_SIGNED mixerControlDetail;
                    if (AudioLine.Channel == Channel.Uniform)
                        mixerControlDetail = (MIXERCONTROLDETAILS_SIGNED)Marshal.PtrToStructure(pUnsigned, typeof(MIXERCONTROLDETAILS_SIGNED));
                    else
                    {
                        if (((int)AudioLine.Channel) > cChannels)
                            return -1;
                        mixerControlDetail = (MIXERCONTROLDETAILS_SIGNED)Marshal.PtrToStructure((IntPtr)((int)pUnsigned + (sizeof(MIXERCONTROLDETAILS_SIGNED) * ((int)AudioLine.Channel - 1))), typeof(MIXERCONTROLDETAILS_SIGNED));
                    }

                    //AudioLine.mVolumeMin = mxcd.cbStruct->
                    return (int)mixerControlDetail.value;
                }
                finally
                {
                    if (pUnsigned != IntPtr.Zero)
                        Marshal.FreeHGlobal(pUnsigned);
                }
            }
            set
            {
                MMErrors errorCode = 0;
                IntPtr pUnsigned = IntPtr.Zero;

                try
                {

                    uint cChannels = AudioLine.Channels;
                    if ((((uint)MIXERCONTROL_CONTROLFLAG.UNIFORM) & (uint)ControlFlag) != 0)
                        cChannels = 1;

                    pUnsigned = Marshal.AllocHGlobal((int)(cChannels * sizeof(MIXERCONTROLDETAILS_SIGNED)));

                    if (IntPtr.Size == 4)
                    {
                        MIXERCONTROLDETAILS mxcd = new MIXERCONTROLDETAILS();
                        mxcd.cbStruct = (uint)sizeof(MIXERCONTROLDETAILS);
                        mxcd.dwControlID = Id;
                        mxcd.cChannels = cChannels;
                        mxcd.hwndOwner = IntPtr.Zero;
                        mxcd.cbDetails = (uint)sizeof(MIXERCONTROLDETAILS_SIGNED);
                        mxcd.paDetails = pUnsigned;

                        errorCode = (MMErrors)MixerNative.mixerGetControlDetails(AudioLine.AudioDeviceInternal.Handle, ref mxcd, MIXER_SETCONTROLDETAILSFLAG.VALUE);
                        if (errorCode != MMErrors.MMSYSERR_NOERROR)
                            throw new MixerException(errorCode, Audio.GetErrorDescription(FuncName.fnMixerGetControlDetails, errorCode));

                        MIXERCONTROLDETAILS_SIGNED mixerControlDetail = (MIXERCONTROLDETAILS_SIGNED)Marshal.PtrToStructure(pUnsigned, typeof(MIXERCONTROLDETAILS_SIGNED));

                        // Set the volume to the middle  (for both channels as needed) 
                        for (int i = 0; i < cChannels; i++)
                        {
                            if (AudioLine.Channel == Channel.Uniform || ((int)AudioLine.Channel - 1) == i)
                            {
                                uint* vol = ((uint*)pUnsigned) + i;
                                *(vol) = (uint)value;
                            }
                        }

                        errorCode = (MMErrors)MixerNative.mixerSetControlDetails(AudioLine.AudioDeviceInternal.Handle, ref mxcd, MIXER_SETCONTROLDETAILSFLAG.VALUE);
                    }
                    else
                    {
                        MIXERCONTROLDETAILS64 mxcd = new MIXERCONTROLDETAILS64();
                        mxcd.cbStruct = (uint)sizeof(MIXERCONTROLDETAILS64);
                        mxcd.dwControlID = Id;
                        mxcd.cChannels = cChannels;
                        mxcd.hwndOwner = IntPtr.Zero;
                        mxcd.cbDetails = (uint)sizeof(MIXERCONTROLDETAILS_SIGNED);
                        mxcd.paDetails = pUnsigned;

                        errorCode = (MMErrors)MixerNative.mixerGetControlDetails(AudioLine.AudioDeviceInternal.Handle, ref mxcd, MIXER_SETCONTROLDETAILSFLAG.VALUE);
                        if (errorCode != MMErrors.MMSYSERR_NOERROR)
                            throw new MixerException(errorCode, Audio.GetErrorDescription(FuncName.fnMixerGetControlDetails, errorCode));

                        MIXERCONTROLDETAILS_SIGNED mixerControlDetail = (MIXERCONTROLDETAILS_SIGNED)Marshal.PtrToStructure(pUnsigned, typeof(MIXERCONTROLDETAILS_SIGNED));

                        // Set the volume to the middle  (for both channels as needed) 
                        for (int i = 0; i < cChannels; i++)
                        {
                            if (AudioLine.Channel == Channel.Uniform || ((int)AudioLine.Channel - 1) == i)
                            {
                                uint* vol = ((uint*)pUnsigned) + i;
                                *(vol) = (uint)value;
                            }
                        }

                        errorCode = (MMErrors)MixerNative.mixerSetControlDetails(AudioLine.AudioDeviceInternal.Handle, ref mxcd, MIXER_SETCONTROLDETAILSFLAG.VALUE);
                    }
                    if (errorCode != MMErrors.MMSYSERR_NOERROR)
                        throw new MixerException(errorCode, Audio.GetErrorDescription(FuncName.fnMixerSetControlDetails, errorCode));
                }
                finally
                {
                    if (pUnsigned != IntPtr.Zero)
                        Marshal.FreeHGlobal(pUnsigned);
                }
            }
        }

        protected unsafe int ValueAsUnsigned
        {
            get
            {
#if DEBUG
                DebugTools.Log("--> AudioLineControl.ValueAsUnsigned");
                DebugTools.Log(String.Format("IntPtr size: {0}", IntPtr.Size));
                DebugTools.Log(String.Format("Device: {0}", AudioLine.AudioDeviceInternal.Name));
                DebugTools.Log(String.Format("AudioLine: {0}", AudioLine.Name));
                DebugTools.Log(String.Format("AudioLineControl: {0}", Name));
#endif
                MMErrors errorCode = 0;
                IntPtr pUnsigned = IntPtr.Zero;

                try
                {
                    uint cChannels = AudioLine.Channels;
                    if ((((uint)MIXERCONTROL_CONTROLFLAG.UNIFORM) & (uint)ControlFlag) != 0)
                        cChannels = 1;

#if DEBUG
                    DebugTools.Log(String.Format("Channels: {0}", cChannels));
#endif

                    pUnsigned = Marshal.AllocHGlobal((int)(cChannels * sizeof(MIXERCONTROLDETAILS_UNSIGNED)));

                    if (IntPtr.Size == 4)
                    {
                        MIXERCONTROLDETAILS mxcd = new MIXERCONTROLDETAILS();
                        mxcd.cbStruct = (uint)sizeof(MIXERCONTROLDETAILS);
                        mxcd.dwControlID = Id;
                        mxcd.cChannels = cChannels;
                        mxcd.hwndOwner = IntPtr.Zero;
                        mxcd.cMultipleItems = MultipleItems;
                        mxcd.cbDetails = (uint)sizeof(MIXERCONTROLDETAILS_UNSIGNED);
                        mxcd.paDetails = pUnsigned;

#if DEBUG
                        DebugTools.Log(String.Format("Getting mixer details..."));
#endif
                        errorCode = (MMErrors)MixerNative.mixerGetControlDetails(AudioLine.AudioDeviceInternal.Handle, ref mxcd, MIXER_SETCONTROLDETAILSFLAG.VALUE);
                    }
                    else
                    {
                        MIXERCONTROLDETAILS64 mxcd = new MIXERCONTROLDETAILS64();
                        mxcd.cbStruct = (uint)sizeof(MIXERCONTROLDETAILS64);
                        mxcd.dwControlID = Id;
                        mxcd.cChannels = cChannels;
                        mxcd.hwndOwner = IntPtr.Zero;
                        mxcd.cMultipleItems = MultipleItems;
                        mxcd.cbDetails = (uint)sizeof(MIXERCONTROLDETAILS_UNSIGNED);
                        mxcd.paDetails = pUnsigned;

#if DEBUG
                        DebugTools.Log(String.Format("Getting mixer details..."));
#endif
                        errorCode = (MMErrors)MixerNative.mixerGetControlDetails(AudioLine.AudioDeviceInternal.Handle, ref mxcd, MIXER_SETCONTROLDETAILSFLAG.VALUE);
                    }

#if DEBUG
                    DebugTools.Log(String.Format("Getting mixer details done! Error code: {0}", errorCode));
#endif
                    if (errorCode != MMErrors.MMSYSERR_NOERROR)
                        throw new MixerException(errorCode, Audio.GetErrorDescription(FuncName.fnMixerGetControlDetails, errorCode));

#if DEBUG
                    DebugTools.Log(String.Format("Converting values from unmanaged structures."));
#endif
                    MIXERCONTROLDETAILS_UNSIGNED mixerControlDetail;
                    if (AudioLine.Channel == Channel.Uniform)
                        mixerControlDetail = (MIXERCONTROLDETAILS_UNSIGNED)Marshal.PtrToStructure(pUnsigned, typeof(MIXERCONTROLDETAILS_UNSIGNED));
                    else
                    {
                        if (((int)AudioLine.Channel) > cChannels)
                            return -1;
                        mixerControlDetail = (MIXERCONTROLDETAILS_UNSIGNED)Marshal.PtrToStructure((IntPtr)((int)pUnsigned + (sizeof(MIXERCONTROLDETAILS_UNSIGNED) * ((int)AudioLine.Channel - 1))), typeof(MIXERCONTROLDETAILS_UNSIGNED));
                    }

#if DEBUG
                    DebugTools.Log(String.Format("<-- AudioLineControl.ValueAsUnsigned. Result: {0}", mixerControlDetail.dwValue));
#endif

                    //AudioLine.mVolumeMin = mxcd.cbStruct->
                    return (int)mixerControlDetail.dwValue;
                }
                finally
                {
                    if (pUnsigned != IntPtr.Zero)
                        Marshal.FreeHGlobal(pUnsigned);
                }
            }
            set
            {
                MMErrors errorCode = 0;
                IntPtr pUnsigned = IntPtr.Zero;

                try
                {

                    uint cChannels = AudioLine.Channels;
                    if ((((uint)MIXERCONTROL_CONTROLFLAG.UNIFORM) & (uint)ControlFlag) != 0)
                        cChannels = 1;

                    pUnsigned = Marshal.AllocHGlobal((int)(cChannels * sizeof(MIXERCONTROLDETAILS_UNSIGNED)));

                    if (IntPtr.Size == 4)
                    {
                        MIXERCONTROLDETAILS mxcd = new MIXERCONTROLDETAILS();
                        mxcd.cbStruct = (uint)sizeof(MIXERCONTROLDETAILS);
                        mxcd.dwControlID = Id;
                        mxcd.cChannels = cChannels;
                        mxcd.hwndOwner = IntPtr.Zero;
                        mxcd.cbDetails = (uint)sizeof(MIXERCONTROLDETAILS_UNSIGNED);
                        mxcd.paDetails = pUnsigned;

                        errorCode = (MMErrors)MixerNative.mixerGetControlDetails(AudioLine.AudioDeviceInternal.Handle, ref mxcd, MIXER_SETCONTROLDETAILSFLAG.VALUE);
                        if (errorCode != MMErrors.MMSYSERR_NOERROR)
                            throw new MixerException(errorCode, Audio.GetErrorDescription(FuncName.fnMixerGetControlDetails, errorCode));

                        MIXERCONTROLDETAILS_UNSIGNED mixerControlDetail = (MIXERCONTROLDETAILS_UNSIGNED)Marshal.PtrToStructure(pUnsigned, typeof(MIXERCONTROLDETAILS_UNSIGNED));

                        // Set the volume to the middle  (for both channels as needed) 
                        for (int i = 0; i < cChannels; i++)
                        {
                            if (AudioLine.Channel == Channel.Uniform || ((int)AudioLine.Channel - 1) == i)
                            {
                                uint* vol = ((uint*)pUnsigned) + i;
                                *(vol) = (uint)value;
                            }
                        }

                        errorCode = (MMErrors)MixerNative.mixerSetControlDetails(AudioLine.AudioDeviceInternal.Handle, ref mxcd, MIXER_SETCONTROLDETAILSFLAG.VALUE);
                    }
                    else
                    {
                        MIXERCONTROLDETAILS64 mxcd = new MIXERCONTROLDETAILS64();
                        mxcd.cbStruct = (uint)sizeof(MIXERCONTROLDETAILS64);
                        mxcd.dwControlID = Id;
                        mxcd.cChannels = cChannels;
                        mxcd.hwndOwner = IntPtr.Zero;
                        mxcd.cbDetails = (uint)sizeof(MIXERCONTROLDETAILS_UNSIGNED);
                        mxcd.paDetails = pUnsigned;

                        errorCode = (MMErrors)MixerNative.mixerGetControlDetails(AudioLine.AudioDeviceInternal.Handle, ref mxcd, MIXER_SETCONTROLDETAILSFLAG.VALUE);
                        if (errorCode != MMErrors.MMSYSERR_NOERROR)
                            throw new MixerException(errorCode, Audio.GetErrorDescription(FuncName.fnMixerGetControlDetails, errorCode));

                        MIXERCONTROLDETAILS_UNSIGNED mixerControlDetail = (MIXERCONTROLDETAILS_UNSIGNED)Marshal.PtrToStructure(pUnsigned, typeof(MIXERCONTROLDETAILS_UNSIGNED));

                        // Set the volume to the middle  (for both channels as needed) 
                        for (int i = 0; i < cChannels; i++)
                        {
                            if (AudioLine.Channel == Channel.Uniform || ((int)AudioLine.Channel - 1) == i)
                            {
                                uint* vol = ((uint*)pUnsigned) + i;
                                *(vol) = (uint)value;
                            }
                        }

                        errorCode = (MMErrors)MixerNative.mixerSetControlDetails(AudioLine.AudioDeviceInternal.Handle, ref mxcd, MIXER_SETCONTROLDETAILSFLAG.VALUE);
                    }
                    if (errorCode != MMErrors.MMSYSERR_NOERROR)
                        throw new MixerException(errorCode, Audio.GetErrorDescription(FuncName.fnMixerSetControlDetails, errorCode));
                }
                finally
                {
                    if (pUnsigned != IntPtr.Zero)
                        Marshal.FreeHGlobal(pUnsigned);
                }
            }
        }

        protected unsafe bool ValueAsBoolean
        {
            get
            {
                MMErrors errorCode = 0;
                IntPtr pUnsigned = IntPtr.Zero;

                try
                {
                    IntPtr pmxcdSelectValue = Marshal.AllocHGlobal((int)(1 * sizeof(MIXERCONTROLDETAILS_BOOLEAN)));

                    if (IntPtr.Size == 4)
                    {
                        MIXERCONTROLDETAILS mxcd = new MIXERCONTROLDETAILS();
                        mxcd.cbStruct = (uint)sizeof(MIXERCONTROLDETAILS);
                        mxcd.dwControlID = Id;
                        mxcd.cChannels = 1;
                        mxcd.hwndOwner = IntPtr.Zero;
                        mxcd.cbDetails = (uint)sizeof(MIXERCONTROLDETAILS_BOOLEAN);
                        mxcd.paDetails = pmxcdSelectValue;

                        unchecked
                        {
                            errorCode = (MMErrors)MixerNative.mixerGetControlDetails(AudioLine.AudioDeviceInternal.Handle, ref mxcd, (MIXER_GETCONTROLDETAILSFLAG)(int)((uint)MIXER_OBJECTFLAG.HMIXER | (int)MIXER_GETCONTROLDETAILSFLAG.VALUE));
                        }
                    }
                    else
                    {
                        MIXERCONTROLDETAILS64 mxcd = new MIXERCONTROLDETAILS64();
                        mxcd.cbStruct = (uint)sizeof(MIXERCONTROLDETAILS64);
                        mxcd.dwControlID = Id;
                        mxcd.cChannels = 1;
                        mxcd.hwndOwner = IntPtr.Zero;
                        mxcd.cbDetails = (uint)sizeof(MIXERCONTROLDETAILS_BOOLEAN);
                        mxcd.paDetails = pmxcdSelectValue;

                        unchecked
                        {
                            errorCode = (MMErrors)MixerNative.mixerGetControlDetails(AudioLine.AudioDeviceInternal.Handle, ref mxcd, (MIXER_GETCONTROLDETAILSFLAG)(int)((uint)MIXER_OBJECTFLAG.HMIXER | (int)MIXER_GETCONTROLDETAILSFLAG.VALUE));
                        }
                    }
                    if (errorCode != MMErrors.MMSYSERR_NOERROR)
                        throw new MixerException(errorCode, Audio.GetErrorDescription(FuncName.fnMixerGetControlDetails, errorCode));

                    uint val = *((uint*)pmxcdSelectValue);
                    return val == 1 ? true : false;
                }
                finally
                {
                    if (pUnsigned != IntPtr.Zero)
                        Marshal.FreeHGlobal(pUnsigned);
                }
            }
            set
            {
                MMErrors errorCode = 0;
                IntPtr pUnsigned = IntPtr.Zero;

                try
                {
                    IntPtr pmxcdSelectValue = Marshal.AllocHGlobal((int)(1 * sizeof(MIXERCONTROLDETAILS_BOOLEAN)));

                    if (IntPtr.Size == 4)
                    {
                        MIXERCONTROLDETAILS mxcd = new MIXERCONTROLDETAILS();
                        mxcd.cbStruct = (uint)sizeof(MIXERCONTROLDETAILS);
                        mxcd.dwControlID = Id;
                        mxcd.cChannels = 1;
                        mxcd.hwndOwner = IntPtr.Zero;
                        mxcd.cbDetails = (uint)sizeof(MIXERCONTROLDETAILS_BOOLEAN);
                        mxcd.paDetails = pmxcdSelectValue;

                        unchecked
                        {
                            errorCode = (MMErrors)MixerNative.mixerGetControlDetails(AudioLine.AudioDeviceInternal.Handle, ref mxcd, (MIXER_GETCONTROLDETAILSFLAG)(int)((uint)MIXER_OBJECTFLAG.HMIXER | (int)MIXER_GETCONTROLDETAILSFLAG.VALUE));
                        }
                        if (errorCode != MMErrors.MMSYSERR_NOERROR)
                            throw new MixerException(errorCode, Audio.GetErrorDescription(FuncName.fnMixerGetControlDetails, errorCode));

                        *((uint*)pmxcdSelectValue) = value ? 1U : 0U;

                        errorCode = (MMErrors)MixerNative.mixerSetControlDetails(AudioLine.AudioDeviceInternal.Handle, ref mxcd, MIXER_SETCONTROLDETAILSFLAG.VALUE);
                    }
                    else
                    {
                        MIXERCONTROLDETAILS64 mxcd = new MIXERCONTROLDETAILS64();
                        mxcd.cbStruct = (uint)sizeof(MIXERCONTROLDETAILS64);
                        mxcd.dwControlID = Id;
                        mxcd.cChannels = 1;
                        mxcd.hwndOwner = IntPtr.Zero;
                        mxcd.cbDetails = (uint)sizeof(MIXERCONTROLDETAILS_BOOLEAN);
                        mxcd.paDetails = pmxcdSelectValue;

                        unchecked
                        {
                            errorCode = (MMErrors)MixerNative.mixerGetControlDetails(AudioLine.AudioDeviceInternal.Handle, ref mxcd, (MIXER_GETCONTROLDETAILSFLAG)(int)((uint)MIXER_OBJECTFLAG.HMIXER | (int)MIXER_GETCONTROLDETAILSFLAG.VALUE));
                        }
                        if (errorCode != MMErrors.MMSYSERR_NOERROR)
                            throw new MixerException(errorCode, Audio.GetErrorDescription(FuncName.fnMixerGetControlDetails, errorCode));

                        *((uint*)pmxcdSelectValue) = value ? 1U : 0U;

                        errorCode = (MMErrors)MixerNative.mixerSetControlDetails(AudioLine.AudioDeviceInternal.Handle, ref mxcd, MIXER_SETCONTROLDETAILSFLAG.VALUE);
                    }
                    if (errorCode != MMErrors.MMSYSERR_NOERROR)
                        throw new MixerException(errorCode, Audio.GetErrorDescription(FuncName.fnMixerSetControlDetails, errorCode));
                }
                finally
                {
                    if (pUnsigned != IntPtr.Zero)
                        Marshal.FreeHGlobal(pUnsigned);
                }
            }
        }

        internal unsafe uint Selected
        {
            get
            {
                IntPtr pmixList = IntPtr.Zero;
                IntPtr pmixBool = IntPtr.Zero;
                MMErrors errorCode = 0;

                try
                {
                    if (MultipleItems == 1 && AudioLine.AudioDeviceInternal.LinesInternal.Count > 0)
                        return AudioLine.AudioDeviceInternal.LinesInternal[0].Id;

                    if (IntPtr.Size == 4)
                    {
                        MIXERCONTROLDETAILS mxcdl = new MIXERCONTROLDETAILS();
                        pmixList = Marshal.AllocHGlobal((int)(MultipleItems * Marshal.SizeOf(typeof(MIXERCONTROLDETAILS_LISTTEXT))));

                        mxcdl.cbStruct = (uint)sizeof(MIXERCONTROLDETAILS);
                        mxcdl.dwControlID = Id;
                        mxcdl.cChannels = 1;
                        mxcdl.cMultipleItems = MultipleItems;
                        mxcdl.cbDetails = (uint)Marshal.SizeOf(typeof(MIXERCONTROLDETAILS_LISTTEXT));
                        mxcdl.paDetails = pmixList;
                        errorCode = (MMErrors)MixerNative.mixerGetControlDetails(AudioLine.AudioDeviceInternal.Handle, ref mxcdl, MIXER_GETCONTROLDETAILSFLAG.LISTTEXT);
                    }
                    else
                    {
                        MIXERCONTROLDETAILS64 mxcdl = new MIXERCONTROLDETAILS64();
                        pmixList = Marshal.AllocHGlobal((int)(MultipleItems * Marshal.SizeOf(typeof(MIXERCONTROLDETAILS_LISTTEXT))));

                        mxcdl.cbStruct = (uint)sizeof(MIXERCONTROLDETAILS64);
                        mxcdl.dwControlID = Id;
                        mxcdl.cChannels = 1;
                        mxcdl.cMultipleItems = MultipleItems;
                        mxcdl.cbDetails = (uint)Marshal.SizeOf(typeof(MIXERCONTROLDETAILS_LISTTEXT));
                        mxcdl.paDetails = pmixList;
                        errorCode = (MMErrors)MixerNative.mixerGetControlDetails(AudioLine.AudioDeviceInternal.Handle, ref mxcdl, MIXER_GETCONTROLDETAILSFLAG.LISTTEXT);
                    }
                    if (errorCode != MMErrors.MMSYSERR_NOERROR)
                        throw new MixerException(errorCode, Audio.GetErrorDescription(FuncName.fnMixerGetControlDetails, errorCode));

                    if (IntPtr.Size == 4)
                    {
                        MIXERCONTROLDETAILS mxcdb = new MIXERCONTROLDETAILS();
                        pmixBool = Marshal.AllocHGlobal((int)(MultipleItems * Marshal.SizeOf(typeof(MIXERCONTROLDETAILS_BOOLEAN))));

                        mxcdb.cbStruct = (uint)sizeof(MIXERCONTROLDETAILS);
                        mxcdb.dwControlID = Id;
                        mxcdb.cChannels = 1;
                        mxcdb.cMultipleItems = MultipleItems;
                        mxcdb.cbDetails = (uint)Marshal.SizeOf(typeof(MIXERCONTROLDETAILS_BOOLEAN));
                        mxcdb.paDetails = pmixBool;
                        errorCode = (MMErrors)MixerNative.mixerGetControlDetails(AudioLine.AudioDeviceInternal.Handle, ref mxcdb, ((uint)MIXER_OBJECTFLAG.HMIXER | (uint)MIXER_GETCONTROLDETAILSFLAG.VALUE));

                        if (errorCode != MMErrors.MMSYSERR_NOERROR)
                            throw new MixerException(errorCode, Audio.GetErrorDescription(FuncName.fnMixerGetControlDetails, errorCode));

                        for (uint y = 0; y < mxcdb.cMultipleItems; y++)
                        {
                            IntPtr pVmixList = (IntPtr)(((byte*)pmixList) + (Marshal.SizeOf(typeof(MIXERCONTROLDETAILS_LISTTEXT)) * y));
                            MIXERCONTROLDETAILS_LISTTEXT mixList = (MIXERCONTROLDETAILS_LISTTEXT)Marshal.PtrToStructure(pVmixList, typeof(MIXERCONTROLDETAILS_LISTTEXT));

                            uint lineId = mixList.dwParam1;
                            bool selected = (*(((uint*)pmixBool) + y)) == 1 ? true : false;
                            if (selected == true)
                                return lineId;
                        }
                    }
                    else
                    {
                        MIXERCONTROLDETAILS64 mxcdb = new MIXERCONTROLDETAILS64();
                        pmixBool = Marshal.AllocHGlobal((int)(MultipleItems * Marshal.SizeOf(typeof(MIXERCONTROLDETAILS_BOOLEAN))));

                        mxcdb.cbStruct = (uint)sizeof(MIXERCONTROLDETAILS64);
                        mxcdb.dwControlID = Id;
                        mxcdb.cChannels = 1;
                        mxcdb.cMultipleItems = MultipleItems;
                        mxcdb.cbDetails = (uint)Marshal.SizeOf(typeof(MIXERCONTROLDETAILS_BOOLEAN));
                        mxcdb.paDetails = pmixBool;
                        errorCode = (MMErrors)MixerNative.mixerGetControlDetails(AudioLine.AudioDeviceInternal.Handle, ref mxcdb, ((uint)MIXER_OBJECTFLAG.HMIXER | (uint)MIXER_GETCONTROLDETAILSFLAG.VALUE));

                        if (errorCode != MMErrors.MMSYSERR_NOERROR)
                            throw new MixerException(errorCode, Audio.GetErrorDescription(FuncName.fnMixerGetControlDetails, errorCode));

                        for (uint y = 0; y < mxcdb.cMultipleItems; y++)
                        {
                            IntPtr pVmixList = (IntPtr)(((byte*)pmixList) + (Marshal.SizeOf(typeof(MIXERCONTROLDETAILS_LISTTEXT)) * y));
                            MIXERCONTROLDETAILS_LISTTEXT mixList = (MIXERCONTROLDETAILS_LISTTEXT)Marshal.PtrToStructure(pVmixList, typeof(MIXERCONTROLDETAILS_LISTTEXT));

                            uint lineId = mixList.dwParam1;
                            bool selected = (*(((uint*)pmixBool) + y)) == 1 ? true : false;
                            if (selected == true)
                                return lineId;
                        }
                    }
                }
                finally
                {
                    if (pmixList != IntPtr.Zero)
                        Marshal.FreeHGlobal(pmixList);
                    if (pmixBool != IntPtr.Zero)
                        Marshal.FreeHGlobal(pmixBool);
                }

                return 0;
            }
            set
            {
                IntPtr pmixList = IntPtr.Zero;
                IntPtr pmixBool = IntPtr.Zero;
                MMErrors errorCode = 0;

                try
                {
                    uint minusOne;
                    unchecked { minusOne = (uint)-1; }

                    if ((MultipleItems == 1 && AudioLine.AudioDeviceInternal.LinesInternal.Count >= 1) || value == minusOne)
                    {
                        MixerNative.SendMessage(AudioLine.AudioDeviceInternal.AudioInternal.CallbackForm.Handle, MixerNative.MM_MIXM_LINE_CHANGE, (uint)AudioLine.AudioDeviceInternal.Handle, AudioLine.AudioDeviceInternal.LinesInternal[0].Id);
                        return;
                    }

                    if (IntPtr.Size == 4)
                    {
                        MIXERCONTROLDETAILS mxcdl = new MIXERCONTROLDETAILS();
                        pmixList = Marshal.AllocHGlobal((int)(MultipleItems * Marshal.SizeOf(typeof(MIXERCONTROLDETAILS_LISTTEXT))));

                        mxcdl.cbStruct = (uint)sizeof(MIXERCONTROLDETAILS);
                        mxcdl.dwControlID = Id;
                        mxcdl.cChannels = 1;
                        mxcdl.cMultipleItems = MultipleItems;
                        mxcdl.cbDetails = (uint)Marshal.SizeOf(typeof(MIXERCONTROLDETAILS_LISTTEXT));
                        mxcdl.paDetails = pmixList;
                        errorCode = (MMErrors)MixerNative.mixerGetControlDetails(AudioLine.AudioDeviceInternal.Handle, ref mxcdl, MIXER_GETCONTROLDETAILSFLAG.LISTTEXT);
                    }
                    else
                    {
                        MIXERCONTROLDETAILS64 mxcdl = new MIXERCONTROLDETAILS64();
                        pmixList = Marshal.AllocHGlobal((int)(MultipleItems * Marshal.SizeOf(typeof(MIXERCONTROLDETAILS_LISTTEXT))));

                        mxcdl.cbStruct = (uint)sizeof(MIXERCONTROLDETAILS64);
                        mxcdl.dwControlID = Id;
                        mxcdl.cChannels = 1;
                        mxcdl.cMultipleItems = MultipleItems;
                        mxcdl.cbDetails = (uint)Marshal.SizeOf(typeof(MIXERCONTROLDETAILS_LISTTEXT));
                        mxcdl.paDetails = pmixList;
                        errorCode = (MMErrors)MixerNative.mixerGetControlDetails(AudioLine.AudioDeviceInternal.Handle, ref mxcdl, MIXER_GETCONTROLDETAILSFLAG.LISTTEXT);
                    }
                    if (errorCode != MMErrors.MMSYSERR_NOERROR)
                        throw new MixerException(errorCode, Audio.GetErrorDescription(FuncName.fnMixerGetControlDetails, errorCode));

                    if (IntPtr.Size == 4)
                    {
                        MIXERCONTROLDETAILS mxcdb = new MIXERCONTROLDETAILS();
                        pmixBool = Marshal.AllocHGlobal((int)(MultipleItems * Marshal.SizeOf(typeof(MIXERCONTROLDETAILS_BOOLEAN))));

                        mxcdb.cbStruct = (uint)sizeof(MIXERCONTROLDETAILS);
                        mxcdb.dwControlID = Id;
                        mxcdb.cChannels = 1;
                        mxcdb.cMultipleItems = MultipleItems;
                        mxcdb.cbDetails = (uint)Marshal.SizeOf(typeof(MIXERCONTROLDETAILS_BOOLEAN));
                        mxcdb.paDetails = pmixBool;
                        errorCode = (MMErrors)MixerNative.mixerGetControlDetails(AudioLine.AudioDeviceInternal.Handle, ref mxcdb, ((uint)MIXER_OBJECTFLAG.HMIXER | (uint)MIXER_GETCONTROLDETAILSFLAG.VALUE));

                        if (errorCode != MMErrors.MMSYSERR_NOERROR)
                            throw new MixerException(errorCode, Audio.GetErrorDescription(FuncName.fnMixerGetControlDetails, errorCode));

                        for (uint y = 0; y < mxcdb.cMultipleItems; y++)
                        {
                            IntPtr pVmixList = (IntPtr)(((byte*)pmixList) + (Marshal.SizeOf(typeof(MIXERCONTROLDETAILS_LISTTEXT)) * y));
                            MIXERCONTROLDETAILS_LISTTEXT mixList = (MIXERCONTROLDETAILS_LISTTEXT)Marshal.PtrToStructure(pVmixList, typeof(MIXERCONTROLDETAILS_LISTTEXT));

                            uint lineId = mixList.dwParam1;
                            uint* pBoolVal = (((uint*)pmixBool) + y);

                            if (lineId == value)
                                *pBoolVal = 1;
                            else
                                *pBoolVal = 0;
                        }

                        errorCode = (MMErrors)MixerNative.mixerSetControlDetails(AudioLine.AudioDeviceInternal.Handle, ref mxcdb, MIXER_SETCONTROLDETAILSFLAG.VALUE);
                    }
                    else
                    {
                        MIXERCONTROLDETAILS64 mxcdb = new MIXERCONTROLDETAILS64();
                        pmixBool = Marshal.AllocHGlobal((int)(MultipleItems * Marshal.SizeOf(typeof(MIXERCONTROLDETAILS_BOOLEAN))));

                        mxcdb.cbStruct = (uint)sizeof(MIXERCONTROLDETAILS64);
                        mxcdb.dwControlID = Id;
                        mxcdb.cChannels = 1;
                        mxcdb.cMultipleItems = MultipleItems;
                        mxcdb.cbDetails = (uint)Marshal.SizeOf(typeof(MIXERCONTROLDETAILS_BOOLEAN));
                        mxcdb.paDetails = pmixBool;
                        errorCode = (MMErrors)MixerNative.mixerGetControlDetails(AudioLine.AudioDeviceInternal.Handle, ref mxcdb, ((uint)MIXER_OBJECTFLAG.HMIXER | (uint)MIXER_GETCONTROLDETAILSFLAG.VALUE));

                        if (errorCode != MMErrors.MMSYSERR_NOERROR)
                            throw new MixerException(errorCode, Audio.GetErrorDescription(FuncName.fnMixerGetControlDetails, errorCode));

                        for (uint y = 0; y < mxcdb.cMultipleItems; y++)
                        {
                            IntPtr pVmixList = (IntPtr)(((byte*)pmixList) + (Marshal.SizeOf(typeof(MIXERCONTROLDETAILS_LISTTEXT)) * y));
                            MIXERCONTROLDETAILS_LISTTEXT mixList = (MIXERCONTROLDETAILS_LISTTEXT)Marshal.PtrToStructure(pVmixList, typeof(MIXERCONTROLDETAILS_LISTTEXT));

                            uint lineId = mixList.dwParam1;
                            uint* pBoolVal = (((uint*)pmixBool) + y);

                            if (lineId == value)
                                *pBoolVal = 1;
                            else
                                *pBoolVal = 0;
                        }

                        errorCode = (MMErrors)MixerNative.mixerSetControlDetails(AudioLine.AudioDeviceInternal.Handle, ref mxcdb, MIXER_SETCONTROLDETAILSFLAG.VALUE);
                    }

                    if (errorCode != MMErrors.MMSYSERR_NOERROR)
                        throw new MixerException(errorCode, Audio.GetErrorDescription(FuncName.fnMixerSetControlDetails, errorCode));

                    foreach (var line in AudioLine.AudioDeviceInternal.LinesInternal)
                        MixerNative.SendMessage(AudioLine.AudioDeviceInternal.AudioInternal.CallbackForm.Handle, MixerNative.MM_MIXM_LINE_CHANGE, (uint)AudioLine.AudioDeviceInternal.Handle, line.Id);
                }
                finally
                {
                    if (pmixList != IntPtr.Zero)
                        Marshal.FreeHGlobal(pmixList);
                    if (pmixBool != IntPtr.Zero)
                        Marshal.FreeHGlobal(pmixBool);
                }
            }
        }
        
        public AudioLineControl(AudioLine audioLine, MIXERCONTROL mc, Action<AudioLine> raiseEventDelegate)
        {
            AudioLine = audioLine;
            _raiseEventDelegate = raiseEventDelegate;

            Id = mc.dwControlID;
            ControlType = (MIXERCONTROL_CONTROLTYPE)mc.dwControlType;
            ControlFlag = (MIXERCONTROL_CONTROLFLAG)mc.fdwControl;
            MultipleItems = mc.cMultipleItems;
            Name = mc.szName;
            Minimum = mc.Bounds.dwMinimum;
            Maximum = mc.Bounds.dwMaximum;
            Steps = mc.Metrics.cSteps;
        }

        internal virtual void HandleChanged()
        {
            if (_raiseEventDelegate != null)
                _raiseEventDelegate.Invoke(AudioLine);
        }
    }
}
