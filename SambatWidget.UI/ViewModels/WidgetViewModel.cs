using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SambatWidget.Core;
using SambatWidget.Core.Models;

namespace SambatWidget.UI.ViewModels
{
    public partial class WidgetViewModel : ViewModelBase
    {
        private readonly ICalendarRenderer _calendarRenderer;
        public WidgetHeaderViewModel WidgetHeaderViewModel { get; }
        public WidgetCalendarViewModel WidgetCalendarViewModel { get; }
        public WidgetContextMenuViewModel WidgetContextMenuViewModel { get; }
        public WidgetViewModel()
        {
            _calendarRenderer = new WidgetCalendarRenderer();
            WidgetHeaderViewModel = new WidgetHeaderViewModel(_calendarRenderer);
            WidgetCalendarViewModel = new WidgetCalendarViewModel(_calendarRenderer);
            WidgetContextMenuViewModel = new WidgetContextMenuViewModel();
        }

        [ObservableProperty]
        bool isExpanded = App.Setting.IsExpanded;

        [ObservableProperty]
        bool showTimeZone = App.Setting.ShowTimeZone;

        [ObservableProperty]
        bool allowTransparency = App.Setting.AllowTransparency; //for context menu checkbox

        [ObservableProperty]
        bool isTransparent = App.Setting.AllowTransparency && !App.Setting.IsExpanded;

        [ObservableProperty]
        bool eventPopupVisible;

        [ObservableProperty]
        EventDataModel eventInfo;

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
            Render(() =>
            {
                WidgetCalendarViewModel.Init();
                WidgetContextMenuViewModel.InitCopyTodayContextMenu();
            });
        }
        [RelayCommand]
        private void MinimizeWidget()
        {
            Render(() =>
            {
                IsExpanded = false;
                App.Setting.IsExpanded = false;
                DecideWhenToTransparent();
            });
        }
        [RelayCommand]
        private void ToggleExpand()
        {
            Render(() =>
            {
                IsExpanded = !IsExpanded;
                App.Setting.IsExpanded = IsExpanded;
                DecideWhenToTransparent();
            });
        }
        [RelayCommand]
        private void ToggleTimeZone()
        {
            Render(() =>
            {
                App.Setting.ShowTimeZone = ShowTimeZone;
            });
        }
        [RelayCommand]
        private void ToggleTransparency()
        {
            IsTransparent = AllowTransparency;
            App.Setting.AllowTransparency = AllowTransparency;
            DecideWhenToTransparent();
        }
        [RelayCommand]
        private void HandleCellClick(int date)
        {
            EventPopupVisible = false;
            EventInfo = EventParser.GetEventByDate(_calendarRenderer.GetYearMonthKey(date));
            EventPopupVisible = EventInfo != null;
        }
        [RelayCommand]
        private void HideEventPopup()
        {
            if (EventPopupVisible)
            {
                EventPopupVisible = false;
            }
        }
        private void DecideWhenToTransparent()
        {
            if (App.Setting.AllowTransparency)
            {
                IsTransparent = !IsExpanded;
            }
        }
        private void Render(Action renderAction)
        {
            renderAction();
            WidgetHeaderViewModel.InitHeader();
        }
    }
}
