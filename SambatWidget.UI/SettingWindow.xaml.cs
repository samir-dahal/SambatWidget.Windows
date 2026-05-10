using SambatWidget.UI.ViewModels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace SambatWidget.UI
{
    /// <summary>
    /// Interaction logic for SettingWindow.xaml
    /// </summary>
    public partial class SettingWindow : Window
    {
        private readonly SettingViewModel _vm;
        private readonly int _initialTabIndex;
        private readonly DispatcherTimer _dateConvertDebounceTimer;
        private Action? _pendingDateConversion;
        public SettingWindow(int tabIndex = 0)
        {
            _initialTabIndex = tabIndex;
            InitializeComponent();
            DataContext = _vm = new SettingViewModel();
            Loaded += SettingWindow_Loaded;

            _dateConvertDebounceTimer = new DispatcherTimer();
            _dateConvertDebounceTimer.Interval = TimeSpan.FromMilliseconds(200);
            _dateConvertDebounceTimer.Tick += DateConvertDebounceTimer_Tick;
        }

        private async void SettingWindow_Loaded(object sender, RoutedEventArgs e)
        {
            TabControl.SelectedIndex = _initialTabIndex;
            await _vm.GetTimeZonesAsync();
        }

        private void EnglishDateTxtBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            QueueDateConversion(() => _vm.DateConverterViewModel.ConvertToNepaliCommand.Execute(null));
        }

        private void NepaliDateTxtBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            QueueDateConversion(() => _vm.DateConverterViewModel.ConvertToEnglishCommand.Execute(null));
        }

        private void QueueDateConversion(Action conversionAction)
        {
            _pendingDateConversion = conversionAction;
            _dateConvertDebounceTimer.Stop();
            _dateConvertDebounceTimer.Start();
        }

        private void DateConvertDebounceTimer_Tick(object? sender, EventArgs e)
        {
            _dateConvertDebounceTimer.Stop();
            _pendingDateConversion?.Invoke();
            _pendingDateConversion = null;
        }
    }
}
