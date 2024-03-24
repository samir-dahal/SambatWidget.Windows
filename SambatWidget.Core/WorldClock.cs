using SambatWidget.Core.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using TimeZoneConverter;

namespace SambatWidget.Core
{
    public static class WorldClock
    {
        private static string _apiUrl = "http://worldtimeapi.org";
        public static async Task<IEnumerable<string>> GetTimeZonesAsync()
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(_apiUrl);
                var res = await client.GetAsync("/api/timezones");
                var timezones = await res.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<IEnumerable<string>>(timezones);
            }
        }
        public static TimeZoneModel GetTimeZoneData(string targetTimeZone)
        {
            try
            {
                DateTime currentDateTime = DateTime.UtcNow;
                TimeZoneInfo targetTimeZoneInfo = TZConvert.GetTimeZoneInfo(targetTimeZone);
                DateTime targetDateTime = TimeZoneInfo.ConvertTimeFromUtc(currentDateTime, targetTimeZoneInfo);

                TimeSpan localOffset = TimeZoneInfo.Local.GetUtcOffset(currentDateTime);
                TimeSpan targetOffset = targetTimeZoneInfo.GetUtcOffset(targetDateTime);
                TimeSpan timeDifference = targetOffset - localOffset;

                string timeDifferenceString = (timeDifference < TimeSpan.Zero ? "-" : "+") + timeDifference.ToString(@"hh\:mm");
                string city = SanitizeTimeZoneCity(targetTimeZone);
                return new TimeZoneModel
                {
                    City = city,
                    Offset = timeDifferenceString,
                    TargetDateTime = targetDateTime,
                };
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
