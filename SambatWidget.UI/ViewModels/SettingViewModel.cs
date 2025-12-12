using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NepDate.Extensions;
using SambatWidget.Core;
using SambatWidget.UI.Helpers;
using System.ComponentModel;
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
        DateTime targetTimeZoneTime = DateTime.Now;

        [ObservableProperty]
        bool showTimeZoneOffset = App.Setting.ShowTimeZoneOffset;

        [ObservableProperty]
        string nepalTime = null!;
        public SettingViewModel()
        {
            TimeZones = new WpfObservableRangeCollection<string>();
            DateConverterViewModel = new DateConverterViewModel();
            var timeZone = WorldClock.GetTimeZoneData(SelectedTimeZone);
            if (timeZone is not null)
            {
                TargetTimeZoneTime = timeZone.TargetDateTime;
            }
        }
        partial void OnSelectedTimeZoneChanged(string value)
        {
            UpdateNepalTime();
        }

        partial void OnTargetTimeZoneTimeChanged(DateTime value)
        {
            UpdateNepalTime();
        }

        private void UpdateNepalTime()
        {
            var time = WorldClock.ConvertToNepalTime(TargetTimeZoneTime, SelectedTimeZone);
            if (time is null) return;
            string nepalTimePart = time.Value.ToString("hh:mm tt");
            NepalTime = $"{time.Value.ToNepaliDate().ToLongDateString()} {nepalTimePart}";
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
            if (SelectedTheme.Key != previousTheme.Key)
            {
                ThemeManager.ApplyTheme(SelectedTheme.Value);
                previousTheme = SelectedTheme;
            }
        }
        private void SaveTimeZoneChanges()
        {
            App.Setting.TimeZone = SelectedTimeZone;
            App.Setting.ShowTimeZoneOffset = ShowTimeZoneOffset;
        }
        public async Task GetTimeZonesAsync()
        {
            var timeZones = await WorldClock.GetTimeZonesAsync();
            TimeZones.ReplaceRange(timeZones);
        }
    }
}
