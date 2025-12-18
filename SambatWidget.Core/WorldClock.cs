using Newtonsoft.Json;
using SambatWidget.Core.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Threading.Tasks;
using TimeZoneConverter;

namespace SambatWidget.Core
{
    public static class WorldClock
    {
        private static string _apiUrl = "https://github.com";
        public static async Task<IEnumerable<string>> GetTimeZonesAsync()
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(_apiUrl);
                var res = await client.GetAsync("user-attachments/files/20422385/timezones.json");
                var timezones = await res.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<IEnumerable<string>>(timezones);
            }
        }
        public static TimeZoneModel GetTimeZoneData(string targetTimeZone)
        {
            try
            {
                DateTime utcNow = DateTime.UtcNow;

                TimeZoneInfo targetTz = TZConvert.GetTimeZoneInfo(targetTimeZone);
                TimeZoneInfo nepalTz = TZConvert.GetTimeZoneInfo("Asia/Kathmandu");

                // Target time
                DateTime targetDateTime = TimeZoneInfo.ConvertTimeFromUtc(utcNow, targetTz);

                // Nepal time (converted from target time)
                DateTime utcFromTarget = TimeZoneInfo.ConvertTimeToUtc(targetDateTime, targetTz);
                DateTime nepalDateTime = TimeZoneInfo.ConvertTimeFromUtc(utcFromTarget, nepalTz);

                // Time difference from LOCAL PC time
                TimeSpan localOffset = TimeZoneInfo.Local.GetUtcOffset(utcNow);
                TimeSpan targetOffset = targetTz.GetUtcOffset(targetDateTime);
                TimeSpan difference = targetOffset - localOffset;

                string offsetStr = (difference < TimeSpan.Zero ? "-" : "+") + difference.ToString(@"hh\:mm");

                return new TimeZoneModel
                {
                    City = SanitizeTimeZoneCity(targetTimeZone),
                    Offset = offsetStr,
                    TargetDateTime = targetDateTime,
                    NepalDateTime = nepalDateTime
                };
            }
            catch
            {
                return null;
            }
        }
        private static string ConvertTime(
        string time,
        string fromTimeZone,
        string toTimeZone)
        {
            try
            {
                // Smart parse (supports 10:30, 10:30 PM, 22:30, etc.)
                if (!DateTime.TryParse(
                        time,
                        CultureInfo.InvariantCulture,
                        DateTimeStyles.AllowWhiteSpaces,
                        out var parsedTime))
                {
                    return null;
                }

                TimeZoneInfo fromTz = TZConvert.GetTimeZoneInfo(fromTimeZone);
                TimeZoneInfo toTz = TZConvert.GetTimeZoneInfo(toTimeZone);

                // Get today's date in source timezone
                DateTime fromToday = TimeZoneInfo.ConvertTime(DateTime.UtcNow, fromTz);

                // Combine date + parsed time
                DateTime fromDateTime = new DateTime(
                    fromToday.Year,
                    fromToday.Month,
                    fromToday.Day,
                    parsedTime.Hour,
                    parsedTime.Minute,
                    0,
                    DateTimeKind.Unspecified);

                // Convert source → UTC → target
                DateTime utc = TimeZoneInfo.ConvertTimeToUtc(fromDateTime, fromTz);
                DateTime targetTime = TimeZoneInfo.ConvertTimeFromUtc(utc, toTz);

                // Always return AM/PM
                return targetTime.ToString("hh:mm tt", CultureInfo.InvariantCulture);
            }
            catch
            {
                return null;
            }
        }
        public static string ConvertNepalToForeign(string nepalTime, string foreignTimeZone)
        {
            return ConvertTime(
                nepalTime,
                "Asia/Kathmandu",
                foreignTimeZone);
        }
        public static string? ConvertForeignToNepal(string foreignTime, string foreignTimeZone)
        {
            return ConvertTime(
                foreignTime,
                foreignTimeZone,
                "Asia/Kathmandu");
        }


        private static string SanitizeTimeZoneCity(string input)
        {
            int lastSlashIndex = input.LastIndexOf('/');
            if (lastSlashIndex == -1)
            {
                return input;
            }
            string sanitizedValue = input.Substring(lastSlashIndex + 1);
            return sanitizedValue;
        }
    }
}
