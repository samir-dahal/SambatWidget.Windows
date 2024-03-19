using MvvmHelpers;
using SambatWidget.Core;
using SambatWidget.Core.Models;

namespace SambatWidget.UI.ViewModels
{
    public class WidgetCalendarViewModel
    {
        private readonly ICalendarRenderer _calendarRenderer;
        public ObservableRangeCollection<WidgetCalendarCellModel> CalendarCells { get; }
        public WidgetCalendarViewModel(ICalendarRenderer renderer)
        {
            _calendarRenderer = renderer;
            CalendarCells = new ObservableRangeCollection<WidgetCalendarCellModel>();
            Init();
        }
        public void Init()
        {
            Render(() => CalendarCells.AddRange(_calendarRenderer.Render()));
        }
        public void RenderNext()
        {
            Render(() => CalendarCells.AddRange(_calendarRenderer.RenderNext()));
        }
        public void RenderPrevious()
        {
            Render(() => CalendarCells.AddRange(_calendarRenderer.RenderPrevious()));
        }
        private void Render(Action renderAction)
        {
            CalendarCells.Clear();
            renderAction();
        }
    }
}
