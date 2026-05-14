using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Windows;
using System.Windows.Interop;
using QuickDock.Models;

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

        private const uint MOD_CTRL     = 0x0002;
        private const uint MOD_ALT      = 0x0001;
        private const uint MOD_SHIFT    = 0x0004;
        private const uint MOD_WIN      = 0x0008;
        private const uint MOD_NOREPEAT = 0x4000;
        private const int  HOTKEY_ID    = 9000;

        private static readonly string ConfigFile = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "QuickDock", "hotkey.json");

        private static readonly JsonSerializerOptions JsonOptions = new() { WriteIndented = true };

        private IntPtr      _windowHandle;
        private HwndSource? _source;

        public uint CurrentModifiers { get; private set; } = MOD_CTRL | MOD_NOREPEAT;
        public uint CurrentVk        { get; private set; } = 0xC0;

        public event Action? HotkeyPressed;
        public event Action? HotkeyConflicted;

        public void Register(Window window)
        {
            _windowHandle = new WindowInteropHelper(window).Handle;
            _source = HwndSource.FromHwnd(_windowHandle);
            _source.AddHook(WndProc);

            LoadConfig();

            bool success = RegisterHotKey(_windowHandle, HOTKEY_ID, CurrentModifiers, CurrentVk);
            if (!success)
                HotkeyConflicted?.Invoke();
        }

        public void TemporaryUnregister()
        {
            if (_windowHandle != IntPtr.Zero)
                UnregisterHotKey(_windowHandle, HOTKEY_ID);
        }

        // modifiers: MOD_NOREPEAT 제외한 modifier 플래그 조합
        public bool ChangeHotkey(uint modifiers, uint vk)
        {
            TemporaryUnregister();

            uint fullMods = modifiers | MOD_NOREPEAT;
            bool ok = RegisterHotKey(_windowHandle, HOTKEY_ID, fullMods, vk);

            if (!ok)
            {
                // 등록 실패 시 기존 단축키 복구 시도
                RegisterHotKey(_windowHandle, HOTKEY_ID, CurrentModifiers, CurrentVk);
                HotkeyConflicted?.Invoke();
                return false;
            }

            CurrentModifiers = fullMods;
            CurrentVk        = vk;
            SaveConfig();
            return true;
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

        private void LoadConfig()
        {
            try
            {
                if (!File.Exists(ConfigFile)) return;

                var json   = File.ReadAllText(ConfigFile);
                var config = JsonSerializer.Deserialize<HotkeyConfig>(json);
                if (config is null) return;

                CurrentModifiers = config.Modifiers;
                CurrentVk        = config.Vk;
            }
            catch { /* 로드 실패 시 기본값 유지 */ }
        }

        private void SaveConfig()
        {
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(ConfigFile)!);
                var json = JsonSerializer.Serialize(
                    new HotkeyConfig { Modifiers = CurrentModifiers, Vk = CurrentVk },
                    JsonOptions);
                File.WriteAllText(ConfigFile, json);
            }
            catch { }
        }
    }
}
