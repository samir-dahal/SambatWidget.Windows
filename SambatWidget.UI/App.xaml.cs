using SambatWidget.UI.Models;
using System.Configuration;
using System.Data;
using System.Windows;

namespace SambatWidget.UI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private static SettingModel _setting;
        public static SettingModel Setting
        {
            get
            {
                if(_setting is null)
                {
                    _setting = new SettingModel();
                }
                return _setting;
            }
        }
    }
}
