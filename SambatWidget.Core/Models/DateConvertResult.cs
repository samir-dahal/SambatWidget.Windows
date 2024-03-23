namespace SambatWidget.Core.Models
{
    public class DateConvertResult
    {
        public DateConvertResult(bool isValid, string convertedDate)
        {
            IsValid = isValid;
            ConvertedDate = convertedDate;
        }
        public bool IsValid { get; }
        public string ConvertedDate { get; }
    }
}
