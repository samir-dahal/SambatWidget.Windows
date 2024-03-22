using SambatWidget.UI.Helpers;
using SambatWidget.UI.Models;
using System.Windows;

namespace SambatWidget.UI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static SettingModel Setting { get; private set; }
        protected override void OnStartup(StartupEventArgs e)
        {
            Setting = AppHelpers.LoadAppSettings();
            ThemeManager.ApplyDefaultTheme();
            base.OnStartup(e);
        }
        protected override void OnExit(ExitEventArgs e)
        {
            App.Setting.Save();
            base.OnExit(e);
        }
    }
}
