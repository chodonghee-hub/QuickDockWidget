using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

namespace QuickDock.Services
{
    public class HotkeyService : IDisposable
    {
        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        private const uint MOD_CTRL = 0x0002;
        private const uint MOD_NOREPEAT = 0x4000;
        private const int HOTKEY_ID = 9000;

        private IntPtr _windowHandle;
        private HwndSource? _source;

        public event Action? HotkeyPressed;
        public event Action? HotkeyConflicted;

        public void Register(Window window)
        {
            _windowHandle = new WindowInteropHelper(window).Handle;
            _source = HwndSource.FromHwnd(_windowHandle);
            _source.AddHook(WndProc);

            bool success = RegisterHotKey(_windowHandle, HOTKEY_ID, MOD_CTRL | MOD_NOREPEAT, 0xC0);

            if (!success)
                HotkeyConflicted?.Invoke();
        }

        public void Unregister()
        {
            if (_windowHandle != IntPtr.Zero)
                UnregisterHotKey(_windowHandle, HOTKEY_ID);
        }

        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            const int WM_HOTKEY = 0x0312;

            if (msg == WM_HOTKEY && wParam.ToInt32() == HOTKEY_ID)
            {
                HotkeyPressed?.Invoke();
                handled = true;
            }

            return IntPtr.Zero;
        }

        public void Dispose()
        {
            Unregister();
            _source?.RemoveHook(WndProc);
        }
    }
}