using System.Windows;

namespace SambatWidget.UI.Models
{
    public class WidgetWindowSetting
    {
        public bool Loaded { get; private set; }
        public bool DefaultPositionSet { get; private set; }
        public bool IsRenderedAfterMouseAction { get; private set; }
        public System.Windows.Point Position { get; private set; }
        public double WindowHeight { get; private set; }
        public WidgetWindowSetting()
        {
            Position = App.Setting.Position;
        }
        public void SetWindowHeight(double height)
        {
            WindowHeight = height;
        }
        public void SetLoaded()
        {
            Loaded = true;
        }
        public void SetPosition(double x, double y)
        {
            Position = new System.Windows.Point(x, y);
            App.Setting.Save(x => x.Position = Position);
        }
        public void SetDefaultPosition(Action action)
        {
            action();
            DefaultPositionSet = true;
        }
        public void SetMouseActionRender(bool isMouseAction)
        {
            IsRenderedAfterMouseAction = isMouseAction;
        }
    }
}
