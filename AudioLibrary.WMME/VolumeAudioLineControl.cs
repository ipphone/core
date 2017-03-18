using System;
using AudioLibrary.Interfaces;
using AudioLibrary.WMME.Native;

namespace AudioLibrary.WMME
{
    internal class VolumeAudioLineControl : AudioLineControl
    {
        public VolumeAudioLineControl(AudioLine audioLine, MIXERCONTROL mc, Action<AudioLine> raiseEventDelegate)
            : base(audioLine, mc, raiseEventDelegate)
        { }

        public override unsafe object Value
        {
            get { return ValueAsUnsigned; }
            set { ValueAsUnsigned = (int)value; }
        }
    }
}
