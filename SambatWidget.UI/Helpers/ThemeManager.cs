using System.Windows;

namespace SambatWidget.UI.Helpers
{
    public static class ThemeManager
    {
        private static string currentTheme = "/Resources/Themes/LightTheme.xaml";
        public static IDictionary<string, string> Themes
        {
            get
            {
                return new Dictionary<string, string>
                {
                    {"Light", "LightTheme" },
                    {"Dark", "DarkTheme" },
                    {"Black", "Black" },
                    {"Solarize Dark", "SolarizedDark" },
                    {"Nord", "Nord" },
                };
            }
        }
        public static string CurrentTheme => App.Setting.Theme;
        public static void ApplyTheme(string themeName)
        {
            AddResourceDictionary($"{themeName}.xaml");
            App.Setting.Theme = themeName;
        }
        private static void AddResourceDictionary(string src)
        {
            var themeResource = App.Current.Resources.MergedDictionaries[3];
            if (themeResource != null)
            {
                themeResource.MergedDictionaries.Clear();
                themeResource.MergedDictionaries.Add(new ResourceDictionary() { Source = new Uri($"/Resources/Themes/{src}", UriKind.Relative) });
            }
        }
    }
}
