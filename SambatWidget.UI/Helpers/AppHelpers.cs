using System.IO;
using Microsoft.Win32;
using System.Windows;
using SambatWidget.UI.Models;
using System.Text.Json;

namespace SambatWidget.UI.Helpers
{
    public static class AppHelpers
    {
        private const string REGISTRY_PATH = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run";
        private const string APP_NAME = "SambatWidget";
        private const string SETTING_FILE = "sambatwidget.json";
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
            if (!File.Exists(SETTING_FILE))
            {
                File.WriteAllText(SETTING_FILE, JsonSerializer.Serialize(new SettingModel()));
            }
            try
            {
                string json = File.ReadAllText(SETTING_FILE);
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
                File.WriteAllText(SETTING_FILE, JsonSerializer.Serialize(settings));
            }
            catch { }
            return settings;
        }
        private static void CoreAutoStartAtStartup(bool enable)
        {
            RegistryKey key = Registry.CurrentUser.OpenSubKey(REGISTRY_PATH, true);
            if (enable)
            {
                string startPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Programs), APP_NAME, $"{APP_NAME}.appref-ms");
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
