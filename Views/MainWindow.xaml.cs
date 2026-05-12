using System;
using System.Windows;
using System.Windows.Input;
using QuickDock.Services;
using QuickDock.ViewModels;

namespace QuickDock.Views
{
    public partial class MainWindow : Window
    {
        private readonly HotkeyService _hotkeyService = new();
        private readonly MainViewModel _viewModel = new();

        public MainWindow()
        {
            InitializeComponent();
            DataContext = _viewModel;
            _viewModel.CloseRequested += () => this.Hide();
            PositionWindow();
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            _hotkeyService.HotkeyPressed += OnHotkeyPressed;
            _hotkeyService.HotkeyConflicted += OnHotkeyConflicted;
            _hotkeyService.Register(this);
        }

        private void OnHotkeyPressed()
        {
            if (this.IsVisible)
                this.Hide();
            else
            {
                PositionWindow();
                this.Show();
                this.Activate();
            }
        }

        private void OnHotkeyConflicted()
        {
            MessageBox.Show(
                "단축키 Ctrl+` 가 다른 앱과 충돌합니다.\n설정에서 단축키를 변경해주세요.",
                "QuickDock — 단축키 충돌",
                MessageBoxButton.OK,
                MessageBoxImage.Warning);
        }

        private void PositionWindow()
        {
            var screen = SystemParameters.WorkArea;
            this.Left = (screen.Width - this.Width) / 2;
            this.Top = (screen.Height - this.Height) / 2;
        }

        protected override void OnDeactivated(EventArgs e)
        {
            base.OnDeactivated(e);
            this.Hide();
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
                this.Hide();
            base.OnKeyDown(e);
        }

        protected override void OnClosed(EventArgs e)
        {
            _hotkeyService.Dispose();
            base.OnClosed(e);
        }
    }
}