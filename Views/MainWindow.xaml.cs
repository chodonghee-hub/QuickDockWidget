using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using QuickDock.Services;
using QuickDock.ViewModels;

namespace QuickDock.Views
{
    public partial class MainWindow : Window
    {
        private readonly MainViewModel _viewModel = new();
        private bool _isAnimating;
        private readonly TranslateTransform _bookmarkTranslate = new();

        public MainWindow()
        {
            InitializeComponent();
            DataContext = _viewModel;
            _viewModel.CloseRequested += () => this.Hide();
            BookmarkItemsControl.RenderTransform = _bookmarkTranslate;
        }

        protected override void OnContentRendered(EventArgs e)
        {
            base.OnContentRendered(e);
            SizeToContent = SizeToContent.Manual;
            PositionWindow();
            _viewModel.Bookmarks.CollectionChanged += (_, _) => ResizeWindow();
        }

        private void ResizeWindow()
        {
            Dispatcher.InvokeAsync(() =>
            {
                SizeToContent = SizeToContent.Height;
                UpdateLayout();
                SizeToContent = SizeToContent.Manual;
                PositionWindow();
            }, DispatcherPriority.Loaded);
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
            Dispatcher.InvokeAsync(() =>
            {
                var button = FindVisualChildren<Button>(BookmarkItemsControl)
                    .FirstOrDefault(b => b.IsVisible);
                button?.Focus();
                if (button != null) Keyboard.Focus(button);
            }, DispatcherPriority.DataBind);
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
            if (_isAnimating)
            {
                _bookmarkTranslate.BeginAnimation(TranslateTransform.XProperty, null);
                BookmarkItemsControl.BeginAnimation(UIElement.OpacityProperty, null);
                _bookmarkTranslate.X = 0;
                BookmarkItemsControl.Opacity = 1;
                _isAnimating = false;
            }
            this.Hide();
        }

        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            if (e.Key == Key.Left)
            {
                AnimatePageChange(goNext: false, focusLast: false);
                e.Handled = true;
            }
            else if (e.Key == Key.Right)
            {
                AnimatePageChange(goNext: true, focusLast: false);
                e.Handled = true;
            }
            else if (e.Key == Key.Tab)
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

            var buttons = FindVisualChildren<Button>(BookmarkItemsControl)
                .Where(b => b.IsVisible).ToList();
            if (buttons.Count == 0) return;

            var focused = buttons.FirstOrDefault(b => b.IsKeyboardFocused);
            if (focused == null) return;

            int index = buttons.IndexOf(focused);

            if (e.Key == Key.Up && index == 0)
            {
                e.Handled = true;
                AnimatePageChange(goNext: false, focusLast: true);
            }
            else if (e.Key == Key.Down && index == buttons.Count - 1)
            {
                e.Handled = true;
                AnimatePageChange(goNext: true, focusLast: false);
            }
        }

        private void FocusButtonAfterPageChange(bool focusLast)
        {
            var buttons = FindVisualChildren<Button>(BookmarkItemsControl)
                .Where(b => b.IsVisible).ToList();
            if (buttons.Count == 0) return;
            var target = focusLast ? buttons[buttons.Count - 1] : buttons[0];
            target.Focus();
            Keyboard.Focus(target);
        }

        private void AnimatePageChange(bool goNext, bool focusLast)
        {
            if (_isAnimating) return;
            if (goNext && _viewModel.CurrentPage >= _viewModel.TotalPages - 1) return;
            if (!goNext && _viewModel.CurrentPage <= 0) return;

            _isAnimating = true;

            double outX = goNext ? -30 : 30;
            double inX  = goNext ?  30 : -30;
            var easeIn  = new CubicEase { EasingMode = EasingMode.EaseIn };
            var easeOut = new CubicEase { EasingMode = EasingMode.EaseOut };

            var fadeOut  = new DoubleAnimation(1, 0, TimeSpan.FromMilliseconds(80))  { EasingFunction = easeIn };
            var slideOut = new DoubleAnimation(0, outX, TimeSpan.FromMilliseconds(80)) { EasingFunction = easeIn };

            fadeOut.Completed += (_, _) =>
            {
                _bookmarkTranslate.BeginAnimation(TranslateTransform.XProperty, null);
                BookmarkItemsControl.BeginAnimation(UIElement.OpacityProperty, null);
                _bookmarkTranslate.X = inX;
                BookmarkItemsControl.Opacity = 0;

                if (goNext) _viewModel.NextPageCommand.Execute(null);
                else        _viewModel.PreviousPageCommand.Execute(null);

                // Completed는 Render 프레임 내에서 실행됨. 여기서 바로 BeginAnimation하면
                // DataBind(우선순위 8)가 다음 Render 프레임 전에 처리되어
                // 첫 프레임 렌더링 시 데이터가 이미 바인딩된 상태가 됨.
                var fadeIn  = new DoubleAnimation(0, 1, TimeSpan.FromMilliseconds(130)) { EasingFunction = easeOut, FillBehavior = FillBehavior.Stop };
                var slideIn = new DoubleAnimation(inX, 0, TimeSpan.FromMilliseconds(130)) { EasingFunction = easeOut, FillBehavior = FillBehavior.Stop };

                fadeIn.Completed += (_, _) =>
                {
                    _bookmarkTranslate.BeginAnimation(TranslateTransform.XProperty, null);
                    BookmarkItemsControl.BeginAnimation(UIElement.OpacityProperty, null);
                    _bookmarkTranslate.X = 0;
                    BookmarkItemsControl.Opacity = 1;
                    _isAnimating = false;
                    FocusButtonAfterPageChange(focusLast);
                };

                _bookmarkTranslate.BeginAnimation(TranslateTransform.XProperty, slideIn);
                BookmarkItemsControl.BeginAnimation(UIElement.OpacityProperty, fadeIn);
            };

            _bookmarkTranslate.BeginAnimation(TranslateTransform.XProperty, slideOut);
            BookmarkItemsControl.BeginAnimation(UIElement.OpacityProperty, fadeOut);
        }

        protected override void OnClosed(EventArgs e)
        {
            _viewModel.HotkeyService.Dispose();
            base.OnClosed(e);
        }
    }
}