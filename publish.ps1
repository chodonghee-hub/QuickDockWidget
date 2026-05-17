param(
    [string]$Version = "1.0.0",
    [bool]$SelfContained = $true
)

$projectFile = "QuickDock.csproj"
$distDir = ".\dist"
$outputDir = "$distDir\QuickDock-v$Version"
$zipPath = "$distDir\QuickDock-v$Version-win-x64.zip"

Write-Host "=== QuickDock v$Version 배포 시작 ===" -ForegroundColor Cyan

if (Test-Path $outputDir) {
    Remove-Item $outputDir -Recurse -Force
}
New-Item -ItemType Directory -Force -Path $outputDir | Out-Null

$publishArgs = @(
    "publish", $projectFile,
    "-c", "Release",
    "-r", "win-x64",
    "--self-contained", $SelfContained.ToString().ToLower(),
    "-p:PublishSingleFile=true",
    "-p:IncludeNativeLibrariesForSelfExtract=true",
    "-o", $outputDir
)

Write-Host "빌드 중..." -ForegroundColor Yellow
dotnet @publishArgs

if ($LASTEXITCODE -ne 0) {
    Write-Host "빌드 실패!" -ForegroundColor Red
    exit 1
}

if (Test-Path $zipPath) {
    Remove-Item $zipPath -Force
}

Write-Host "압축 중: $zipPath" -ForegroundColor Yellow
Compress-Archive -Path "$outputDir\*" -DestinationPath $zipPath -Force

$sizeMB = [math]::Round((Get-Item $zipPath).Length / 1MB, 1)
Write-Host ""
Write-Host "=== 배포 완료 ===" -ForegroundColor Green
Write-Host "파일: $zipPath ($sizeMB MB)"