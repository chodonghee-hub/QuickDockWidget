# QuickDock

Windows에서 글로벌 단축키(기본: Ctrl+\`)로 즐겨찾기 웹사이트를 빠르게 실행하는 북마크 위젯 앱

![Platform](https://img.shields.io/badge/Platform-Windows-blue)
![Framework](https://img.shields.io/badge/.NET-8.0-purple)
![UI](https://img.shields.io/badge/UI-WPF-blueviolet)
[![GitHub Release](https://img.shields.io/github/v/release/chodonghee-hub/QuickDockWidget)](https://github.com/chodonghee-hub/QuickDockWidget/releases/latest)
[![GitHub Downloads](https://img.shields.io/github/downloads/chodonghee-hub/QuickDockWidget/total)](https://github.com/chodonghee-hub/QuickDockWidget/releases)

## 다운로드 및 설치

> **별도 설치 없이 바로 실행 가능합니다** (.NET 런타임 포함 빌드)

**[최신 버전 다운로드 →](https://github.com/chodonghee-hub/QuickDockWidget/releases/latest)**

### 1단계 — 다운로드 및 실행

1. `QuickDock-vX.X.X-win-x64.zip` 파일 다운로드
2. **영구적으로 사용할 폴더**에 압축 해제 (나중에 폴더를 옮기거나 삭제하면 시작프로그램 등록이 깨질 수 있음)
   - 권장 경로: `C:\Users\사용자이름\AppData\Local\QuickDock\` 또는 `C:\Program Files\QuickDock\`
3. `QuickDock.exe` 더블클릭으로 실행

> Windows SmartScreen 경고가 뜨면 **"추가 정보" → "실행"** 클릭

### 2단계 — Windows 시작 시 자동 실행 설정

#### 방법 1 — `--install` 플래그 (권장)

압축 해제 후 PowerShell 또는 명령 프롬프트에서 아래 명령어를 한 번만 실행하면 시작프로그램 등록과 앱 실행이 동시에 이뤄집니다.

```powershell
# 압축 해제 폴더로 이동 후 실행
.\QuickDock.exe --install
```

이후 Windows 재시작 시 QuickDock이 자동으로 실행됩니다.

> **바탕화면 바로가기로 설정하는 방법:** 바로가기 속성 → 대상(Target) 항목 끝에 `--install` 추가 → 더블클릭 한 번으로 설치 완료

#### 방법 2 — 트레이 메뉴에서 설정

앱을 실행하면 시스템 트레이(우측 하단 알림 영역)에 QuickDock 아이콘이 생깁니다.

1. 화면 우측 하단 트레이 아이콘 우클릭
2. **"시작 시 자동 실행"** 항목 클릭 (체크 표시되면 활성화됨)
3. 이제 Windows 로그인 시 QuickDock이 자동으로 백그라운드에서 실행됩니다.

> 내부적으로 `HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Run` 레지스트리에 등록됩니다. 관리자 권한 없이도 설정 가능합니다.

---

## 주요 기능

- **글로벌 단축키** — Ctrl+\`로 언제 어디서나 위젯 토글 (설정에서 다른 키 조합으로 변경 가능)
- **플로팅 위젯** — 화면 중앙에 반투명 위젯 표시, 외부 클릭 또는 Escape로 닫기
- **페이지네이션** — 북마크를 6개씩 나눠 표시, 이전/다음 페이지 버튼으로 탐색
- **북마크 관리** — 설정 창에서 추가 / 인라인 편집 / 삭제 / 드래그 앤 드롭 순서 변경
- **아이콘 색상 선택** — 9가지 색상 팔레트에서 북마크 아이콘 색상 지정
- **로컬 저장** — `%APPDATA%\QuickDock\bookmarks.json` 저장 및 자동 백업
- **시스템 트레이** — 백그라운드 상주, 트레이 메뉴로 기능 접근
- **시작 시 자동 실행** — Windows 레지스트리로 자동 시작 설정

## 스크린샷

|     북마크 위젯      |     수정/설정 창     |     추가 창     |
| :------------------: | :------------------: | :-------------: |
| _(북마크 목록 표시)_ | _(북마크 수정/삭제)_ | _(북마크 추가)_ |

## 시스템 요구사항

- Windows 10 (1903 이상) / Windows 11
- .NET 8.0 런타임 — **다운로드 버전은 포함되어 있어 별도 설치 불필요**

## 사용 방법

1. 앱 실행 시 시스템 트레이에 아이콘이 등록됩니다.
2. **Ctrl+\`** 를 누르면 북마크 위젯이 나타납니다.
3. 북마크를 클릭하면 기본 브라우저로 해당 URL이 열립니다.
4. **Escape** 또는 위젯 외부 클릭으로 위젯을 닫습니다.
5. 위젯 하단 **⚙ 설정** 버튼이나 트레이 메뉴에서 북마크를 관리합니다.

### 기본 제공 북마크

저장된 북마크가 없을 때 아래 7개가 자동으로 생성됩니다.

| 이름     | URL                     |
| -------- | ----------------------- |
| GitHub   | https://github.com      |
| Notion   | https://notion.so       |
| Figma    | https://figma.com       |
| YouTube  | https://youtube.com     |
| ChatGPT  | https://chat.openai.com |
| Discord  | https://discord.com     |
| Supabase | https://supabase.com    |

## 빌드 및 실행

```bash
# 저장소 클론
git clone https://github.com/chodonghee-hub/QuickDockWidget.git
cd QuickDock

# 빌드
dotnet build

# 실행
dotnet run
```

또는 Visual Studio 2022에서 `QuickDock.sln`을 열어 실행합니다.

### 배포 빌드

```powershell
# 단일 exe 생성 (self-contained, ~85MB)
.\publish.ps1 -Version "1.0.1"

# .NET 런타임이 설치된 환경용 경량 빌드 (~5~10MB)
.\publish.ps1 -Version "1.0.1" -SelfContained $false
```

빌드 결과물은 `dist/QuickDock-vX.X.X-win-x64.zip`으로 생성됩니다.

> **csproj 배포 설정:** Release 구성에서 `PublishSingleFile`, `SelfContained`, `PublishReadyToRun`, `IncludeNativeLibrariesForSelfExtract`가 자동 적용됩니다.

## 프로젝트 구조

```
QuickDock/
├── Models/
│   ├── Bookmark.cs                  # 북마크 데이터 모델 (INotifyPropertyChanged 구현)
│   └── HotkeyConfig.cs              # 단축키 설정 저장 모델 (Modifiers, Vk)
├── ViewModels/
│   ├── MainViewModel.cs             # 메인 위젯 ViewModel (페이지네이션, 북마크 로드)
│   ├── SettingViewModel.cs          # 설정 창 ViewModel (인라인 편집, 단축키 캡처)
│   ├── AddBookmarkViewModel.cs      # 북마크 추가 창 ViewModel (URL 검증, 색상 선택)
│   └── RelayCommand.cs              # ICommand 구현체 (제네릭 지원)
├── Views/
│   ├── MainWindow.xaml              # 북마크 위젯 UI (6슬롯 그리드, 페이지 도트)
│   ├── SettingsWindow.xaml          # 설정 창 UI (목록 + 인라인 편집 폼 + 단축키 섹션)
│   └── AddBookmarkWindow.xaml       # 북마크 추가 창 UI
├── Services/
│   ├── BrowserService.cs            # URL 검증 및 브라우저 실행 (OpenResult 열거형 반환)
│   ├── HotKeyService.cs             # Win32 글로벌 단축키 등록/변경/저장 (IDisposable)
│   ├── JsonService.cs               # JSON 저장/로드 (SchemaVersion 래퍼, 자동 백업)
│   └── TrayService.cs               # 시스템 트레이 관리 (알림, 자동 실행 토글)
├── Converters/
│   ├── FirstLetterConverter.cs      # 북마크 제목 첫 글자 추출 (아이콘 텍스트용)
│   ├── TitleColorConverter.cs       # 사이트명 기반 아이콘 배경색 결정
│   ├── ColorMatchConverter.cs       # 색상 팔레트 선택 상태 비교 (IMultiValueConverter)
│   └── UrlDisplayConverter.cs       # URL 표시 정제 (https://, www. 제거)
├── publish.ps1                      # 배포 빌드 스크립트
└── PRD/
    └── bookmark_widget_project_plan.md  # 프로젝트 기획서
```

## 기술 스택

| 항목           | 기술                           | 역할                                                      |
| -------------- | ------------------------------ | --------------------------------------------------------- |
| 런타임         | .NET 8.0 (LTS)                 | Windows 네이티브 API 지원 및 self-contained 단일 exe 배포 |
| UI 프레임워크  | WPF (XAML)                     | 투명 창·커스텀 스타일 UI 구성 및 MVVM 데이터 바인딩       |
| 아키텍처 패턴  | MVVM                           | View와 로직 분리, 데이터 바인딩으로 코드비하인드 최소화   |
| 직렬화         | System.Text.Json               | 북마크 목록을 JSON 파일로 저장·로드                       |
| 트레이 아이콘  | Hardcodet.NotifyIcon.Wpf 2.0.1 | 시스템 트레이 아이콘, 풍선 알림, 컨텍스트 메뉴 구성       |
| 드래그 앤 드롭 | gong-wpf-dragdrop 4.0.0        | 설정 창 북마크 목록의 드래그 앤 드롭 순서 변경            |
| 시스템 연동    | Win32 API (P/Invoke)           | `RegisterHotKey`로 OS 수준 글로벌 단축키 등록             |

## 아키텍처

MVVM 패턴으로 구성되며, 각 레이어는 다음 역할을 담당합니다.

```
View (XAML) ──바인딩──> ViewModel ──명령──> Service ──조작──> Model
```

- **View** — XAML로 작성된 UI, ViewModel과 데이터 바인딩
- **ViewModel** — UI 로직 및 상태 관리, `INotifyPropertyChanged` 구현
- **Service** — 단축키, 브라우저 실행, JSON 저장, 트레이 등 기능 담당
- **Model** — `Bookmark`(Title, Url, IconPath, Group, Index), `HotkeyConfig`(Modifiers, Vk)

## 주요 기술 상세

### 글로벌 단축키 (`HotKeyService`)

Win32 `RegisterHotKey` / `UnregisterHotKey` API를 P/Invoke로 호출합니다. 앱의 숨겨진 창 핸들(`HwndSource`)에 `WndProc` 훅을 걸어 `WM_HOTKEY (0x0312)` 메시지를 감지합니다. `MOD_NOREPEAT (0x4000)` 플래그를 함께 등록해 키를 누른 채로 있을 때 반복 발생을 방지합니다.

단축키는 `%APPDATA%\QuickDock\hotkey.json`에 저장되어 재시작 후에도 유지됩니다. 등록 실패 시 `HotkeyConflicted` 이벤트를 발생시켜 다른 앱과의 충돌을 알립니다.

```
RegisterHotKey(hWnd, HOTKEY_ID, MOD_CTRL | MOD_NOREPEAT, 0xC0 /* ` */)
```

설정 창의 **Change** 버튼을 누르면 캡처 모드로 진입하며, 이 동안 현재 단축키를 임시 해제(`TemporaryUnregister`)하여 입력 감지가 가능해집니다. **Save** 클릭 시 새 조합을 등록하고, 실패하면 기존 단축키를 복구합니다.

### 데이터 저장 (`JsonService`)

북마크는 `SchemaVersion` 필드를 포함한 래퍼 객체에 감싸서 JSON으로 저장합니다. 이 필드는 추후 데이터 형식 변경 시 마이그레이션에 활용할 수 있습니다.

```json
{
  "SchemaVersion": 1,
  "Bookmarks": [
    {
      "Title": "GitHub",
      "Url": "https://github.com",
      "IconPath": "#1A1A1A",
      "Group": ""
    }
  ]
}
```

저장 시 메인 파일(`bookmarks.json`)과 백업 파일(`bookmarks.backup.json`)을 동시에 갱신합니다. 로드 시 메인 파일이 손상됐거나 없으면 백업 파일로 자동 복구합니다.

### 페이지네이션 (`MainViewModel`)

북마크를 6개씩 한 페이지로 나눠 표시합니다. `PagedBookmarks`는 6개 슬롯 고정 크기의 `ObservableCollection<Bookmark?>`로, 페이지 이동 시 Replace 알림만 발생시켜 컨테이너를 재생성하지 않습니다. 빈 슬롯은 `null`로 채워집니다.

### URL 보안 (`BrowserService`)

`http://` 및 `https://` 스킴만 허용합니다. `Uri.TryCreate`로 파싱 후 스킴을 검사하여 `javascript:`, `file:`, `data:` 등 위험 스킴을 차단합니다. 실행 결과를 `OpenResult` 열거형(`Success`, `InvalidUrl`, `BlockedScheme`, `LaunchFailed`)으로 반환해 ViewModel에서 사용자에게 구체적인 오류 메시지를 표시합니다.

### 설정 창 인라인 편집

별도의 팝업 창 없이 `SettingsWindow` 하단 고정 영역에서 이름·URL·아이콘 색상을 직접 편집합니다. 북마크 목록에서 항목을 클릭하면 해당 데이터가 폼에 로드되며, 저장 시 `ObservableCollection`의 해당 객체를 직접 수정합니다. 중복 URL 추가 시 덮어쓸지 여부를 사용자에게 확인합니다.

## 보안

- 허용 URL 스킴: `http://`, `https://` 만 허용
- `javascript:`, `file:`, `data:` 등 위험 스킴 차단
- 레지스트리 자동 시작 등록: `HKCU\...\Run` (관리자 권한 불필요)

## 라이센스

이 프로젝트는 개인 학습 목적으로 제작되었습니다.
