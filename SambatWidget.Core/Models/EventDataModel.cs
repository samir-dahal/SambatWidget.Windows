namespace SambatWidget.Core.Models
{
    public class EventDataModel
    {
        public EventDataModel(bool isHoliday, string nepaliName, string englishName, int date)
        {
            IsHoliday = isHoliday;
            NepaliName = nepaliName;
            EnglishName = englishName;
            Date = date;
        }
        public int Date { get; set; }
        public bool IsHoliday { get;  }
        public string NepaliName { get; }
        public string EnglishName { get; }
    }
}
