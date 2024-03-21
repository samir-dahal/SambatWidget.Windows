using SambatWidget.UI.Helpers;
using System.Text.Json;
using System.Windows;

namespace SambatWidget.UI.Models
{
    public class SettingModel
    {
        public bool ShowTimeZone { get; set; }
        public bool IsExpanded { get; set; } = true;
        public bool LockPosition { get; set; }
        public bool AllowTransparency { get; set; }
        public bool AllowGlobalPosition { get; set; }
        public string Theme { get; set; } = "LightTheme";
        public Point Position { get; set; }
        public virtual SettingModel Save()
        {
            return AppHelpers.SaveAppSettings(this);
        }
    }
}
