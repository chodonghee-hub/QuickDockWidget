using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using QuickDock.Models;

namespace QuickDock.Services
{
    public class JsonService
    {
        private static readonly string AppFolder = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "QuickDock");

        private static readonly string MainFile = Path.Combine(AppFolder, "bookmarks.json");
        private static readonly string BackupFile = Path.Combine(AppFolder, "bookmarks.backup.json");
        private static readonly string SettingsFile = Path.Combine(AppFolder, "settings.json");

        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            WriteIndented = true
        };

        private class AppSettings
        {
            public double? WindowLeft { get; set; }
            public double? WindowTop  { get; set; }
        }

        public void SaveWindowPosition(double left, double top)
        {
            try
            {
                Directory.CreateDirectory(AppFolder);
                var settings = new AppSettings { WindowLeft = left, WindowTop = top };
                File.WriteAllText(SettingsFile, JsonSerializer.Serialize(settings, JsonOptions));
            }
            catch { }
        }

        public (double Left, double Top)? LoadWindowPosition()
        {
            try
            {
                if (!File.Exists(SettingsFile)) return null;
                var settings = JsonSerializer.Deserialize<AppSettings>(File.ReadAllText(SettingsFile));
                if (settings?.WindowLeft is double l && settings.WindowTop is double t)
                    return (l, t);
            }
            catch { }
            return null;
        }

        // 저장 데이터 래퍼 (스키마 버전 포함)
        private class BookmarkData
        {
            public int SchemaVersion { get; set; } = 1;
            public List<Bookmark> Bookmarks { get; set; } = new();
        }

        public List<Bookmark> Load()
        {
            // 1. 메인 파일 로드 시도
            var result = TryLoad(MainFile);
            if (result is not null)
                return result;

            // 2. 실패 시 백업 파일 로드 시도
            result = TryLoad(BackupFile);
            if (result is not null)
                return result;

            // 3. 둘 다 실패 시 빈 목록 반환
            return new List<Bookmark>();
        }

        public bool Save(List<Bookmark> bookmarks)
        {
            try
            {
                Directory.CreateDirectory(AppFolder);

                var data = new BookmarkData { Bookmarks = bookmarks };
                var json = JsonSerializer.Serialize(data, JsonOptions);

                // 메인 파일 저장
                File.WriteAllText(MainFile, json);

                // 백업 파일 갱신
                File.WriteAllText(BackupFile, json);

                return true;
            }
            catch
            {
                return false;
            }
        }

        private List<Bookmark>? TryLoad(string filePath)
        {
            try
            {
                if (!File.Exists(filePath))
                    return null;

                var json = File.ReadAllText(filePath);
                var data = JsonSerializer.Deserialize<BookmarkData>(json);

                return data?.Bookmarks ?? new List<Bookmark>();
            }
            catch
            {
                return null;
            }
        }
    }
}