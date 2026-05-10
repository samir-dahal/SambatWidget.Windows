using NepDate;
using SambatWidget.Core.Models;
using System;
using System.Linq;

namespace SambatWidget.Core
{
    public static class EventParser
    {
        /// <summary>
        /// Get event data by Nepali date format as param yyyy-m-d
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static EventDataModel GetEventByDate(string date)
        {
            try
            {
                var nepDate = NepaliDate.Parse(date);
                return GetEventByDate(nepDate.Year, nepDate.Month, nepDate.Day);
            }
            catch
            {
                return null;
            }
        }

        public static EventDataModel GetEventByDate(int year, int month, int day)
        {
            try
            {
                var nepDate = new NepaliDate(year, month, day);
                var info = nepDate.GetCalendarInfo();

                bool hasEvents = info.EventsNp.Length > 0 || info.EventsEn.Length > 0;
                string nepaliName = info.EventsNp.Length > 0 ? string.Join(", ", info.EventsNp.Where(x => !string.IsNullOrWhiteSpace(x))) : string.Empty;
                string englishName = info.EventsEn.Length > 0 ? string.Join(", ", info.EventsEn.Where(x => !string.IsNullOrWhiteSpace(x))) : string.Empty;
                string tithi = info.TithiNp ?? string.Empty;
                string nepaliDate = nepDate.ToLongDateUnicodeString();
                string englishDate = nepDate.EnglishDate.ToLongDateString();

                return new EventDataModel(
                    info.IsPublicHoliday,
                    hasEvents,
                    nepaliName,
                    englishName,
                    nepaliDate,
                    englishDate,
                    tithi);
            }
            catch
            {
                return null;
            }
        }

        public static bool HasEvent(string date)
        {
            return GetEventByDate(date)?.HasEvent ?? false;
        }

        public static bool HasEventHoliday(string date)
        {
            return GetEventByDate(date)?.IsHoliday ?? false;
        }
    }
}
