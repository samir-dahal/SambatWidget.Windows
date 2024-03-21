using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace SambatWidget.UI.ViewModels
{
    public partial class WidgetContextMenuViewModel : ObservableObject
    {
        [ObservableProperty]
        bool isLocked = App.Setting.LockPosition;
        
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
            App.Current.Shutdown();
        }
    }
}
