using System;
using System.Collections.Generic;
using System.Text;

namespace ContactPoint.Common.Audio
{
    public interface IAudioPlayer : IDisposable
    {
        void Play(string filename);
        void Stop();
    }
}
