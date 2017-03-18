using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Threading;
using AudioLibrary.Interfaces;
using Sipek;

namespace AudioLibrary.PjSIP
{
    public class PjLoader : IDisposable
    {
        internal const string PJSIP_DLL = "pjsipDll.dll";

        [DllImport(PJSIP_DLL, EntryPoint = "dll_init", CallingConvention = CallingConvention.Cdecl)]
        private static extern int dll_init();
        [DllImport(PJSIP_DLL, EntryPoint = "dll_main", CallingConvention = CallingConvention.Cdecl)]
        private static extern int dll_main();
        [DllImport(PJSIP_DLL, EntryPoint = "dll_shutdown", CallingConvention = CallingConvention.Cdecl)]
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
