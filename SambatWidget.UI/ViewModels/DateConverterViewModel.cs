using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SambatWidget.Core;

namespace SambatWidget.UI.ViewModels
{
    public partial class DateConverterViewModel : ObservableObject
    {
        [ObservableProperty]
        string englishDate = DateTime.Now.ToString("yyyy/MM/dd");

        [ObservableProperty]
        string nepaliDate = DateConverter.GetCurrentNepaliDate();

        [ObservableProperty]
        string convertedNepaliDate = null!;

        [ObservableProperty]
        string convertedEnglishDate = null!;

        [RelayCommand]
        private void ConvertToEnglish()
        {
            ConvertedEnglishDate = NepaliDate.ConvertToEnglishDate().ConvertedDate;
        }
        [RelayCommand]
        private void ConvertToNepali()
        {
            ConvertedNepaliDate = EnglishDate.ConvertToNepaliDate().ConvertedDate;
        }
    }
}
