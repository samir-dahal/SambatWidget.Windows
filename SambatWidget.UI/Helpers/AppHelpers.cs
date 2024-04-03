using System.IO;
using Microsoft.Win32;
using System.Windows;
using SambatWidget.UI.Models;
using System.Text.Json;
using SambatWidget.Core;

namespace SambatWidget.UI.Helpers
{
    public static class AppHelpers
    {
        private const string REGISTRY_PATH = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run";
        private const string APP_NAME = "SambatWidget";
        private const string SETTING_FILE = "sambatwidget.json";
        private const string DEFAULT_EVENT_FILE_PATH = "./Resources/events_minified.json";
        private const string EVENT_FILE = "events_minified.json";
        private static string GetAppDirectoryFilePath(string fileName)
        {
            if (!string.IsNullOrWhiteSpace(fileName))
            {
                return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), APP_NAME, fileName);
            }
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), APP_NAME);
        }
        public static void DisableAutoStartAtStartup()
        {
            try
            {
                CoreAutoStartAtStartup(false);
            }
            catch { }
        }
        public static void EnableAutoStartAtStartup()
        {
            try
            {
                CoreAutoStartAtStartup(true);
            }
            catch { }
        }
        public static bool IsAutoStartAtStartupEnabled()
        {
            try
            {
                RegistryKey key = Registry.CurrentUser.OpenSubKey(REGISTRY_PATH, true);
                return key.GetValue(APP_NAME) != null;
            }
            catch
            {
                return false;
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
                string eventFilePath = GetAppDirectoryFilePath(EVENT_FILE);
                string appDir = GetAppDirectoryFilePath(null);
                if (!Directory.Exists(appDir))
                {
                    Directory.CreateDirectory(appDir);
                }
                if (!File.Exists(settingFilePath))
                {
                    File.WriteAllText(settingFilePath, JsonSerializer.Serialize(new SettingModel()));
                }
                bool hasEvent = File.Exists(eventFilePath);
                if (!hasEvent && File.Exists(DEFAULT_EVENT_FILE_PATH))
                {
                    File.Copy(DEFAULT_EVENT_FILE_PATH, GetAppDirectoryFilePath(EVENT_FILE), true);
                    hasEvent = true;
                }
                if (hasEvent)
                {
                    EventParser.ParseEventsJson(GetAppDirectoryFilePath(EVENT_FILE));
                }
                string json = File.ReadAllText(settingFilePath);
                return JsonSerializer.Deserialize<SettingModel>(json);
            }
            catch
            {
                return new SettingModel();
            }
        }
        public static SettingModel SaveAppSettings(SettingModel settings)
        {
            try
            {
                File.WriteAllText(GetAppDirectoryFilePath(SETTING_FILE), JsonSerializer.Serialize(settings));
            }
            catch { }
            return settings;
        }
        private static void CoreAutoStartAtStartup(bool enable)
        {
            RegistryKey key = Registry.CurrentUser.OpenSubKey(REGISTRY_PATH, true);
            if (enable)
            {
                string dirPath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
                string startPath = Path.Combine(dirPath, $"{APP_NAME}.exe");
                key.SetValue(APP_NAME, startPath);
            }
            else
            {
                key.DeleteValue(APP_NAME, false);
            }
            key.Close();
        }
    }
}
