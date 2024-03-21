using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SambatWidget.UI.Models
{
    public class WidgetWindowSetting
    {
        public bool IsFirstLoad { get; set; } = true;
        public Point Position { get; set; }
        public double WindowHeight { get; set; }
        public WidgetWindowSetting()
        {
            Position = App.Setting.Position;
        }
        public void SetPosition(double x, double y)
        {
            Position = new Point(x, y);
            App.Setting.Position = Position;
        }
    }
}
