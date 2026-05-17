# QuickDock

Windows에서 글로벌 단축키(Ctrl+\`)로 즐겨찾기 웹사이트를 빠르게 실행하는 북마크 위젯 앱

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

- **글로벌 단축키** — Ctrl+\`로 언제 어디서나 위젯 토글
- **플로팅 위젯** — 화면 중앙에 반투명 위젯 표시
- **북마크 관리** — 추가 / 수정 / 삭제 / 드래그 앤 드롭 순서 변경
- **로컬 저장** — `%APPDATA%\QuickDock\bookmarks.json` 저장 및 자동 백업
- **시스템 트레이** — 백그라운드 상주, 트레이 메뉴로 기능 접근
- **시작 시 자동 실행** — Windows 레지스트리로 자동 시작 설정

## 스크린샷

|     북마크 위젯      |          설정 창          |
| :------------------: | :-----------------------: |
| _(북마크 목록 표시)_ | _(북마크 추가/수정/삭제)_ |

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

| 이름    | URL                 |
| ------- | ------------------- |
| GitHub  | https://github.com  |
| ChatGPT | https://chatgpt.com |
| Notion  | https://notion.so   |
| Figma   | https://figma.com   |
| YouTube | https://youtube.com |
| Discord | https://discord.com |

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

## 프로젝트 구조

```
QuickDock/
├── Models/
│   └── Bookmark.cs                  # 북마크 데이터 모델
├── ViewModels/
│   ├── MainViewModel.cs             # 메인 위젯 ViewModel
│   ├── SettingViewModel.cs          # 설정 창 ViewModel
│   └── RelayCommand.cs              # ICommand 구현체
├── Views/
│   ├── MainWindow.xaml              # 북마크 위젯 UI
│   ├── SettingsWindow.xaml          # 설정 창 UI
│   └── AddBookmarkWindow.xaml       # 북마크 추가 창 UI
├── Services/
│   ├── BrowserService.cs            # URL 검증 및 브라우저 실행
│   ├── HotKeyService.cs             # Win32 글로벌 단축키 등록
│   ├── JsonService.cs               # JSON 저장/로드 (자동 백업)
│   └── TrayService.cs               # 시스템 트레이 관리
├── Converters/
│   ├── FirstLetterConverter.cs      # 북마크 첫 글자 추출
│   └── TitleColorConverter.cs       # 첫 글자 기반 배경색 결정
├── publish.ps1                      # 배포 빌드 스크립트
└── PRD/
    └── bookmark_widget_project_plan.md  # 프로젝트 기획서
```

## 기술 스택

| 항목           | 기술                           |
| -------------- | ------------------------------ |
| 런타임         | .NET 8.0                       |
| UI 프레임워크  | WPF                            |
| 아키텍처 패턴  | MVVM                           |
| 직렬화         | System.Text.Json               |
| 트레이 아이콘  | Hardcodet.NotifyIcon.Wpf 2.0.1 |
| 드래그 앤 드롭 | gong-wpf-dragdrop 4.0.0        |
| 시스템 연동    | Win32 API (P/Invoke)           |

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

## 라이센스

이 프로젝트는 개인 학습 목적으로 제작되었습니다.
