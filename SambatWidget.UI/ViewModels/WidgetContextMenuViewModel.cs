using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NepDate;
using SambatWidget.UI.Helpers;

namespace SambatWidget.UI.ViewModels
{
    public partial class WidgetContextMenuViewModel : ObservableObject
    {
        [ObservableProperty]
        bool isLocked = App.Setting.LockPosition;

        [ObservableProperty]
        bool minimizeOnLostFocus = App.Setting.MinimizeOnLostFocus;

        [ObservableProperty]
        bool globalPosition = App.Setting.AllowGlobalPosition;

        [ObservableProperty]
        bool autoRunAtStartup = App.Setting.AutoRunAtStartup;

        [ObservableProperty]
        string defaultFormat;

        [ObservableProperty]
        string longDateFormat;

        [ObservableProperty]
        string unicodeFormat;

        [ObservableProperty]
        string longUnicodeFormat;

        [ObservableProperty]
        string dashedFormat;
        public WidgetContextMenuViewModel()
        {
            InitCopyTodayContextMenu();
        }

        [RelayCommand]
        private void CopyToday(string dateFormat)
        {
            AppHelpers.CopyToClipboard(dateFormat);
        }

        [RelayCommand]
        private void LockPosition()
        {
            App.Setting.LockPosition = IsLocked;
        }
        [RelayCommand]
        private void ToggleMinimizeOnLostFocus()
        {
            App.Setting.MinimizeOnLostFocus = MinimizeOnLostFocus;
        }
        [RelayCommand]
        private void ShowSetting()
        {
            new SettingWindow().ShowDialog();
        }
        [RelayCommand]
        private void Exit()
        {
            App.IsShuttingDown = true;
            App.Current.Shutdown();
        }
        [RelayCommand]
        private void ToggleGlobalPosition()
        {
            App.Setting.AllowGlobalPosition = GlobalPosition;
        }
        [RelayCommand]
        private void ToggleAutoRunAtStartup()
        {
            if (AutoRunAtStartup)
            {
                AppHelpers.EnableAutoStartAtStartup();
            }
            else
            {
                AppHelpers.DisableAutoStartAtStartup();
            }
            App.Setting.AutoRunAtStartup = AutoRunAtStartup;
        }
        public void InitCopyTodayContextMenu()
        {
            DefaultFormat = NepaliDate.Now.ToString();
            LongDateFormat = NepaliDate.Now.ToLongDateString();
            UnicodeFormat = NepaliDate.Now.ToUnicodeString();
            LongUnicodeFormat = NepaliDate.Now.ToLongDateUnicodeString();
            DashedFormat = NepaliDate.Now.ToString(DateFormats.YearMonthDay, Separators.Dash);
        }
    }
}
