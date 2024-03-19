using SambatWidget.Core.Models;
using System.Collections.Generic;

namespace SambatWidget.Core
{
    public interface ICalendarRenderer
    {
        IEnumerable<WidgetCalendarCellModel> Render();
        IEnumerable<WidgetCalendarCellModel> RenderNext();
        IEnumerable<WidgetCalendarCellModel> RenderPrevious();
        string GetConsecutiveEnglishYearsPair();
        string GetConsecutiveEnglishMonthsPair();
        string GetWeekDayName();
        string GetShortWeekDayName();
        int? GetRemainingDays();
        string GetFormattedEnglishDate();
        string GetFormattedNepaliDate();
        bool IsToday();
    }
}
