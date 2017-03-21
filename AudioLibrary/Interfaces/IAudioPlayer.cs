using System;
using System.Collections.Generic;
using System.Text;

namespace AudioLibrary.Interfaces
{
    public interface IAudioPlayer : IDisposable
    {
        void Play(string fileName);
        void Stop();
    }
}
