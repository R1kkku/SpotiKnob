using System;
using System.Runtime.InteropServices;
using System.Threading;

namespace SpotiKnob
{
    internal static class SingleInstanceManager
    {
        private const string MutexName = @"Local\SpotiKnob.SingleInstance";
        private const string ShowWindowMessageName = "SpotiKnob.ShowMainWindow";
        private static readonly int showWindowMessage = NativeMethods.RegisterWindowMessage(ShowWindowMessageName);

        public static int ShowWindowMessage
        {
            get { return showWindowMessage; }
        }

        public static bool TryAcquire(out Mutex mutex)
        {
            bool createdNew;
            mutex = new Mutex(true, MutexName, out createdNew);
            return createdNew;
        }

        public static void SignalExistingInstance()
        {
            NativeMethods.PostMessage(new IntPtr(NativeMethods.HwndBroadcast), ShowWindowMessage, IntPtr.Zero, IntPtr.Zero);
        }

        private static class NativeMethods
        {
            public const int HwndBroadcast = 0xffff;

            [DllImport("user32.dll", CharSet = CharSet.Auto)]
            public static extern int RegisterWindowMessage(string lpString);

            [DllImport("user32.dll", SetLastError = true)]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool PostMessage(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);
        }
    }
}
