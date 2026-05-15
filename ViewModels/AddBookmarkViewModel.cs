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

namespace QuickDock.ViewModels
{
    public class AddBookmarkViewModel : INotifyPropertyChanged
    {
        private readonly ObservableCollection<Bookmark> _bookmarks;
        private readonly JsonService _jsonService;

        private string _inputTitle    = string.Empty;
        private string _inputUrl      = string.Empty;
        private string _inputGroup    = string.Empty;
        private bool   _isUrlValid;
        private string _selectedIconColor;
        private bool   _isAutoFavicon;

        public static readonly IReadOnlyList<string> IconColors = new[]
        {
            "#3B82F6", "#1A1A1A", "#F97316", "#14B8A6",
            "#60A5FA", "#EF4444", "#84CC16", "#8B5CF6", "#6366F1"
        };

        public string InputTitle
        {
            get => _inputTitle;
            set
            {
                if (value.Length > 40) value = value[..40];
                _inputTitle = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(TitleCharCount));
            }
        }

        public string TitleCharCount => $"{_inputTitle.Length}/40";

        public string InputUrl
        {
            get => _inputUrl;
            set
            {
                // 붙여넣기 시 prefix 자동 제거
                if (value.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
                    value = value["https://".Length..];
                else if (value.StartsWith("http://", StringComparison.OrdinalIgnoreCase))
                    value = value["http://".Length..];

                _inputUrl = value;
                OnPropertyChanged();
                ValidateUrl();
            }
        }

        public bool IsUrlValid
        {
            get => _isUrlValid;
            private set { _isUrlValid = value; OnPropertyChanged(); OnPropertyChanged(nameof(UrlValidationText)); }
        }

        public string UrlValidationText => _isUrlValid ? "✓ valid" : string.Empty;

        public string InputGroup
        {
            get => _inputGroup;
            set { _inputGroup = value; OnPropertyChanged(); }
        }

        public string SelectedIconColor
        {
            get => _selectedIconColor;
            set { _selectedIconColor = value; OnPropertyChanged(); }
        }

        public bool IsAutoFavicon
        {
            get => _isAutoFavicon;
            set { _isAutoFavicon = value; OnPropertyChanged(); }
        }

        public ICommand SaveCommand        { get; }
        public ICommand CancelCommand      { get; }
        public ICommand SelectIconCommand  { get; }

        public event Action?                    CloseRequested;
        public event PropertyChangedEventHandler? PropertyChanged;

        public AddBookmarkViewModel(
            ObservableCollection<Bookmark> bookmarks,
            JsonService jsonService)
        {
            _bookmarks    = bookmarks;
            _jsonService  = jsonService;
            _selectedIconColor = IconColors[0];

            SaveCommand       = new RelayCommand<object>(_ => Save());
            CancelCommand     = new RelayCommand<object>(_ => CloseRequested?.Invoke());
            SelectIconCommand = new RelayCommand<string>(color =>
            {
                if (color is not null) SelectedIconColor = color;
            });
        }

        private void ValidateUrl()
        {
            var full = "https://" + _inputUrl.Trim();
            IsUrlValid = !string.IsNullOrWhiteSpace(_inputUrl)
                         && Uri.TryCreate(full, UriKind.Absolute, out var uri)
                         && (uri.Scheme == "https" || uri.Scheme == "http");
        }

        private void Save()
        {
            if (string.IsNullOrWhiteSpace(InputTitle))
            {
                MessageBox.Show("이름을 입력해주세요.", "QuickDock",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!IsUrlValid)
            {
                MessageBox.Show("올바른 URL을 입력해주세요.\n(예: github.com)",
                    "QuickDock", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var url = "https://" + InputUrl.Trim();

            if (_bookmarks.Any(b => b.Url.Equals(url, StringComparison.OrdinalIgnoreCase)))
            {
                var result = MessageBox.Show(
                    "이미 등록된 URL입니다.\n덮어쓰시겠습니까?",
                    "QuickDock", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result != MessageBoxResult.Yes) return;
            }

            _bookmarks.Add(new Bookmark
            {
                Title    = InputTitle.Trim(),
                Url      = url,
                IconPath = IsAutoFavicon ? "auto" : SelectedIconColor,
                Group    = InputGroup.Trim()
            });

            _jsonService.Save(new System.Collections.Generic.List<Bookmark>(_bookmarks));
            CloseRequested?.Invoke();
        }

        private void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
