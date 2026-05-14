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

        public SettingsWindow(SettingsViewModel viewModel)
        {
            InitializeComponent();
            _vm = viewModel;
            DataContext = _vm;
            _vm.CloseRequested += () => this.Close();
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is Bookmark bookmark)
                _vm.StartEdit(bookmark);
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is Bookmark bookmark)
                _vm.DeleteBookmark(bookmark);
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
            if (!_vm.IsCapturingHotkey) return;

            var key = e.Key == Key.System ? e.SystemKey : e.Key;

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