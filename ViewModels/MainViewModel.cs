using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using QuickDock.Models;
using QuickDock.Services;

namespace QuickDock.ViewModels
{
    public class MainViewModel
    {
        private readonly BrowserService _browserService = new();

        public ObservableCollection<Bookmark> Bookmarks { get; } = new();

        public ICommand OpenBookmarkCommand { get; }

        public event Action? CloseRequested;

        public MainViewModel()
        {
            OpenBookmarkCommand = new RelayCommand<Bookmark>(OpenBookmark);

            Bookmarks.Add(new Bookmark { Title = "GitHub", Url = "https://github.com" });
            Bookmarks.Add(new Bookmark { Title = "ChatGPT", Url = "https://chat.openai.com" });
            Bookmarks.Add(new Bookmark { Title = "Notion", Url = "https://notion.so" });
            Bookmarks.Add(new Bookmark { Title = "Figma", Url = "https://figma.com" });
            Bookmarks.Add(new Bookmark { Title = "YouTube", Url = "https://youtube.com" });
            Bookmarks.Add(new Bookmark { Title = "Discord", Url = "https://discord.com" });
        }

        private void OpenBookmark(Bookmark? bookmark)
        {
            if (bookmark is null) return;

            var result = _browserService.Open(bookmark.Url);

            switch (result)
            {
                case BrowserService.OpenResult.Success:
                    CloseRequested?.Invoke();
                    break;

                case BrowserService.OpenResult.InvalidUrl:
                    MessageBox.Show($"잘못된 URL 형식입니다:\n{bookmark.Url}",
                        "QuickDock", MessageBoxButton.OK, MessageBoxImage.Warning);
                    break;

                case BrowserService.OpenResult.BlockedScheme:
                    MessageBox.Show($"허용되지 않는 URL 형식입니다:\n{bookmark.Url}",
                        "QuickDock", MessageBoxButton.OK, MessageBoxImage.Warning);
                    break;

                case BrowserService.OpenResult.LaunchFailed:
                    MessageBox.Show($"브라우저를 실행할 수 없습니다:\n{bookmark.Url}",
                        "QuickDock", MessageBoxButton.OK, MessageBoxImage.Error);
                    break;
            }
        }
    }
}