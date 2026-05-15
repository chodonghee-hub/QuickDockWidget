using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using QuickDock.Services;
using QuickDock.ViewModels;

namespace QuickDock.Views
{
    public partial class MainWindow : Window
    {
        private readonly MainViewModel _viewModel = new();

        public MainWindow()
        {
            InitializeComponent();
            DataContext = _viewModel;
            _viewModel.CloseRequested += () => this.Hide();
        }

        protected override void OnContentRendered(EventArgs e)
        {
            base.OnContentRendered(e);
            PositionWindow();
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            _viewModel.HotkeyService.HotkeyPressed += OnHotkeyPressed;
            _viewModel.HotkeyService.HotkeyConflicted += OnHotkeyConflicted;
            _viewModel.HotkeyService.Register(this);
        }

        private void OnHotkeyPressed()
        {
            foreach (Window w in Application.Current.Windows)
            {
                if (w is SettingsWindow or AddBookmarkWindow)
                {
                    w.Close();
                    this.Show();
                    this.UpdateLayout();
                    PositionWindow();
                    this.Activate();
                    FocusFirstButton();
                    return;
                }
            }

            if (this.IsVisible)
                this.Hide();
            else
            {
                this.Show();
                this.UpdateLayout();
                PositionWindow();
                this.Activate();
                FocusFirstButton();
            }
        }

        public void OpenSettings()
        {
            foreach (Window w in Application.Current.Windows)
            {
                if (w is SettingsWindow)
                {
                    w.Activate();
                    return;
                }
            }

            var vm = new SettingsViewModel(_viewModel.Bookmarks, _viewModel.JsonService, _viewModel.HotkeyService);
            var window = new SettingsWindow(vm);
            window.Closed += (s, e) =>
            {
                Show();
                UpdateLayout();
                PositionWindow();
                Activate();
                FocusFirstButton();
            };
            window.Show();
        }

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
            }, DispatcherPriority.Render);
        }

        private static System.Collections.Generic.IEnumerable<T> FindVisualChildren<T>(
            DependencyObject parent) where T : DependencyObject
        {
            int count = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < count; i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                if (child is T t) yield return t;
                foreach (var d in FindVisualChildren<T>(child))
                    yield return d;
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
            this.Left = screen.Right - this.Width - 16;
            this.Top = screen.Bottom - this.ActualHeight - 16;
        }

        protected override void OnDeactivated(EventArgs e)
        {
            base.OnDeactivated(e);
            this.Hide();
        }

        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            if (e.Key == Key.Tab)
            {
                var buttons = FindVisualChildren<Button>(BookmarkItemsControl).ToList();
                bool focusInList = buttons.Any(b => b.IsKeyboardFocused);

                if (focusInList)
                {
                    SettingsButton.Focus();
                    e.Handled = true;
                }
                else if (SettingsButton.IsKeyboardFocused)
                {
                    AddButton.Focus();
                    e.Handled = true;
                }
                else if (AddButton.IsKeyboardFocused)
                {
                    FocusFirstButton();
                    e.Handled = true;
                }
            }
            base.OnPreviewKeyDown(e);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
                this.Hide();
            base.OnKeyDown(e);
        }

        private void BookmarkItemsControl_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Up && e.Key != Key.Down) return;

            var buttons = FindVisualChildren<Button>(BookmarkItemsControl).ToList();
            if (buttons.Count == 0) return;

            var focused = buttons.FirstOrDefault(b => b.IsKeyboardFocused);
            if (focused == null) return;

            int index = buttons.IndexOf(focused);

            if (e.Key == Key.Up && index == 0)
            {
                e.Handled = true;
                buttons[buttons.Count - 1].Focus();
            }
            else if (e.Key == Key.Down && index == buttons.Count - 1)
            {
                e.Handled = true;
                buttons[0].Focus();
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            _viewModel.HotkeyService.Dispose();
            base.OnClosed(e);
        }
    }
}