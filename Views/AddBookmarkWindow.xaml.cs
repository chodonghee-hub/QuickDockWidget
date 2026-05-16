using System;
using System.Windows;
using System.Windows.Input;
using QuickDock.ViewModels;

namespace QuickDock.Views
{
    public partial class AddBookmarkWindow : Window
    {
        private readonly AddBookmarkViewModel _vm;

        public AddBookmarkWindow(AddBookmarkViewModel viewModel)
        {
            InitializeComponent();
            _vm = viewModel;
            DataContext = _vm;
            _vm.CloseRequested += () => Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
            => NameTextBox.Focus();

        private void TitleBar_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left) DragMove();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e) => Close();

        private void CancelButton_Click(object sender, RoutedEventArgs e) => Close();

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (_vm.SaveCommand.CanExecute(null))
                _vm.SaveCommand.Execute(null);
        }

        private void IconSelector_KeyDown(object sender, KeyEventArgs e)
        {
            var colors = AddBookmarkViewModel.IconColors;
            int current = -1;
            for (int i = 0; i < colors.Count; i++)
                if (colors[i] == _vm.SelectedIconColor) { current = i; break; }
            if (current == -1) return;

            const int columns = 5;
            int next = e.Key switch
            {
                Key.Left  => Math.Max(0, current - 1),
                Key.Right => Math.Min(colors.Count - 1, current + 1),
                Key.Up    => Math.Max(0, current - columns),
                Key.Down  => Math.Min(colors.Count - 1, current + columns),
                _         => current
            };

            if (next == current) return;
            _vm.SelectedIconColor = colors[next];
            e.Handled = true;
        }
    }
}
