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
        private readonly JsonService    _jsonService;
        private readonly HotkeyService  _hotkeyService;
        private readonly ObservableCollection<Bookmark> _bookmarks;

        // ── 북마크 폼 ──────────────────────────────────────
        private Bookmark? _editingBookmark;
        private string    _inputTitle     = string.Empty;
        private string    _inputUrl       = string.Empty;
        private string    _inputIconColor;

        public static readonly IReadOnlyList<string> IconColors = new[]
        {
            "#3B82F6", "#1A1A1A", "#F97316", "#14B8A6",
            "#60A5FA", "#EF4444", "#84CC16", "#8B5CF6", "#6366F1"
        };

        // ── 단축키 캡처 ────────────────────────────────────
        private bool   _isCapturingHotkey;
        private uint   _pendingModifiers;   // 0 = 미입력
        private uint   _pendingVk;
        private string _hotkeyModifierText = string.Empty;
        private string _hotkeyKeyText      = string.Empty;
        private string _hotkeyStatusText   = "currently active";

        // Win32 modifier 상수 (MOD_NOREPEAT 제외)
        private const uint MOD_CTRL  = 0x0002;
        private const uint MOD_ALT   = 0x0001;
        private const uint MOD_SHIFT = 0x0004;
        private const uint MOD_WIN   = 0x0008;

        // ── 공개 프로퍼티: 북마크 ──────────────────────────
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

        public string InputIconColor
        {
            get => _inputIconColor;
            set { _inputIconColor = value; OnPropertyChanged(); }
        }

        public ICommand SelectIconColorCommand { get; }

        public string SaveButtonText => "저장";
        public bool IsEditMode => _editingBookmark is not null;

        // ── 공개 프로퍼티: 단축키 ──────────────────────────
        public bool IsCapturingHotkey
        {
            get => _isCapturingHotkey;
            private set { _isCapturingHotkey = value; OnPropertyChanged(); OnPropertyChanged(nameof(HotkeyButtonText)); }
        }

        public string HotkeyButtonText => _isCapturingHotkey ? "Save" : "Change";

        public string HotkeyModifierText
        {
            get => _hotkeyModifierText;
            private set { _hotkeyModifierText = value; OnPropertyChanged(); }
        }

        public string HotkeyKeyText
        {
            get => _hotkeyKeyText;
            private set { _hotkeyKeyText = value; OnPropertyChanged(); }
        }

        public string HotkeyStatusText
        {
            get => _hotkeyStatusText;
            private set { _hotkeyStatusText = value; OnPropertyChanged(); }
        }

        // ── 이벤트 ─────────────────────────────────────────
        public event Action? CloseRequested;
        public event PropertyChangedEventHandler? PropertyChanged;

        public ICommand CloseCommand { get; }

        // ── 생성자 ─────────────────────────────────────────
        public SettingsViewModel(
            ObservableCollection<Bookmark> bookmarks,
            JsonService jsonService,
            HotkeyService hotkeyService)
        {
            _bookmarks      = bookmarks;
            _jsonService    = jsonService;
            _hotkeyService  = hotkeyService;
            _inputIconColor = IconColors[0];
            CloseCommand    = new RelayCommand<object>(_ => CloseRequested?.Invoke());
            SelectIconColorCommand = new RelayCommand<string>(color =>
            {
                if (color is not null) InputIconColor = color;
            });

            RefreshHotkeyDisplay();
        }

        // ── 북마크 메서드 ──────────────────────────────────
        public bool SaveBookmark()
        {
            if (string.IsNullOrWhiteSpace(InputTitle) || string.IsNullOrWhiteSpace(InputUrl))
            {
                MessageBox.Show("이름과 URL을 모두 입력해주세요.",
                    "QuickDock", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            var url = InputUrl.Trim();
            if (!url.StartsWith("http://") && !url.StartsWith("https://"))
                url = "https://" + url;

            if (_editingBookmark is null)
            {
                if (_bookmarks.Any(b => b.Url.Equals(url, StringComparison.OrdinalIgnoreCase)))
                {
                    var result = MessageBox.Show(
                        "이미 등록된 URL입니다.\n덮어쓰시겠습니까?",
                        "QuickDock", MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if (result != MessageBoxResult.Yes) return false;
                }
                _bookmarks.Add(new Bookmark { Title = InputTitle.Trim(), Url = url, IconPath = InputIconColor });
            }
            else
            {
                _editingBookmark.Title    = InputTitle.Trim();
                _editingBookmark.Url      = url;
                _editingBookmark.IconPath = InputIconColor;
            }

            _jsonService.Save(new System.Collections.Generic.List<Bookmark>(_bookmarks));
            ClearForm();
            return true;
        }

        public void StartEdit(Bookmark bookmark)
        {
            _editingBookmark = bookmark;
            InputTitle     = bookmark.Title;
            InputUrl       = bookmark.Url;
            InputIconColor = IconColors.Contains(bookmark.IconPath) ? bookmark.IconPath : IconColors[0];
            OnPropertyChanged(nameof(SaveButtonText));
            OnPropertyChanged(nameof(IsEditMode));
        }

        public void DeleteBookmark(Bookmark bookmark)
        {
            _bookmarks.Remove(bookmark);
            _jsonService.Save(new System.Collections.Generic.List<Bookmark>(_bookmarks));
            if (_editingBookmark == bookmark) ClearForm();
        }

        public void ClearForm()
        {
            _editingBookmark = null;
            InputTitle     = string.Empty;
            InputUrl       = string.Empty;
            InputIconColor = IconColors[0];
            OnPropertyChanged(nameof(SaveButtonText));
            OnPropertyChanged(nameof(IsEditMode));
        }

        // ── 단축키 캡처 메서드 ────────────────────────────
        public void ToggleCaptureMode()
        {
            if (!_isCapturingHotkey)
            {
                // Change 클릭 → 캡처 모드 진입
                _pendingModifiers = 0;
                _pendingVk        = 0;
                _hotkeyService.TemporaryUnregister();
                IsCapturingHotkey  = true;
                HotkeyStatusText   = "키를 입력하세요...";
                // 배지를 비워 입력 대기 표시
                HotkeyModifierText = "?";
                HotkeyKeyText      = "?";
            }
            else
            {
                // Save 클릭 → 저장 또는 원복
                if (_pendingVk == 0)
                {
                    // 아무 키도 입력하지 않음 → 원래 단축키 재등록
                    _hotkeyService.ChangeHotkey(
                        _hotkeyService.CurrentModifiers & ~0x4000u,
                        _hotkeyService.CurrentVk);
                    ExitCaptureMode();
                    return;
                }

                // 현재 등록된 단축키와 동일한지 확인 (중복 방지)
                uint newFullMods = _pendingModifiers | 0x4000u; // MOD_NOREPEAT 포함
                if (newFullMods == _hotkeyService.CurrentModifiers &&
                    _pendingVk  == _hotkeyService.CurrentVk)
                {
                    MessageBox.Show(
                        "이미 사용 중인 단축키입니다.\n다른 키 조합을 입력해주세요.",
                        "QuickDock", MessageBoxButton.OK, MessageBoxImage.Warning);
                    // 원래 단축키 재등록 후 캡처 종료
                    _hotkeyService.ChangeHotkey(
                        _hotkeyService.CurrentModifiers & ~0x4000u,
                        _hotkeyService.CurrentVk);
                    ExitCaptureMode();
                    return;
                }

                _hotkeyService.ChangeHotkey(_pendingModifiers, _pendingVk);
                ExitCaptureMode();
            }
        }

        public void UpdatePendingHotkey(ModifierKeys modifiers, Key key)
        {
            // 조건 1: modifier 필수
            if (modifiers == ModifierKeys.None) return;

            uint newMods = ConvertModifiers(modifiers);
            uint newVk   = (uint)KeyInterop.VirtualKeyFromKey(key);

            // 조건 2: 이미 같은 조합이 입력됨 → 중복 무시
            if (newVk == _pendingVk && newMods == _pendingModifiers && _pendingVk != 0) return;

            _pendingModifiers  = newMods;
            _pendingVk         = newVk;
            HotkeyModifierText = GetModifierText(newMods);
            HotkeyKeyText      = GetKeyText(key);
        }

        // ── private 헬퍼 ──────────────────────────────────
        private void ExitCaptureMode()
        {
            IsCapturingHotkey = false;
            RefreshHotkeyDisplay();
        }

        private void RefreshHotkeyDisplay()
        {
            uint mods = _hotkeyService.CurrentModifiers & ~0x4000u; // MOD_NOREPEAT 제거
            HotkeyModifierText = GetModifierText(mods);
            HotkeyKeyText      = GetKeyDisplayFromVk(_hotkeyService.CurrentVk);
            HotkeyStatusText   = "currently active";
        }

        private static uint ConvertModifiers(ModifierKeys mods)
        {
            uint result = 0;
            if (mods.HasFlag(ModifierKeys.Control)) result |= MOD_CTRL;
            if (mods.HasFlag(ModifierKeys.Alt))     result |= MOD_ALT;
            if (mods.HasFlag(ModifierKeys.Shift))   result |= MOD_SHIFT;
            if (mods.HasFlag(ModifierKeys.Windows)) result |= MOD_WIN;
            return result;
        }

        private static string GetModifierText(uint mods)
        {
            var parts = new System.Collections.Generic.List<string>();
            if ((mods & MOD_CTRL)  != 0) parts.Add("Ctrl");
            if ((mods & MOD_ALT)   != 0) parts.Add("Alt");
            if ((mods & MOD_SHIFT) != 0) parts.Add("Shift");
            if ((mods & MOD_WIN)   != 0) parts.Add("Win");
            return parts.Count > 0 ? string.Join("+", parts) : "?";
        }

        private static string GetKeyText(Key key) => key switch
        {
            Key.Oem3       => "`",
            Key.Space      => "Space",
            Key.Return     => "Enter",
            Key.Tab        => "Tab",
            Key.Escape     => "Esc",
            Key.Back       => "Back",
            Key.Delete     => "Del",
            Key.Insert     => "Ins",
            Key.Home       => "Home",
            Key.End        => "End",
            Key.PageUp     => "PgUp",
            Key.PageDown   => "PgDn",
            Key.Up         => "↑",
            Key.Down       => "↓",
            Key.Left       => "←",
            Key.Right      => "→",
            Key.OemMinus   => "-",
            Key.OemPlus    => "=",
            Key.OemOpenBrackets => "[",
            Key.OemCloseBrackets => "]",
            Key.OemPipe    => "\\",
            Key.OemSemicolon => ";",
            Key.OemQuotes  => "'",
            Key.OemComma   => ",",
            Key.OemPeriod  => ".",
            Key.OemQuestion => "/",
            _              => key.ToString()
        };

        private static string GetKeyDisplayFromVk(uint vk)
        {
            if (vk >= 0x70 && vk <= 0x87) return $"F{vk - 0x6F}"; // F1–F24
            if (vk >= 0x30 && vk <= 0x39) return ((char)vk).ToString(); // 0–9
            if (vk >= 0x41 && vk <= 0x5A) return ((char)vk).ToString(); // A–Z

            return vk switch
            {
                0xC0 => "`",
                0x20 => "Space",
                0x0D => "Enter",
                0x09 => "Tab",
                0x1B => "Esc",
                0x08 => "Back",
                0x2E => "Del",
                0x2D => "Ins",
                0x24 => "Home",
                0x23 => "End",
                0x21 => "PgUp",
                0x22 => "PgDn",
                0x26 => "↑",
                0x28 => "↓",
                0x25 => "←",
                0x27 => "→",
                _    => $"0x{vk:X2}"
            };
        }

        private void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
