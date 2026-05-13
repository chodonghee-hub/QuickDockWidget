using System;
using System.Diagnostics;
using System.Windows.Controls;
using Microsoft.Win32;

namespace QuickDock.Services
{
    public class TrayService : IDisposable
    {
        private Hardcodet.Wpf.TaskbarNotification.TaskbarIcon? _trayIcon;

        public event Action? ShowWidgetRequested;
        public event Action? OpenSettingsRequested;
        public event Action? ExitRequested;

        public void Initialize()
        {
            _trayIcon = new Hardcodet.Wpf.TaskbarNotification.TaskbarIcon
            {
                ToolTipText = "QuickDock"
            };

            var menu = new ContextMenu();

            var showItem = new MenuItem { Header = "위젯 보이기" };
            showItem.Click += (_, _) => ShowWidgetRequested?.Invoke();

            var settingsItem = new MenuItem { Header = "설정 열기" };
            settingsItem.Click += (_, _) => OpenSettingsRequested?.Invoke();

            var startupItem = new MenuItem
            {
                Header = "시작 시 자동 실행",
                IsCheckable = true,
                IsChecked = IsStartupEnabled()
            };
            startupItem.Click += (_, _) => ToggleStartup(startupItem.IsChecked);

            var separator = new Separator();

            var exitItem = new MenuItem { Header = "종료" };
            exitItem.Click += (_, _) => ExitRequested?.Invoke();

            menu.Items.Add(showItem);
            menu.Items.Add(settingsItem);
            menu.Items.Add(startupItem);
            menu.Items.Add(separator);
            menu.Items.Add(exitItem);

            _trayIcon.ContextMenu = menu;
            _trayIcon.TrayMouseDoubleClick += (_, _) => ShowWidgetRequested?.Invoke();
        }

        public void ShowNotification(string title, string message)
        {
            _trayIcon?.ShowBalloonTip(
                title, message,
                Hardcodet.Wpf.TaskbarNotification.BalloonIcon.Info);
        }

        private bool IsStartupEnabled()
        {
            using var key = Registry.CurrentUser
                .OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run");
            return key?.GetValue("QuickDock") is not null;
        }

        private void ToggleStartup(bool enable)
        {
            using var key = Registry.CurrentUser
                .OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run", writable: true);
            if (key is null) return;

            if (enable)
            {
                var exePath = Process.GetCurrentProcess().MainModule?.FileName;
                if (exePath is not null)
                    key.SetValue("QuickDock", $"\"{exePath}\"");
            }
            else
            {
                key.DeleteValue("QuickDock", throwOnMissingValue: false);
            }
        }

        public void Dispose()
        {
            _trayIcon?.Dispose();
        }
    }
}