using System;

namespace SambatWidget.Core.Models
{
    public class TimeZoneModel
    {
        public string City { get; set; }
        public string Offset { get; set; }
        public DateTime TargetDateTime { get; set; }
        public override string ToString()
        {
            return $"{City} {TargetDateTime.ToString("hh:mm tt")}";
        }
        public string GetTimeZoneWithOffset()
        {
            return $"{City} {TargetDateTime.ToString("hh:mm tt")} {Offset}";
        }
    }
}
