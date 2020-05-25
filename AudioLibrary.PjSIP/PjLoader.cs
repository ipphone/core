using System;
using System.Runtime.InteropServices;
using System.Windows.Threading;
using AudioLibrary.Interfaces;
using Sipek;

namespace AudioLibrary.PjSIP
{
    public class PjLoader : IDisposable
    {
        [DllImport(Sipek.Native.PJSIP_DLL, EntryPoint = "dll_init", CallingConvention = CallingConvention.Cdecl)]
        private static extern int dll_init();
        [DllImport(Sipek.Native.PJSIP_DLL, EntryPoint = "dll_main", CallingConvention = CallingConvention.Cdecl)]
        private static extern int dll_main();
        [DllImport(Sipek.Native.PJSIP_DLL, EntryPoint = "dll_shutdown", CallingConvention = CallingConvention.Cdecl)]
        private static extern int dll_shutdown();

        public PjLoader()
        { }

        public IAudio GetAudio()
        {
            return new Audio();
        }

        public void Init()
        {
            CommonDelegates.Initialize(Dispatcher.CurrentDispatcher);

            dll_init();
            dll_main();
        }

        public void Dispose()
        {
            dll_shutdown();
        }
    }
}
