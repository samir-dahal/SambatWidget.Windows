﻿using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SambatWidget.Core;
using SambatWidget.UI.Helpers;
using System.Diagnostics;

namespace SambatWidget.UI.ViewModels
{
    public partial class SettingViewModel : ViewModelBase
    {
        private KeyValuePair<string, string> previousTheme;
        public IDictionary<string, string> Themes { get; set; } = ThemeManager.Themes;

        [ObservableProperty]
        KeyValuePair<string, string> selectedTheme = ThemeManager.Themes.FirstOrDefault(x => x.Value == App.Setting.Theme);

        public WpfObservableRangeCollection<string> TimeZones { get; }
        public DateConverterViewModel DateConverterViewModel { get; }

        [ObservableProperty]
        string selectedTimeZone = App.Setting.TimeZone;

        [ObservableProperty]
        bool showTimeZoneOffset = App.Setting.ShowTimeZoneOffset;
        public SettingViewModel()
        {
            TimeZones = new WpfObservableRangeCollection<string>();
            DateConverterViewModel = new DateConverterViewModel();
            GetTimeZonesAsync().FireAndForget();
        }

        [RelayCommand]
        private void ApplyChanges()
        {
            SaveUIChanges();
            SaveTimeZoneChanges();
            App.Setting.Save();
        }

        [RelayCommand]
        private void OpenSettingsPath()
        {
            Process.Start(new ProcessStartInfo(AppHelpers.GetAppDirectoryFilePath(null)) { UseShellExecute = true });
        }
        private void SaveUIChanges()
        {
            if (selectedTheme.Key != previousTheme.Key)
            {
                ThemeManager.ApplyTheme(selectedTheme.Value);
                previousTheme = selectedTheme;
            }
        }
        private void SaveTimeZoneChanges()
        {
            App.Setting.TimeZone = SelectedTimeZone;
            App.Setting.ShowTimeZoneOffset = ShowTimeZoneOffset;
        }
        private async Task GetTimeZonesAsync()
        {
            var timeZones = await WorldClock.GetTimeZonesAsync();
            TimeZones.AddRange(timeZones);
        }
    }
}
