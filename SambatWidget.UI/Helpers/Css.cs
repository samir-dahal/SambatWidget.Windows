using System.Windows;

namespace SambatWidget.UI.Helpers
{
    public static class Css
    {
        public static string GetClass(DependencyObject element)
        {
            if (element == null)
                throw new ArgumentNullException("element");

            return (string)element.GetValue(ClassProperty);
        }

        public static void SetClass(DependencyObject element, string value)
        {
            if (element == null)
                throw new ArgumentNullException("element");

            element.SetValue(ClassProperty, value);
        }

        public static readonly DependencyProperty ClassProperty =
            DependencyProperty.RegisterAttached("Class", typeof(string), typeof(Css),
                new PropertyMetadata(null, OnClassChanged));
        private static void OnClassChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var ui = d as FrameworkElement;
            Style newStyle = new Style();

            if (e.NewValue != null)
            {
                var names = e.NewValue as string;
                var arr = names.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var name in arr)
                {
                    Style style = ui.FindResource(name) as Style;
                    foreach (var setter in style.Setters)
                    {
                        newStyle.Setters.Add(setter);
                    }
                    foreach (var trigger in style.Triggers)
                    {
                        newStyle.Triggers.Add(trigger);
                    }
                }
            }
            ui.Style = newStyle;
        }
    }
}
