using SambatWidget.Core;
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
        public static bool IsShuttingDown { get; set; }
        protected override void OnStartup(StartupEventArgs e)
        {
            Setting = AppHelpers.LoadAppSettings();

            bool startupEnabled = AppHelpers.IsAutoStartAtStartupEnabled();
            if (Setting.AutoRunAtStartup && !startupEnabled)
            {
                startupEnabled = AppHelpers.EnableAutoStartAtStartup();
            }

            if (Setting.AutoRunAtStartup != startupEnabled)
            {
                Setting.Save(x => x.AutoRunAtStartup = startupEnabled);
            }

            ThemeManager.ApplyDefaultTheme();
            base.OnStartup(e);
        }
        protected override void OnExit(ExitEventArgs e)
        {
            IsShuttingDown = true;
            App.Setting.Save();
            AppHelpers.FlushPendingSettingsWrites();
            base.OnExit(e);
        }
    }
}
