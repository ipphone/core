using System;
using AudioLibrary.Interfaces;
using AudioLibrary.WMME.Native;

namespace AudioLibrary.WMME
{
    internal class MuteAudioLineControl : AudioLineControl
    {
        public MuteAudioLineControl(AudioLine audioLine, MIXERCONTROL mc, Action<AudioLine> raiseEventDelegate)
            : base(audioLine, mc, raiseEventDelegate)
        { }

        public override unsafe object Value
        {
            get { return ValueAsBoolean; }
            set { ValueAsBoolean = (bool)value; }
        }
    }
}
