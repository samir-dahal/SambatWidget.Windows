using NepDate;
using SambatWidget.Core.Models;
using System;
using System.Globalization;
using System.Linq;

namespace SambatWidget.Core
{
    public static class DateConverter
    {
        private static readonly CultureInfo EnUsCulture = CultureInfo.GetCultureInfo("en-US");
        private static readonly string[] SupportedParseFormats = new[] { "yyyy/MM/dd", "yyyy/dd/MM", "yyyy/M/d", "yyyy/d/M", "yyyy-M-d", "yyyy-d-M", "yyyy-dd-MM", "yyyy-MM-dd" }
            .Union(EnUsCulture.DateTimeFormat.GetAllDateTimePatterns())
            .Distinct()
            .ToArray();

        public static string GetCurrentNepaliDate() => NepaliDate.Now.ToString(DateFormats.YearMonthDay, Separators.ForwardSlash);
        public static DateConvertResult ConvertToNepaliDate(this string engDate)
        {
            try
            {
                var result = DateTime.ParseExact(engDate, SupportedParseFormats, EnUsCulture, DateTimeStyles.AssumeLocal).ToNepaliDate().ToLongDateString(displayDayName: true);
                return new DateConvertResult(true, result);
            }
            catch
            {
                return new DateConvertResult(false, "Invalid date format");
            }
        }
        public static DateConvertResult ConvertToEnglishDate(this string nepDate)
        {
            try
            {
                var result = NepaliDate.Parse(nepDate);
                return new DateConvertResult(true, result.EnglishDate.ToLongDateString());
            }
            catch
            {
                return new DateConvertResult(false, "Invalid date format");
            }
        }
    }
}
