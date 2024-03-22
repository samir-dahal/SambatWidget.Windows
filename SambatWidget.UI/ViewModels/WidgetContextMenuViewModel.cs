using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SambatWidget.UI.Helpers;

namespace SambatWidget.UI.ViewModels
{
    public partial class WidgetContextMenuViewModel : ObservableObject
    {
        [ObservableProperty]
        bool isLocked = App.Setting.LockPosition;

        [ObservableProperty]
        bool globalPosition = App.Setting.AllowGlobalPosition;

        [ObservableProperty]
        bool autoRunAtStartup = App.Setting.AutoRunAtStartup;

        [RelayCommand]
        private void CopyToday()
        {

        }

        [RelayCommand]
        private void LockPosition()
        {
            App.Setting.LockPosition = IsLocked;
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
    }
}
