using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using QuickDock.Models;
using QuickDock.Services;
using QuickDock.Views;

namespace QuickDock.ViewModels
{
    public class MainViewModel
    {
        private readonly BrowserService _browserService = new();
        private readonly JsonService _jsonService = new();

        public JsonService JsonService => _jsonService;     // SettingsViewModel에서 접근할 수 있도록 공개

        public ObservableCollection<Bookmark> Bookmarks { get; } = new();

        public ICommand OpenBookmarkCommand { get; }
        public ICommand OpenSettingsCommand { get; }

        public event Action? CloseRequested;

        public MainViewModel()
        {
            OpenBookmarkCommand = new RelayCommand<Bookmark>(OpenBookmark);
            OpenSettingsCommand = new RelayCommand<object>(_ => OpenSettings());
            Bookmarks.CollectionChanged += (_, _) => UpdateIndices();
            LoadBookmarks();
        }

        private void LoadBookmarks()
        {
            var saved = _jsonService.Load();

            if (saved.Count > 0)
            {
                foreach (var bookmark in saved)
                    Bookmarks.Add(bookmark);
            }
            else
            {
                var defaults = new[]
                {
                    new Bookmark { Title = "GitHub",   Url = "https://github.com" },
                    new Bookmark { Title = "Notion",   Url = "https://notion.so" },
                    new Bookmark { Title = "Figma",    Url = "https://figma.com" },
                    new Bookmark { Title = "YouTube",  Url = "https://youtube.com" },
                    new Bookmark { Title = "ChatGPT",  Url = "https://chat.openai.com" },
                    new Bookmark { Title = "Discord",  Url = "https://discord.com" },
                    new Bookmark { Title = "Supabase", Url = "https://supabase.com" },
                };

                foreach (var bookmark in defaults)
                    Bookmarks.Add(bookmark);

                SaveBookmarks();
            }

            UpdateIndices();
        }

        private void UpdateIndices()
        {
            for (int i = 0; i < Bookmarks.Count; i++)
                Bookmarks[i].Index = i + 1;
        }

        public bool SaveBookmarks()
        {
            var result = _jsonService.Save(
                new System.Collections.Generic.List<Bookmark>(Bookmarks));

            if (!result)
                MessageBox.Show("북마크 저장에 실패했습니다.",
                    "QuickDock", MessageBoxButton.OK, MessageBoxImage.Warning);

            return result;
        }

        private void OpenSettings()
        {
            var vm = new SettingsViewModel(Bookmarks, _jsonService);
            var window = new SettingsWindow(vm);
            window.ShowDialog();
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