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
        public void InitHeader()
        {
            //use the abbreviated day of week name to reduce the space when timezone is enabled
            Header = App.Setting.ShowTimeZone
                ? _calendarRenderer.GetShortWeekDayName() 
                : _calendarRenderer.GetWeekDayName();

            string engHeader = _calendarRenderer.GetConsecutiveEnglishMonthsPair();
            if (!App.Setting.IsExpanded)
            {
                engHeader = _calendarRenderer.GetFormattedEnglishDate();
            }
            SubHeader = $"{_calendarRenderer.GetFormattedNepaliDate()}  |  {engHeader}";
        }
    }
}
