using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using AudioLibrary.Interfaces;

namespace ContactPoint.Core.Audio
{
    internal class AudioLoader
    {
        public static IAudio LoadAudio()
        {
            var assembly = Assembly.LoadFile(Path.GetFullPath("AudioLibrary.PjSIP.dll"));

            var audioDriverType = assembly.GetTypes().FirstOrDefault(x => x.GetInterfaces().Contains(typeof(IAudio)));
            if (audioDriverType != null)
                return (IAudio)Activator.CreateInstance(audioDriverType);

            return null;
        }
    }
}
