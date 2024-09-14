using CommunityToolkit.Mvvm.ComponentModel;
using SambatWidget.Core;
using SambatWidget.Core.Models;
using SambatWidget.UI.Helpers;

namespace SambatWidget.UI.ViewModels
{
    public partial class WidgetCalendarViewModel : ObservableObject
    {
        private readonly ICalendarRenderer _calendarRenderer;
        public WpfObservableRangeCollection<WidgetCalendarCellModel> CalendarCells { get; }
        public WidgetCalendarViewModel(ICalendarRenderer renderer)
        {
            _calendarRenderer = renderer;
            CalendarCells = new WpfObservableRangeCollection<WidgetCalendarCellModel>();
            Init();
        }
        [ObservableProperty]
        string sunday = "Sun";

        [ObservableProperty]
        string monday = "Mon";

        [ObservableProperty]
        string tuesday = "Tue";

        [ObservableProperty]
        string wednesday = "Wed";

        [ObservableProperty]
        string thursday = "Thu";

        [ObservableProperty]
        string friday = "Fri";

        [ObservableProperty]
        string saturday = "Sat";
        public void Init()
        {
            SetDynamicWeekdayNamesForHeader();
            Render(() => CalendarCells.AddRange(_calendarRenderer.RenderToday()));
        }

        private void SetDynamicWeekdayNamesForHeader()
        {
            if (App.Setting.IsNepali)
            {
                Sunday = "आइ";
                Monday = "सोम";
                Tuesday = "मंगल";
                Wednesday = "बुध";
                Thursday = "बिहि";
                Friday = "शुक्र";
                Saturday = "शनि";
            }
            else
            {
                Sunday = "Sun";
                Monday = "Mon";
                Tuesday = "Tue";
                Wednesday = "Wed";
                Thursday = "Thu";
                Friday = "Fri";
                Saturday = "Sat";
            }
        }

        public void RenderNext()
        {
            Render(() => CalendarCells.AddRange(_calendarRenderer.RenderNext()));
        }
        public void RenderPrevious()
        {
            Render(() => CalendarCells.AddRange(_calendarRenderer.RenderPrevious()));
        }
        public void ReRender()
        {
            CalendarCells.Clear();
            Init();
        }
        private void Render(Action renderAction)
        {
            CalendarCells.Clear();
            renderAction();
        }
    }
}
