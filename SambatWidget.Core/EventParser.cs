using NepDate;
using Newtonsoft.Json;
using SambatWidget.Core.Models;
using System.Collections.Generic;
using System.IO;

namespace SambatWidget.Core
{
    public static class EventParser
    {
        private static Dictionary<string, EventDataModel> _events = new Dictionary<string, EventDataModel>();
        private static Dictionary<string, string[]> _cachedEvents = new Dictionary<string, string[]>();
        public static void ParseEventsJson(string path)
        {
            try
            {
                if (_cachedEvents.Count <= 0)
                {
                    _cachedEvents = JsonConvert.DeserializeObject<Dictionary<string, string[]>>(File.ReadAllText(path));
                    foreach (KeyValuePair<string, string[]> kvp in _cachedEvents)
                    {
                        int day = NepaliDate.Parse(kvp.Key).Day;
                        _events.Add(kvp.Key, new EventDataModel(bool.Parse(kvp.Value[0]), kvp.Value[1], kvp.Value[2], day));
                    }
                    _cachedEvents = null;
                }
            }
            catch { }
        }
        /// <summary>
        /// Get event data by Nepali date format as param yyyy-m-d
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static EventDataModel GetEventByDate(string date)
        {
            EventDataModel result = null;
            if (_events != null)
            {
                _events.TryGetValue(date, out result);
            }
            return result;
        }
        public static bool HasEvent(string date)
        {
            if (_events != null)
            {
                return _events.ContainsKey(date);
            }
            return false;
        }
        public static bool HasEventHoliday(string date)
        {
            return GetEventByDate(date)?.IsHoliday ?? false;
        }
    }
}
