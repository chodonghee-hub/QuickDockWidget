using System.Windows;
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
    }
}