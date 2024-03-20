using System.Text.Json;

namespace SambatWidget.UI.Models
{
    public class SettingModel
    {
        public bool ShowTimeZone { get; set; }
        public bool IsExpanded { get; set; }
        public bool LockPosition { get; set; }
        public bool AllowTransparency { get; set; }
        public string Theme { get; set; }
        public virtual void Save()
        {
            JsonSerializer.Serialize(this);
        }
    }
}
