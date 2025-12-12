using Newtonsoft.Json;
using SambatWidget.Core.Models;
using System;
using System.Collections.Generic;
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
        public static DateTime? ConvertToNepalTime(DateTime foreignDateTime, string foreignTimeZone)
        {
            try
            {
                TimeZoneInfo foreignTz = TZConvert.GetTimeZoneInfo(foreignTimeZone);
                TimeZoneInfo nepalTz = TZConvert.GetTimeZoneInfo("Asia/Kathmandu");

                // Step 1: Convert foreign time → UTC
                DateTime utc = TimeZoneInfo.ConvertTimeToUtc(foreignDateTime, foreignTz);

                // Step 2: Convert UTC → Nepal time
                DateTime nepalTime = TimeZoneInfo.ConvertTimeFromUtc(utc, nepalTz);

                return nepalTime;
            }
            catch
            {
                return null;
            }
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
