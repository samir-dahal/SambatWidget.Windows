using System.Globalization;
using System.Windows.Data;

namespace SambatWidget.UI.Converters
{
    public class EnglishToNepaliNumberConverter : IValueConverter
    {
        private static char[] _digits = new char[] { '०', '१', '२', '३', '४', '५', '६', '७', '८', '९' };
        private string ConvertToNepali(int number)
        {
            var result = new List<char>();
            int numDigits = (int)Math.Floor(Math.Log10(number) + 1);
            for (int i = numDigits - 1; i >= 0; i--)
            {
                int divisor = (int)Math.Pow(10, i);
                int digit = (number / divisor) % 10;
                result.Add(_digits[digit]);
            }
            return string.Join("", result);
        }
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (App.Setting.IsNepali)
            {
                return ConvertToNepali((int)value);
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
