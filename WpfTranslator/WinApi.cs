using System;
using System.Runtime.InteropServices;

namespace WpfTranslator
{
    class WinApi
    {
        public const int AppHotKeyId = 999;
        const int VK_Q = 81;
        const int MOD_ALT = 0x0001;

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        public static void RegisterAppHotKey(IntPtr hWnd) => RegisterHotKey(hWnd, AppHotKeyId, MOD_ALT, VK_Q);
        public static void UnregisterAppHotKey(IntPtr hWnd) => UnregisterHotKey(hWnd, AppHotKeyId);
    }
}
