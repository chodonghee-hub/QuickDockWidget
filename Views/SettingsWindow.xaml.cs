using System.Windows;
using System.Windows.Controls;
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
            => _vm.SaveBookmark();

        private void CancelButton_Click(object sender, RoutedEventArgs e)
            => _vm.ClearForm();

        private void CloseButton_Click(object sender, RoutedEventArgs e)
            => this.Close();
    }
}