using SambatWidget.UI.Helpers;
using System.Windows;

namespace SambatWidget.UI.Models
{
    public class SettingModel
    {
        public bool ShowTimeZone { get; set; }
        public bool IsExpanded { get; set; } = true;
        public bool LockPosition { get; set; }
        public bool AllowTransparency { get; set; }
        public string TimeZone { get; set; }
        public bool MinimizeOnLostFocus { get; set; }
        public bool ShowTimeZoneOffset { get; set; }
        public bool AutoRunAtStartup { get; set; }
        public bool AllowGlobalPosition { get; set; } = true;
        public string Theme { get; set; } = "LightTheme";
        public Point Position { get; set; }
        public virtual SettingModel Save(Action<SettingModel> action = null)
        {
            action?.Invoke(this);
            return AppHelpers.SaveAppSettings(this);
        }
    }
}
