using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
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
            // 설정 창 열려있으면 무시
            foreach (Window w in Application.Current.Windows)
                if (w is SettingsWindow) return;

            if (this.IsVisible)
            {
                this.Hide();
            }
            else
            {
                PositionWindow();
                this.Show();
                this.Activate();
                FocusFirstButton();
            }
        }

        // 첫 번째 버튼에 포커스
        private void FocusFirstButton()
        {
            this.Dispatcher.InvokeAsync(() =>
            {
                BookmarkItemsControl.UpdateLayout();

                var container = BookmarkItemsControl
                    .ItemContainerGenerator
                    .ContainerFromIndex(0) as ContentPresenter;

                if (container == null) return;

                var button = FindVisualChildren<Button>(container).FirstOrDefault();
                button?.Focus();
                if (button != null) Keyboard.Focus(button);

            }, System.Windows.Threading.DispatcherPriority.Render);
        }

        // 시각적 트리에서 특정 타입의 자식 요소 탐색
        private static System.Collections.Generic.IEnumerable<T> FindVisualChildren<T>(
            DependencyObject parent) where T : DependencyObject
        {
            int count = System.Windows.Media.VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < count; i++)
            {
                var child = System.Windows.Media.VisualTreeHelper.GetChild(parent, i);
                if (child is T t) yield return t;
                foreach (var descendant in FindVisualChildren<T>(child))
                    yield return descendant;
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