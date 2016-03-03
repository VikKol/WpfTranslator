using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace WpfTranslator
{
    class WinApi
    {
        public const int TranslateHotKeyId = 888;
        public const int PronounceHotKeyId = 999;

        const int MOD_ALT = 0x1;
        const int MOD_CONTROL = 0x2;
        const int MOD_SHIFT = 0x4;
        const int MOD_WIN = 0x8;
        const int WM_HOTKEY = 0x312;

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        public static void RegisterAppHotKey(IntPtr hWnd)
        {
            RegisterHotKey(hWnd, TranslateHotKeyId, MOD_SHIFT, (int)Keys.Q);
            RegisterHotKey(hWnd, PronounceHotKeyId, MOD_SHIFT, (int)Keys.W);
        }

        public static void UnregisterAppHotKey(IntPtr hWnd)
        {
            UnregisterHotKey(hWnd, TranslateHotKeyId);
            UnregisterHotKey(hWnd, PronounceHotKeyId);
        }
    }
}
