using SambatWidget.UI.Helpers;
using SambatWidget.UI.Models;
using System.Windows;

namespace SambatWidget.UI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : System.Windows.Application
    {
        public static SettingModel Setting { get; private set; }
        public static bool IsShuttingDown { get; set; }
        protected override void OnStartup(StartupEventArgs e)
        {
            Setting = AppHelpers.LoadAppSettings();
            ThemeManager.ApplyDefaultTheme();
            base.OnStartup(e);
        }
        protected override void OnExit(ExitEventArgs e)
        {
            IsShuttingDown = true;
            App.Setting.Save();
            base.OnExit(e);
        }
    }
}
