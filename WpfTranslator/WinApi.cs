using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

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

        #region Keyboard

        const int KEYEVENTF_KEYUP = 2;
        const int KEYEVENTF_EXTENDEDKEY = 1;

        [DllImport("user32.dll")]
        static extern void keybd_event(byte bVk, byte bScan, int dwFlags, int dwExtraInfo);

        public static void KeyDown(Keys vKey)
        {
            keybd_event((byte)vKey, 0, KEYEVENTF_EXTENDEDKEY, 0);
        }

        public static void KeyUp(Keys vKey)
        {
            keybd_event((byte)vKey, 0, KEYEVENTF_EXTENDEDKEY | KEYEVENTF_KEYUP, 0);
        }

        public static void KeyPress(Keys key)
        {
            KeyDown(key);
            KeyUp(key);
        }

        public static void SendCtrlC()
        {
            KeyDown(Keys.Control);
            KeyDown(Keys.C);
            KeyUp(Keys.C);
            KeyUp(Keys.Control);
        }

        #endregion
    }
}
