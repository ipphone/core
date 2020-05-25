using System;
using System.Runtime.InteropServices;
using System.Windows.Threading;

namespace Sipek
{
    public static class CommonDelegates
    {
        [DllImport(Native.PJSIP_DLL, EntryPoint = "dll_isThreadRegistered", CallingConvention = CallingConvention.Cdecl)]
        private static extern bool dll_isThreadRegistered();
        [DllImport(Native.PJSIP_DLL, EntryPoint = "dll_threadRegister", CallingConvention = CallingConvention.Cdecl)]
        private static extern int dll_threadRegister(string name);

        private static volatile Dispatcher _dispatcher;

        public static void Initialize(Dispatcher dispatcher)
        {
            _dispatcher = dispatcher;
        }

        public static void SafeBeginInvoke(Delegate del)
        {
            if (Dispatcher.CurrentDispatcher != _dispatcher)
                _dispatcher.BeginInvoke(del);
            else
                del.DynamicInvoke(null);
        }

        public static void SafeInvoke(Delegate del)
        {
            if (Dispatcher.CurrentDispatcher != _dispatcher)
                _dispatcher.BeginInvoke(del);
            else
                del.DynamicInvoke(null);
        }

        public static T SafeInvoke<T>(Delegate del)
        {
            if (Dispatcher.CurrentDispatcher != _dispatcher)
                return (T)_dispatcher.Invoke(del, DispatcherPriority.Send, null);
            else
                return (T)del.DynamicInvoke(null);
        }

        #region Helper methods

        public static void SafeBeginInvoke(Action action)
        {
            SafeBeginInvoke((Delegate)action);
        }

        public static void SafeInvoke(Action action)
        {
            SafeInvoke((Delegate) action);
        }

        public static T SafeInvoke<T>(Func<T> func)
        {
            return SafeInvoke<T>((Delegate) func);
        }
        
        #endregion

        /// <summary>
        /// Deprecated method. Trying to determine is this thread registered in PjSIP.
        /// We're trying to handle thread problems internally, so all call to PjSIP will be made in thread
        /// where PjSIP stack was initialized.
        /// </summary>
        /// <returns></returns>
        private static bool IsThreadRegistered()
        {
            var invokeRequired = !dll_isThreadRegistered();
            if (invokeRequired)
            {
                dll_threadRegister(Guid.NewGuid().ToString());

                invokeRequired = !dll_isThreadRegistered();
            }

            if (invokeRequired) 
            { 
                ContactPoint.Common.Logger.LogError(new InvalidOperationException("Can't register thread in pjsua!")); 
                
                return false; 
            }

            return true;
        }
    }
}
