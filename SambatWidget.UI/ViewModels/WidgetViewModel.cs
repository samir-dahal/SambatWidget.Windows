using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SambatWidget.Core;

namespace SambatWidget.UI.ViewModels
{
    public partial class WidgetViewModel : ViewModelBase
    {
        private readonly ICalendarRenderer _calendarRenderer;
        public WidgetHeaderViewModel WidgetHeaderViewModel { get; }
        public WidgetCalendarViewModel WidgetCalendarViewModel { get; }
        public WidgetViewModel()
        {
            _calendarRenderer = new WidgetCalendarRenderer();
            WidgetHeaderViewModel = new WidgetHeaderViewModel(_calendarRenderer);
            WidgetCalendarViewModel = new WidgetCalendarViewModel(_calendarRenderer);
        }
        [ObservableProperty]
        bool showTimeZone = App.Setting.ShowTimeZone;

        [ObservableProperty]
        bool isExpanded = App.Setting.IsExpanded;

        [RelayCommand]
        private void RenderNext()
        {
            Render(WidgetCalendarViewModel.RenderNext);
        }
        [RelayCommand]
        private void RenderPrevious()
        {
            Render(WidgetCalendarViewModel.RenderPrevious);
        }
        [RelayCommand]
        private void ShowToday()
        {
            Render(WidgetCalendarViewModel.Init);
        }
        [RelayCommand]
        private void ToggleTimeZone()
        {
            ShowTimeZone = !ShowTimeZone;
            App.Setting.ShowTimeZone = ShowTimeZone;
        }
        [RelayCommand]
        private void ToggleExpand()
        {
            IsExpanded = !IsExpanded;
            App.Setting.IsExpanded = IsExpanded;
        }
        private void Render(Action renderAction)
        {
            renderAction();
            WidgetHeaderViewModel.InitHeader();
        }
    }
}
