using System.Diagnostics;
using System.Windows;
using Microsoft.Win32;
using QuickDock.Services;
using QuickDock.Views;

namespace QuickDock
{
    public partial class App : Application
    {
        private TrayService? _trayService;
        private MainWindow? _mainWindow;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            if (e.Args.Contains("--install"))
                RegisterStartup();

            this.ShutdownMode = ShutdownMode.OnExplicitShutdown;

            _mainWindow = new MainWindow();

            _trayService = new TrayService();
            _trayService.Initialize();

            _trayService.ShowWidgetRequested += OnShowWidgetRequested;
            _trayService.OpenSettingsRequested += OnOpenSettingsRequested;
            _trayService.ExitRequested += OnExitRequested;

            _mainWindow.Show();
        }

        private void OnShowWidgetRequested()
        {
            if (_mainWindow is null) return;

            if (_mainWindow.IsVisible)
                _mainWindow.Hide();
            else
            {
                _mainWindow.Show();
                _mainWindow.Activate();
            }
        }

        private void OnOpenSettingsRequested()
        {
            _mainWindow?.OpenSettings();
        }

        private void OnExitRequested()
        {
            _trayService?.Dispose();
            Application.Current.Shutdown();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            _trayService?.Dispose();
            base.OnExit(e);
        }

        private static void RegisterStartup()
        {
            var exePath = Process.GetCurrentProcess().MainModule?.FileName;
            if (exePath is null) return;

            using var key = Registry.CurrentUser
                .OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run", writable: true);
            key?.SetValue("QuickDock", $"\"{exePath}\"");
        }
    }
}