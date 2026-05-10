namespace SambatWidget.Core.Models
{
    public class EventDataModel
    {
        public EventDataModel(
            bool isHoliday,
            bool hasEvent,
            string nepaliName,
            string englishName,
            string nepaliDate,
            string englishDate,
            string tithi)
        {
            IsHoliday = isHoliday;
            HasEvent = hasEvent;
            NepaliName = nepaliName;
            EnglishName = englishName;
            NepaliDate = nepaliDate;
            EnglishDate = englishDate;
            Tithi = tithi;
        }
        public bool HasEvent { get; }
        public bool IsHoliday { get; }
        public string NepaliName { get; }
        public string EnglishName { get; }
        public string NepaliDate { get; }
        public string EnglishDate { get; }
        public string Tithi { get; }
    }
}
