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
        bool isNepali = App.Setting.IsNepali;

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
                App.Setting.Save(x => x.IsExpanded = false);
                DecideWhenToTransparent();
            });
        }
        [RelayCommand]
        private void ToggleExpand()
        {
            Render(() =>
            {
                IsExpanded = !IsExpanded;
                App.Setting.Save(x => x.IsExpanded = IsExpanded);
                DecideWhenToTransparent();
            });
        }
        [RelayCommand]
        private void ToggleTimeZone()
        {
            Render(() =>
            {
                App.Setting.Save(x => x.ShowTimeZone = ShowTimeZone);
            });
        }
        [RelayCommand]
        private void ToggleTransparency()
        {
            IsTransparent = AllowTransparency;
            App.Setting.Save(x => x.AllowTransparency = AllowTransparency);
            DecideWhenToTransparent();
        }
        [RelayCommand]
        private void HandleCellClick(WidgetCalendarCellModel cell)
        {
            if (cell.IsNotPartOfCurrentPage) return;
            EventPopupVisible = false;
            EventInfo = EventParser.GetEventByDate(_calendarRenderer.GetYearMonthKey(cell.Day));
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
        [RelayCommand]
        private void ToggleNepaliLang()
        {
            App.Setting.Save(x => x.IsNepali = IsNepali);
            WidgetCalendarViewModel.ReRender();
            WidgetHeaderViewModel.InitHeader();
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
