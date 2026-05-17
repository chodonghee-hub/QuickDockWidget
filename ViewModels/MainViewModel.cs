using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using QuickDock.Models;
using QuickDock.Services;
using QuickDock.Views;

namespace QuickDock.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        private readonly BrowserService _browserService = new();
        private readonly JsonService    _jsonService    = new();

        public JsonService    JsonService    => _jsonService;
        public HotkeyService  HotkeyService  { get; } = new();

        public ObservableCollection<Bookmark> Bookmarks { get; } = new();

        // 6개 슬롯 고정 — Replace 알림으로 컨테이너 재사용 (생성/파괴 없음)
        public ObservableCollection<Bookmark?> PagedBookmarks { get; } =
            new(new Bookmark?[] { null, null, null, null, null, null });

        private List<bool> _pageDots = new();
        public IReadOnlyList<bool> PageDots
        {
            get => _pageDots;
            private set { _pageDots = (List<bool>)value; OnPropertyChanged(); }
        }

        public const int PageSize = 6;
        private int _currentPage = 0;

        public int CurrentPage
        {
            get => _currentPage;
            private set
            {
                if (_currentPage == value) return;
                _currentPage = value;
                OnPropertyChanged();
                RefreshPage();
            }
        }

        public int  TotalPages       => Bookmarks.Count == 0 ? 1 : (int)Math.Ceiling(Bookmarks.Count / (double)PageSize);
        public bool HasMultiplePages => TotalPages > 1;

        public ICommand OpenBookmarkCommand    { get; }
        public ICommand OpenSettingsCommand    { get; }
        public ICommand OpenAddBookmarkCommand { get; }
        public ICommand PreviousPageCommand    { get; }
        public ICommand NextPageCommand        { get; }

        public event Action? CloseRequested;

        public MainViewModel()
        {
            OpenBookmarkCommand    = new RelayCommand<Bookmark>(OpenBookmark);
            OpenSettingsCommand    = new RelayCommand<object>(_ => OpenSettings());
            OpenAddBookmarkCommand = new RelayCommand<object>(_ => OpenAddBookmark());
            PreviousPageCommand    = new RelayCommand<object>(_ => { if (_currentPage > 0) CurrentPage--; });
            NextPageCommand        = new RelayCommand<object>(_ => { if (_currentPage < TotalPages - 1) CurrentPage++; });
            Bookmarks.CollectionChanged += (_, _) => { UpdateIndices(); RefreshPagination(); };
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
            RefreshPage();
        }

        private void RefreshPage()
        {
            int start = _currentPage * PageSize;
            for (int slot = 0; slot < PageSize; slot++)
            {
                int idx = start + slot;
                PagedBookmarks[slot] = idx < Bookmarks.Count ? Bookmarks[idx] : null;
            }
            PageDots = Enumerable.Range(0, TotalPages).Select(i => i == _currentPage).ToList();
            OnPropertyChanged(nameof(HasMultiplePages));
            OnPropertyChanged(nameof(TotalPages));
        }

        private void RefreshPagination()
        {
            if (_currentPage >= TotalPages) _currentPage = Math.Max(0, TotalPages - 1);
            RefreshPage();
            OnPropertyChanged(nameof(CurrentPage));
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
            var vm = new SettingsViewModel(Bookmarks, _jsonService, HotkeyService);
            var window = new SettingsWindow(vm);
            window.ShowDialog();
        }

        private void OpenAddBookmark()
        {
            var vm = new AddBookmarkViewModel(Bookmarks, _jsonService);
            var window = new AddBookmarkWindow(vm);
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