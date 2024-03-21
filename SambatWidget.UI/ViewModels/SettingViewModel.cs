using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SambatWidget.UI.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SambatWidget.UI.ViewModels
{
    public partial class SettingViewModel : ViewModelBase
    {
        private KeyValuePair<string, string> previousTheme;
        public IDictionary<string, string> Themes { get; set; } = ThemeManager.Themes;

        [ObservableProperty]
        KeyValuePair<string, string> selectedTheme;

        [RelayCommand]
        private void ApplyChanges()
        {
            if (selectedTheme.Key != previousTheme.Key)
            {
                ThemeManager.ApplyTheme(selectedTheme.Value);
                previousTheme = selectedTheme;
            }
        }
    }
}
