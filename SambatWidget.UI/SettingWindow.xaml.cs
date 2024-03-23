using SambatWidget.UI.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SambatWidget.UI
{
    /// <summary>
    /// Interaction logic for SettingWindow.xaml
    /// </summary>
    public partial class SettingWindow : Window
    {
        public SettingWindow()
        {
            InitializeComponent();
        }

        private void EnglishDateTxtBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if(DataContext is SettingViewModel vm)
            {
                vm.DateConverterViewModel.ConvertToNepaliCommand.Execute(null);
            }
        }

        private void NepaliDateTxtBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (DataContext is SettingViewModel vm)
            {
                vm.DateConverterViewModel.ConvertToEnglishCommand.Execute(null);
            }
        }
    }
}
