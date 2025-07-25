﻿using NepDate;
using SambatWidget.Core.Models;
using System;
using System.Collections.Generic;

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
            return Populate();
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
            int nextEngYear = _carouselNepDate.MonthEndDate().EnglishDate.Year;
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
            string nextEngMonth = _carouselNepDate.MonthEndDate().EnglishDate.ToString("MMM");
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
            if (_carouselNepDate == _carouselNepDate.MonthEndDate())
            {
                return 1;
            }
            else
            {
                int remainingDays = _carouselNepDate.MonthEndDate().Day - _carouselNepDate.Day;
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
        public string GetWeekDayNameNP()
        {
            if (!IsToday())
            {
                return GetNepaliWeekDayName(GetThisMonthNepaliStartDate().DayOfWeek);
            }
            return GetNepaliWeekDayName(_carouselNepDate.DayOfWeek);
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
            return _carouselNepDate.ToLongDateString();
        }
        public string GetFormattedNepaliDateNP()
        {
            if (!IsToday())
            {
                return new NepaliDate(_carouselNepDate.Year, _carouselNepDate.Month, 1).ToLongDateUnicodeString();
            }
            return _carouselNepDate.ToLongDateUnicodeString();
        }
        private List<WidgetCalendarCellModel> Populate()
        {
            _calendarData.Clear();

            bool isCurrentMonth = _carouselNepDate.Month == NepaliDate.Now.Month && _carouselNepDate.Year == NepaliDate.Now.Year;
            int monthStartDayIndex = (int)GetThisMonthNepaliStartDate().DayOfWeek;
            int totalDaysInPrevMonth = _carouselNepDate.AddMonths(-1).MonthEndDay;
            int totalDaysInCurrMonth = _carouselNepDate.MonthEndDay;

            // Total cells (42) minus the current month's days and starting offset gives the remaining cells for the end
            int trailingDaysCount = 42 - (totalDaysInCurrMonth + monthStartDayIndex);

            int currentCellIndex = 0; // To keep track of the current cell position in the calendar grid

            // Add trailing days from the previous month
            for (int i = 0; i < monthStartDayIndex; i++, currentCellIndex++)
            {
                _calendarData.Add(new WidgetCalendarCellModel()
                    .FadeOutCurrentDay((totalDaysInPrevMonth - monthStartDayIndex) + i + 1)
                    .SetGridPosition(currentCellIndex));
            }

            // Add the current month's days
            for (int day = 1; day <= totalDaysInCurrMonth; day++, currentCellIndex++)
            {
                string yearMonthKey = GetYearMonthKey(day);
                _calendarData.Add(new WidgetCalendarCellModel
                {
                    Day = day,
                    IsToday = day == NepaliDate.Now.Day && isCurrentMonth,
                    HasEvent = EventParser.HasEvent(yearMonthKey),
                    HasEventHoliday = EventParser.HasEventHoliday(yearMonthKey),
                }.SetGridPosition(currentCellIndex));
            }

            // Adjust trailingDaysCount if the calendar fits within 5 rows
            //if (_calendarData.Count <= 35)
            //{
            //    trailingDaysCount = 35 - _calendarData.Count;
            //}

            // Add leading days for the next month
            for (int i = 1; i <= trailingDaysCount; i++, currentCellIndex++)
            {
                _calendarData.Add(new WidgetCalendarCellModel()
                    .FadeOutCurrentDay(i)
                    .SetGridPosition(currentCellIndex));
            }

            return _calendarData;
        }

        private IEnumerable<WidgetCalendarCellModel> Render()
        {
            return Populate();
        }
        private NepaliDate GetThisMonthNepaliStartDate()
        {
            return new NepaliDate(_carouselNepDate.Year, _carouselNepDate.Month, 1);
        }

        public string GetMonthName()
        {
            return _carouselNepDate.MonthName.ToString();
        }

        public string GetYearMonthKey(int date)
        {
            return $"{_carouselNepDate.Year}-{_carouselNepDate.Month}-{date}";
        }
        private string GetNepaliWeekDayName(DayOfWeek dayOfWeek) =>
           dayOfWeek switch
           {
               DayOfWeek.Sunday => "आइतबार",
               DayOfWeek.Monday => "सोमबार",
               DayOfWeek.Tuesday => "मंगलबार",
               DayOfWeek.Wednesday => "बुधबार",
               DayOfWeek.Thursday => "बिहिबार",
               DayOfWeek.Friday => "शुक्रबार",
               DayOfWeek.Saturday => "शनिबार",
               _ => string.Empty,
           };
    }
}
