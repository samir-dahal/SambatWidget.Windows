using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SambatWidget.Core.Models
{
    public class WidgetCalendarCellModel
    {
        public bool IsNotPartOfCurrentPage { get; private set; }
        public int RowIndex { get; set; }
        public int ColIndex { get; set; }
        public int Day { get; set; }
        public bool IsToday { get; set; }
        public bool HasEvent { get; set; }
        public bool HasEventHoliday { get; set; }
        public bool IsSatDay => ColIndex == 6;
        public WidgetCalendarCellModel FadeOutCurrentDay(int day)
        {
            Day = day;
            IsNotPartOfCurrentPage = true;
            return this;
        }
    }
}
