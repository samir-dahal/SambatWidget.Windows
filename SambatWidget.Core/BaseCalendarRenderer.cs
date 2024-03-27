using NepDate;
using SambatWidget.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SambatWidget.Core
{
    public class BaseCalendarRenderer : ICalendarRenderer
    {
        protected NepaliDate _carouselNepDate;
        private readonly List<WidgetCalendarCellModel> _calendarData;
        public BaseCalendarRenderer()
        {
            _carouselNepDate = NepaliDate.Now;
            _calendarData = new List<WidgetCalendarCellModel>();
        }
        public IEnumerable<WidgetCalendarCellModel> RenderToday()
        {
            _carouselNepDate = NepaliDate.Now;
            return PrepareCalendarDataGrid(Populate());
        }
        public IEnumerable<WidgetCalendarCellModel> RenderNext()
        {
            _carouselNepDate = _carouselNepDate.AddMonths(1);
            return Render();
        }
        public IEnumerable<WidgetCalendarCellModel> RenderPrevious()
        {
            _carouselNepDate = _carouselNepDate.AddMonths(-1);
            return Render();
        }
        public virtual string GetConsecutiveEnglishYearsPair()
        {
            int engYear = _carouselNepDate.EnglishDate.Year;
            int nextEngYear = _carouselNepDate.MonthEndDate.EnglishDate.Year;
            if (engYear != nextEngYear)
            {
                return $"{engYear}/{nextEngYear}";
            }
            else
            {
                return engYear.ToString();
            }
        }
        public string GetConsecutiveEnglishMonthsPair()
        {
            string engMonth = new NepaliDate(_carouselNepDate.Year, _carouselNepDate.Month, 1).EnglishDate.ToString("MMM");
            string nextEngMonth = _carouselNepDate.MonthEndDate.EnglishDate.ToString("MMM");
            return $"{engMonth}/{nextEngMonth}";
        }
        public int? GetRemainingDays()
        {
            //do not show remaining days tooltip on other month if not started yet
            if (!IsToday())
            {
                return null;
            }
            //show 1 day as remaining day if end of the month and today is the same day
            if (_carouselNepDate == _carouselNepDate.MonthEndDate)
            {
                return 1;
            }
            else
            {
                int remainingDays = _carouselNepDate.MonthEndDate.Day - _carouselNepDate.Day;
                return remainingDays;
            }
        }
        public string GetWeekDayName()
        {
            if (!IsToday())
            {
                return GetThisMonthNepaliStartDate().DayOfWeek.ToString();
            }
            return DateTime.Now.DayOfWeek.ToString();
        }
        public string GetShortWeekDayName()
        {
            if (!IsToday())
            {
                return GetThisMonthNepaliStartDate().EnglishDate.ToString("ddd");
            }
            return DateTime.Now.ToString("ddd");
        }
        public bool IsToday() => NepaliDate.Now == _carouselNepDate;
        public string GetFormattedEnglishDate() => _carouselNepDate.EnglishDate.ToString("dd MMM, yyyy");
        public string GetFormattedNepaliDate()
        {
            if (!IsToday())
            {
                return $"{_carouselNepDate.MonthName} 1, {_carouselNepDate.Year}";
            }
            return $"{_carouselNepDate.MonthName} {_carouselNepDate.Day}, {_carouselNepDate.Year}";
        }
        private List<WidgetCalendarCellModel> Populate()
        {
            _calendarData.Clear();
            bool isTodayYearMonth = _carouselNepDate.Month == NepaliDate.Now.Month && _carouselNepDate.Year == NepaliDate.Now.Year;
            for (int i = 1; i <= _carouselNepDate.MonthEndDay; i++)
            {
                _calendarData.Add(new WidgetCalendarCellModel
                {
                    Date = i,
                    IsToday = i == NepaliDate.Now.Day && isTodayYearMonth,
                    HasEvent = EventParser.HasEvent(GetYearMonthKey(i)),
                    HasEventHoliday = EventParser.HasEventHoliday(GetYearMonthKey(i)),
                });
            }
            return _calendarData;
        }
        private IEnumerable<WidgetCalendarCellModel> Render()
        {
            return PrepareCalendarDataGrid(Populate());
        }
        private NepaliDate GetThisMonthNepaliStartDate()
        {
            return new NepaliDate(_carouselNepDate.Year, _carouselNepDate.Month, 1);
        }
        private IEnumerable<WidgetCalendarCellModel> PrepareCalendarDataGrid(List<WidgetCalendarCellModel> cells)
        {
            int currRow = 0;
            int currCol = 0;
            int monthStartDateIndex = (int)GetThisMonthNepaliStartDate().DayOfWeek;
            for (int i = 0; i < cells.Count(); i++)
            {
                if (currRow == 4 && currCol == 6)
                {
                    monthStartDateIndex = 0;
                }
                currCol = monthStartDateIndex % 7;
                currRow = (int)Math.Ceiling((double)(monthStartDateIndex + 1) / 7) - 1;
                cells[i].ColIndex = currCol;
                cells[i].RowIndex = currRow;
                monthStartDateIndex++;
            }
            return cells;
        }

        public string GetMonthName()
        {
            return _carouselNepDate.MonthName.ToString();
        }

        public string GetYearMonthKey(int date)
        {
            return $"{_carouselNepDate.Year}-{_carouselNepDate.Month}-{date}";
        }
    }
}
