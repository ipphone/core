using AudioLibrary.Interfaces;

namespace AudioLibrary.WMME
{
    public static class AudioLoader
    {
        static object LockObject = new object();
        static IAudio AudioInstance = null;

        public static IAudio GetAudio()
        {
            lock (LockObject)
            {
                if (AudioInstance == null)
                    AudioInstance = new Audio();

                return AudioInstance;
            }
        }
    }
}
