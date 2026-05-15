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
    }
}
