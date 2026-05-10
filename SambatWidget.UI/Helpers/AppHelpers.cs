using System.IO;
using Microsoft.Win32;
using System.Windows;
using SambatWidget.UI.Models;
using SambatWidget.Core;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace SambatWidget.UI.Helpers
{
    public static class AppHelpers
    {
        private const string REGISTRY_PATH = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run";
        private const string STARTUP_APPROVED_RUN_PATH = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\StartupApproved\Run";
        private const string APP_NAME = "SambatWidget";
        private const string SETTING_FILE = "sambatwidget.json";
        private const int SETTINGS_SAVE_DEBOUNCE_MS = 500;
        private static readonly object _settingsSaveLock = new object();
        private static CancellationTokenSource? _settingsSaveCts;
        private static string? _pendingSettingsJson;
        public static string GetAppDirectoryFilePath(string? fileName)
        {
            if (!string.IsNullOrWhiteSpace(fileName))
            {
                return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), APP_NAME, fileName);
            }
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), APP_NAME);
        }
        public static bool DisableAutoStartAtStartup()
        {
            try
            {
                return CoreAutoStartAtStartup(false);
            }
            catch
            {
                return false;
            }
        }
        public static bool EnableAutoStartAtStartup()
        {
            try
            {
                return CoreAutoStartAtStartup(true);
            }
            catch
            {
                return false;
            }
        }
        public static bool IsAutoStartAtStartupEnabled()
        {
            return GetAutoStartHealth().IsEnabled;
        }

        public static (bool IsEnabled, string Message) GetAutoStartHealth()
        {
            try
            {
                using var key = Registry.CurrentUser.OpenSubKey(REGISTRY_PATH, writable: false);
                string? startupCommand = key?.GetValue(APP_NAME)?.ToString();
                if (string.IsNullOrWhiteSpace(startupCommand))
                {
                    return (false, "Autorun is off: missing registry Run entry.");
                }

                string? startupPath = ExtractExecutablePath(startupCommand);
                if (string.IsNullOrWhiteSpace(startupPath))
                {
                    return (false, "Autorun is off: startup command is invalid.");
                }

                if (!File.Exists(startupPath))
                {
                    return (false, $"Autorun is off: executable not found at {startupPath}.");
                }

                if (IsStartupApprovedDisabled())
                {
                    return (false, "Autorun is blocked: disabled in Task Manager Startup Apps.");
                }

                return (true, "Autorun is healthy and enabled.");
            }
            catch (Exception ex)
            {
                return (false, $"Autorun status unavailable: {ex.Message}");
            }
        }
        public static bool CopyToClipboard(string text)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(text))
                {
                    Clipboard.SetText(text);
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public static SettingModel LoadAppSettings()
        {
            try
            {
                string settingFilePath = GetAppDirectoryFilePath(SETTING_FILE);
                string appDir = GetAppDirectoryFilePath(null);
                if (!Directory.Exists(appDir))
                {
                    Directory.CreateDirectory(appDir);
                }
                if (!File.Exists(settingFilePath))
                {
                    File.WriteAllText(settingFilePath, JsonConvert.SerializeObject(new SettingModel()));
                }
                string json = File.ReadAllText(settingFilePath);
                return JsonConvert.DeserializeObject<SettingModel>(json)!;
            }
            catch
            {
                return new SettingModel();
            }
        }
        public static SettingModel SaveAppSettings(SettingModel settings)
        {
            string serializedSettings;
            try
            {
                serializedSettings = JsonConvert.SerializeObject(settings);
            }
            catch
            {
                return settings;
            }

            CancellationToken token;
            lock (_settingsSaveLock)
            {
                _pendingSettingsJson = serializedSettings;

                _settingsSaveCts?.Cancel();
                _settingsSaveCts?.Dispose();

                _settingsSaveCts = new CancellationTokenSource();
                token = _settingsSaveCts.Token;
            }

            _ = Task.Run(async () =>
            {
                try
                {
                    await Task.Delay(SETTINGS_SAVE_DEBOUNCE_MS, token).ConfigureAwait(false);
                    FlushPendingSettingsWrites();
                }
                catch (TaskCanceledException)
                {
                }
            });

            return settings;
        }

        public static void FlushPendingSettingsWrites()
        {
            string? pendingJson;
            lock (_settingsSaveLock)
            {
                pendingJson = _pendingSettingsJson;
                _pendingSettingsJson = null;

                _settingsSaveCts?.Cancel();
                _settingsSaveCts?.Dispose();
                _settingsSaveCts = null;
            }

            if (string.IsNullOrWhiteSpace(pendingJson))
            {
                return;
            }

            try
            {
                File.WriteAllText(GetAppDirectoryFilePath(SETTING_FILE), pendingJson);
            }
            catch
            {
            }
        }
        private static bool CoreAutoStartAtStartup(bool enable)
        {
            using (var key = Registry.CurrentUser.OpenSubKey(REGISTRY_PATH, writable: true)
                           ?? Registry.CurrentUser.CreateSubKey(REGISTRY_PATH))
            {
                if (key == null)
                    throw new InvalidOperationException("Failed to access or create startup registry key.");

                if (enable)
                {
                    string exePath = GetCurrentExecutablePath();
                    string startupCommand = $"\"{exePath}\"";

                    key.SetValue(APP_NAME, startupCommand, RegistryValueKind.String);
                    ClearStartupApprovedOverride();
                    return string.Equals(key.GetValue(APP_NAME)?.ToString(), startupCommand, StringComparison.Ordinal);
                }
                else
                {
                    key.DeleteValue(APP_NAME, throwOnMissingValue: false);
                    return key.GetValue(APP_NAME) == null;
                }
            }
        }

        private static string GetCurrentExecutablePath()
        {
            string? exePath = Process.GetCurrentProcess().MainModule?.FileName;
            if (string.IsNullOrWhiteSpace(exePath) || !File.Exists(exePath))
            {
                exePath = Environment.ProcessPath;
            }

            if (string.IsNullOrWhiteSpace(exePath) || !File.Exists(exePath))
            {
                throw new InvalidOperationException("Unable to determine executable path.");
            }

            return exePath;
        }

        private static string? ExtractExecutablePath(string startupCommand)
        {
            string command = startupCommand.Trim();
            if (command.StartsWith("\"", StringComparison.Ordinal))
            {
                int endQuoteIndex = command.IndexOf('"', 1);
                if (endQuoteIndex > 1)
                {
                    return command.Substring(1, endQuoteIndex - 1);
                }
            }

            int firstSpace = command.IndexOf(' ');
            if (firstSpace > 0)
            {
                return command.Substring(0, firstSpace);
            }

            return command;
        }

        private static bool IsStartupApprovedDisabled()
        {
            using var key = Registry.CurrentUser.OpenSubKey(STARTUP_APPROVED_RUN_PATH, writable: false);
            if (key?.GetValue(APP_NAME) is byte[] stateBytes && stateBytes.Length > 0)
            {
                // 0x03 is the common "disabled" state used by Task Manager startup toggles.
                return stateBytes[0] == 0x03;
            }

            return false;
        }

        private static void ClearStartupApprovedOverride()
        {
            using var key = Registry.CurrentUser.OpenSubKey(STARTUP_APPROVED_RUN_PATH, writable: true)
                           ?? Registry.CurrentUser.CreateSubKey(STARTUP_APPROVED_RUN_PATH);
            key?.DeleteValue(APP_NAME, throwOnMissingValue: false);
        }
    }
}
