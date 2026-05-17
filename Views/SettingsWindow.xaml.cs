using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using QuickDock.Models;
using QuickDock.ViewModels;

namespace QuickDock.Views
{
    public partial class SettingsWindow : Window
    {
        private SettingsViewModel _vm = null!;
        private bool _isKeyboardNavigation;

        public SettingsWindow(SettingsViewModel viewModel)
        {
            InitializeComponent();
            _vm = viewModel;
            DataContext = _vm;
            _vm.CloseRequested += () => this.Close();
            Loaded += (_, _) => BookmarkListBox.Focus();
        }

        private void BookmarkListBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Up || e.Key == Key.Down)
                _isKeyboardNavigation = true;
        }

        private void BookmarkListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_isKeyboardNavigation)
            {
                _isKeyboardNavigation = false;
                return;
            }
            if (BookmarkListBox.SelectedItem is Bookmark bookmark)
                ActivateEdit(bookmark);
        }

        private void ActivateEdit(Bookmark bookmark)
        {
            _vm.StartEdit(bookmark);
            Dispatcher.BeginInvoke(new System.Action(() =>
            {
                BookmarkListBox.SelectedIndex = -1;
                TitleTextBox.Focus();
            }));
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is Bookmark bookmark)
            {
                var dialog = new ConfirmDeleteWindow(bookmark.Title) { Owner = this };
                if (dialog.ShowDialog() == true)
                    _vm.DeleteBookmark(bookmark);
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (_vm.SaveBookmark())
                this.Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
            => this.Close();

        private void CloseButton_Click(object sender, RoutedEventArgs e)
            => this.Close();

        private void TitleBar_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }

        private void ChangeHotkeyButton_Click(object sender, RoutedEventArgs e)
            => _vm.ToggleCaptureMode();

        private void SettingsWindow_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            var key = e.Key == Key.System ? e.SystemKey : e.Key;

            if ((key == Key.Left || key == Key.Right) && IconColorPicker.IsKeyboardFocusWithin)
            {
                var colors = SettingsViewModel.IconColors;
                int idx = 0;
                for (int i = 0; i < colors.Count; i++)
                    if (colors[i] == _vm.InputIconColor) { idx = i; break; }
                idx = key == Key.Left
                    ? (idx - 1 + colors.Count) % colors.Count
                    : (idx + 1) % colors.Count;
                _vm.SelectIconColorCommand.Execute(colors[idx]);
                e.Handled = true;
                return;
            }

            if (key == Key.Return && !_vm.IsCapturingHotkey)
            {
                if (!_vm.IsEditMode && BookmarkListBox.IsKeyboardFocusWithin
                    && BookmarkListBox.SelectedItem is Bookmark bm)
                {
                    ActivateEdit(bm);
                    e.Handled = true;
                    return;
                }
                if (_vm.IsEditMode)
                {
                    if (_vm.SaveBookmark())
                        this.Close();
                    e.Handled = true;
                    return;
                }
            }

            if (!_vm.IsCapturingHotkey) return;

            if (key is Key.LeftCtrl  or Key.RightCtrl  or
                       Key.LeftShift or Key.RightShift  or
                       Key.LeftAlt   or Key.RightAlt    or
                       Key.LWin      or Key.RWin        or
                       Key.System)
                return;

            _vm.UpdatePendingHotkey(Keyboard.Modifiers, key);
            e.Handled = true;
        }

        private void BookmarkListBox_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (e.Handled) return;
            e.Handled = true;
            var args = new MouseWheelEventArgs(e.MouseDevice, e.Timestamp, e.Delta)
            {
                RoutedEvent = MouseWheelEvent,
                Source = sender
            };
            (sender as UIElement)?.RaiseEvent(args);
        }
    }
}