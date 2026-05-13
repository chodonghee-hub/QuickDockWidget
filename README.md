# QuickDock

Windows에서 글로벌 단축키(Ctrl+\`)로 즐겨찾기 웹사이트를 빠르게 실행하는 북마크 위젯 앱

![Platform](https://img.shields.io/badge/Platform-Windows-blue)
![Framework](https://img.shields.io/badge/.NET-8.0-purple)
![UI](https://img.shields.io/badge/UI-WPF-blueviolet)

## 주요 기능

- **글로벌 단축키** — Ctrl+\`로 언제 어디서나 위젯 토글
- **플로팅 위젯** — 화면 중앙에 200×340px 반투명 위젯 표시
- **북마크 관리** — 추가 / 수정 / 삭제 / 드래그 앤 드롭 순서 변경
- **로컬 저장** — `%APPDATA%\QuickDock\bookmarks.json` 저장 및 자동 백업
- **시스템 트레이** — 백그라운드 상주, 트레이 메뉴로 기능 접근
- **시작 시 자동 실행** — Windows 레지스트리로 자동 시작 설정

## 스크린샷

| 북마크 위젯 | 설정 창 |
|:-----------:|:-------:|
| *(북마크 목록 표시)* | *(북마크 추가/수정/삭제)* |

## 시스템 요구사항

- Windows 10 (1903 이상) / Windows 11
- [.NET 8.0 Runtime](https://dotnet.microsoft.com/download/dotnet/8.0)

## 빌드 및 실행

```bash
# 저장소 클론
git clone https://github.com/chodonghee-hub/QuickDock.git
cd QuickDock

# 빌드
dotnet build

# 실행
dotnet run
```

또는 Visual Studio 2022에서 `QuickDock.sln`을 열어 실행합니다.

## 사용 방법

1. 앱 실행 시 시스템 트레이에 아이콘이 등록됩니다.
2. **Ctrl+\`** 를 누르면 북마크 위젯이 나타납니다.
3. 북마크를 클릭하면 기본 브라우저로 해당 URL이 열립니다.
4. **Escape** 또는 위젯 외부 클릭으로 위젯을 닫습니다.
5. 위젯 하단 **⚙ 설정** 버튼이나 트레이 메뉴에서 북마크를 관리합니다.

### 기본 제공 북마크

| 이름 | URL |
|------|-----|
| GitHub | https://github.com |
| ChatGPT | https://chatgpt.com |
| Notion | https://notion.so |
| Figma | https://figma.com |
| YouTube | https://youtube.com |
| Discord | https://discord.com |

## 프로젝트 구조

```
QuickDock/
├── Models/
│   └── Bookmark.cs              # 북마크 데이터 모델
├── ViewModels/
│   ├── MainViewModel.cs         # 메인 위젯 ViewModel
│   ├── SettingViewModel.cs      # 설정 창 ViewModel
│   └── RelayCommand.cs          # ICommand 구현체
├── Views/
│   ├── MainWindow.xaml          # 북마크 위젯 UI
│   └── SettingsWindow.xaml      # 설정 창 UI
├── Services/
│   ├── BrowserService.cs        # URL 검증 및 브라우저 실행
│   ├── HotKeyService.cs         # Win32 글로벌 단축키 등록
│   ├── JsonService.cs           # JSON 저장/로드 (자동 백업)
│   └── TrayService.cs           # 시스템 트레이 관리
├── Converters/
│   ├── FirstLetterConverter.cs  # 북마크 첫 글자 추출
│   └── TitleColorConverter.cs   # 첫 글자 기반 배경색 결정
└── PRD/
    └── bookmark_widget_project_plan.md  # 프로젝트 기획서
```

## 기술 스택

| 항목 | 기술 |
|------|------|
| 런타임 | .NET 8.0 |
| UI 프레임워크 | WPF |
| 아키텍처 패턴 | MVVM |
| 직렬화 | System.Text.Json |
| 트레이 아이콘 | Hardcodet.NotifyIcon.Wpf 2.0.1 |
| 드래그 앤 드롭 | gong-wpf-dragdrop 4.0.0 |
| 시스템 연동 | Win32 API (P/Invoke) |

## 아키텍처

MVVM 패턴으로 구성되며, 각 레이어는 다음 역할을 담당합니다.

```
View (XAML) ──바인딩──> ViewModel ──명령──> Service ──조작──> Model
```

- **View** — XAML로 작성된 UI, ViewModel과 데이터 바인딩
- **ViewModel** — UI 로직 및 상태 관리, INotifyPropertyChanged 구현
- **Service** — 단축키, 브라우저 실행, JSON 저장, 트레이 등 기능 담당
- **Model** — `Bookmark` 데이터 구조 (Title, Url, IconPath, Group)

## 보안

- 허용 URL 스킴: `http://`, `https://` 만 허용
- `javascript:`, `file:`, `data:` 등 위험 스킴 차단

## 라이선스

이 프로젝트는 개인 학습 목적으로 제작되었습니다.
