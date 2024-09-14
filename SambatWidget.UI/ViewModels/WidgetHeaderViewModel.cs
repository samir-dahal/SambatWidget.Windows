using CommunityToolkit.Mvvm.ComponentModel;
using SambatWidget.Core;

namespace SambatWidget.UI.ViewModels
{
    public partial class WidgetHeaderViewModel : ObservableObject
    {
        private readonly ICalendarRenderer _calendarRenderer;
        public WidgetHeaderViewModel(ICalendarRenderer renderer)
        {
            _calendarRenderer = renderer;
            InitHeader();
        }
        [ObservableProperty]
        string header;

        [ObservableProperty]
        string subHeader;

        [ObservableProperty]
        string timeZone;
        public void InitHeader()
        {
            SetHeader();
            InitTimeZone();
            SetSubHeader();
        }
        private void SetHeader()
        {
            if (App.Setting.IsNepali)
            {
                Header = _calendarRenderer.GetWeekDayNameNP();
            }
            else
            {
                // Use the abbreviated day of the week name when the timezone is enabled
                Header = App.Setting.ShowTimeZone
                            ? _calendarRenderer.GetShortWeekDayName()
                            : _calendarRenderer.GetWeekDayName();
            }
        }
        private void SetSubHeader()
        {
            // Get the English header, conditionally formatting based on expanded setting
            string engHeader = App.Setting.IsExpanded
                                ? _calendarRenderer.GetConsecutiveEnglishMonthsPair()
                                : _calendarRenderer.GetFormattedEnglishDate();

            // Set subheader with Nepali and English dates
            SubHeader = App.Setting.IsNepali
                            ? $"{_calendarRenderer.GetFormattedNepaliDateNP()} | {engHeader}"
                            : $"{_calendarRenderer.GetFormattedNepaliDate()} | {engHeader}";
        }
        public void InitTimeZone()
        {
            var timeZoneData = WorldClock.GetTimeZoneData(App.Setting.TimeZone);
            if (App.Setting.ShowTimeZoneOffset && !App.Setting.IsExpanded)
            {
                TimeZone = timeZoneData?.GetTimeZoneWithOffset();
            }
            else
            {
                TimeZone = timeZoneData?.ToString();
            }
        }
    }
}
