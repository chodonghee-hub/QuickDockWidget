using System.Windows;

namespace QuickDock.Views
{
    public partial class ConfirmDeleteWindow : Window
    {
        public ConfirmDeleteWindow(string bookmarkTitle)
        {
            InitializeComponent();
            MessageText.Text = $"'{bookmarkTitle}'을(를) 삭제하시겠습니까?";
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
            => DialogResult = false;

        private void ConfirmButton_Click(object sender, RoutedEventArgs e)
            => DialogResult = true;
    }
}
