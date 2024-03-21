using SambatWidget.Core;
using SambatWidget.Core.Models;
using SambatWidget.UI.Helpers;

namespace SambatWidget.UI.ViewModels
{
    public partial class WidgetCalendarViewModel
    {
        private readonly ICalendarRenderer _calendarRenderer;
        public WpfObservableRangeCollection<WidgetCalendarCellModel> CalendarCells { get; }
        public WidgetCalendarViewModel(ICalendarRenderer renderer)
        {
            _calendarRenderer = renderer;
            CalendarCells = new WpfObservableRangeCollection<WidgetCalendarCellModel>();
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
