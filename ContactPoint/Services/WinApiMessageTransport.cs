using System;
using System.Runtime.InteropServices;

namespace ContactPoint.Services
{
    class WinApiMessageTransport
    {
        public const uint MAKECALL_MESSAGE_ID = 4890271;
        public const uint WM_COPYDATA = 0x4A;

        [DllImport("user32.dll")]
        private static extern int SendMessage(
              IntPtr hWnd,      // handle to destination window
              UInt32 msg,       // message
              uint wParam,  // first message parameter
              COPYDATASTRUCT lParam   // second message parameter
              );

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        /// <summary>
        /// Structure required to be sent with the WM_COPYDATA message
        /// This structure is used to contain the CommandLine
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public class COPYDATASTRUCT
        {
            public IntPtr dwData = new IntPtr(3);//32 bit int to passed. Not used.
            public int cbData;//length of string. Will be one greater because of null termination.
            [MarshalAs(UnmanagedType.LPStr)]
            public string lpData;//string to be passed.

            public COPYDATASTRUCT()
            { }

            public COPYDATASTRUCT(string data)
            {
                lpData = data + "\0"; //add null termination
                cbData = lpData.Length; //length includes null chr so will be one greater
            }
        }
    }
}
