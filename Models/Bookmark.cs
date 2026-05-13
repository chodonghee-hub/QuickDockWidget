using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace QuickDock.Models
{
    public class Bookmark : INotifyPropertyChanged
    {
        private string _title = string.Empty;
        private string _url = string.Empty;
        private string _iconPath = string.Empty;
        private string _group = string.Empty;
        private int _index;

        public string Title
        {
            get => _title;
            set { _title = value; OnPropertyChanged(); }
        }

        public string Url
        {
            get => _url;
            set { _url = value; OnPropertyChanged(); }
        }

        public string IconPath
        {
            get => _iconPath;
            set { _iconPath = value; OnPropertyChanged(); }
        }

        public string Group
        {
            get => _group;
            set { _group = value; OnPropertyChanged(); }
        }

        [System.Text.Json.Serialization.JsonIgnore]
        public int Index
        {
            get => _index;
            set { _index = value; OnPropertyChanged(); }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}