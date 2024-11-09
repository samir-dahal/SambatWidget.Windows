using System.Windows;

namespace SambatWidget.UI.Helpers
{
    public static class ThemeManager
    {
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
        public static void ApplyTheme(string themeName)
        {
            AddResourceDictionary($"{themeName}.xaml");
            App.Setting.Theme = themeName;
        }
        public static void ApplyDefaultTheme()
        {
            if (string.IsNullOrWhiteSpace(App.Setting.Theme) || !Themes.Values.Contains(App.Setting.Theme))
            {
                ApplyTheme("LightTheme");
            }
            else
            {
                ApplyTheme(App.Setting.Theme);
            }
        }
        private static void AddResourceDictionary(string src)
        {
            var themeResource = App.Current.Resources.MergedDictionaries.LastOrDefault();
            if (themeResource is not null)
            {
                themeResource.MergedDictionaries.Clear();
                themeResource.MergedDictionaries.Add(new ResourceDictionary() { Source = new Uri($"/Resources/Themes/{src}", UriKind.Relative) });
            }
        }
    }
}
