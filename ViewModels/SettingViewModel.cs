using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using QuickDock.Models;
using QuickDock.Services;

namespace QuickDock.ViewModels
{
    public class SettingsViewModel : INotifyPropertyChanged
    {
        private readonly JsonService _jsonService;
        private readonly ObservableCollection<Bookmark> _bookmarks;
        private Bookmark? _editingBookmark;

        private string _inputTitle = string.Empty;
        private string _inputUrl = string.Empty;

        public ObservableCollection<Bookmark> Bookmarks => _bookmarks;

        public string InputTitle
        {
            get => _inputTitle;
            set { _inputTitle = value; OnPropertyChanged(); }
        }

        public string InputUrl
        {
            get => _inputUrl;
            set { _inputUrl = value; OnPropertyChanged(); }
        }

        public string SaveButtonText => _editingBookmark is null ? "추가" : "저장";

        public ICommand CloseCommand { get; }

        public event Action? CloseRequested;
        public event PropertyChangedEventHandler? PropertyChanged;

        public SettingsViewModel(ObservableCollection<Bookmark> bookmarks, JsonService jsonService)
        {
            _bookmarks = bookmarks;
            _jsonService = jsonService;
            CloseCommand = new RelayCommand<object>(_ => CloseRequested?.Invoke());
        }

        public void SaveBookmark()
        {
            if (string.IsNullOrWhiteSpace(InputTitle) || string.IsNullOrWhiteSpace(InputUrl))
            {
                MessageBox.Show("이름과 URL을 모두 입력해주세요.",
                    "QuickDock", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var url = InputUrl.Trim();
            if (!url.StartsWith("http://") && !url.StartsWith("https://"))
                url = "https://" + url;

            if (_editingBookmark is null)
            {
                // 추가 모드
                if (_bookmarks.Any(b => b.Url.Equals(url, StringComparison.OrdinalIgnoreCase)))
                {
                    var result = MessageBox.Show(
                        "이미 등록된 URL입니다.\n덮어쓰시겠습니까?",
                        "QuickDock", MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if (result != MessageBoxResult.Yes) return;
                }
                _bookmarks.Add(new Bookmark { Title = InputTitle.Trim(), Url = url });
            }
            else
            {
                // 수정 모드 — 프로퍼티 직접 수정 (INotifyPropertyChanged로 UI 자동 갱신)
                _editingBookmark.Title = InputTitle.Trim();
                _editingBookmark.Url = url;
            }

            _jsonService.Save(new System.Collections.Generic.List<Bookmark>(_bookmarks));
            ClearForm();
        }

        public void StartEdit(Bookmark bookmark)
        {
            _editingBookmark = bookmark;
            InputTitle = bookmark.Title;
            InputUrl = bookmark.Url;
            OnPropertyChanged(nameof(SaveButtonText));
        }

        public void DeleteBookmark(Bookmark bookmark)
        {
            var confirm = MessageBox.Show(
                $"'{bookmark.Title}' 을(를) 삭제하시겠습니까?",
                "QuickDock", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (confirm != MessageBoxResult.Yes) return;

            _bookmarks.Remove(bookmark);
            _jsonService.Save(new System.Collections.Generic.List<Bookmark>(_bookmarks));
            if (_editingBookmark == bookmark) ClearForm();
        }

        public void ClearForm()
        {
            _editingBookmark = null;
            InputTitle = string.Empty;
            InputUrl = string.Empty;
            OnPropertyChanged(nameof(SaveButtonText));
        }

        private void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}