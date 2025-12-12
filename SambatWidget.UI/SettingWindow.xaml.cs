using SambatWidget.UI.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace SambatWidget.UI
{
    /// <summary>
    /// Interaction logic for SettingWindow.xaml
    /// </summary>
    public partial class SettingWindow : Window
    {
        private readonly SettingViewModel _vm;
        private readonly int _initialTabIndex;
        public SettingWindow(int tabIndex = 0)
        {
            _initialTabIndex = tabIndex;
            InitializeComponent();
            DataContext = _vm = new SettingViewModel();
            Loaded += SettingWindow_Loaded;
        }

        private async void SettingWindow_Loaded(object sender, RoutedEventArgs e)
        {
            TabControl.SelectedIndex = _initialTabIndex;
            await _vm.GetTimeZonesAsync();
        }

        private void EnglishDateTxtBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            _vm.DateConverterViewModel.ConvertToNepaliCommand.Execute(null);
        }

        private void NepaliDateTxtBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            _vm.DateConverterViewModel.ConvertToEnglishCommand.Execute(null);
        }
    }
}
