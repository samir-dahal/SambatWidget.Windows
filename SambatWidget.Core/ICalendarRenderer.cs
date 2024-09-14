using SambatWidget.Core.Models;
using System;
using System.Collections.Generic;

namespace SambatWidget.Core
{
    public interface ICalendarRenderer
    {
        IEnumerable<WidgetCalendarCellModel> RenderToday();
        IEnumerable<WidgetCalendarCellModel> RenderNext();
        IEnumerable<WidgetCalendarCellModel> RenderPrevious();
        string GetConsecutiveEnglishYearsPair();
        string GetConsecutiveEnglishMonthsPair();
        string GetWeekDayName();
        string GetWeekDayNameNP();
        string GetShortWeekDayName();
        string GetMonthName();
        int? GetRemainingDays();
        string GetFormattedEnglishDate();
        string GetFormattedNepaliDate();
        string GetFormattedNepaliDateNP();
        bool IsToday();
        string GetYearMonthKey(int date);
    }
}
