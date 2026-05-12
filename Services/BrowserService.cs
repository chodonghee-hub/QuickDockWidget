using System;
using System.Diagnostics;

namespace QuickDock.Services
{
    public class BrowserService
    {
        private static readonly string[] AllowedSchemes = { "http", "https" };

        public enum OpenResult { Success, InvalidUrl, BlockedScheme, LaunchFailed }

        public OpenResult Open(string url)
        {
            if (!Uri.TryCreate(url, UriKind.Absolute, out Uri? uri))
                return OpenResult.InvalidUrl;

            if (!Array.Exists(AllowedSchemes, s => s == uri.Scheme.ToLower()))
                return OpenResult.BlockedScheme;

            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = url,
                    UseShellExecute = true
                });
                return OpenResult.Success;
            }
            catch
            {
                return OpenResult.LaunchFailed;
            }
        }
    }
}