using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SambatWidget.Core.Models
{
    public class WidgetCalendarCellModel
    {
        public int RowIndex { get; set; }
        public int ColIndex { get; set; }
        public int Date { get; set; }
        public bool IsToday { get; set; }
        public bool HasEvent { get; set; }
        public bool IsSatDay => ColIndex == 6;
    }
}
