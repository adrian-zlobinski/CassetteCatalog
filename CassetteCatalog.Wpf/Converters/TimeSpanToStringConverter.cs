using System.Globalization;
using System.Windows.Data;

namespace CassetteCatalog.Wpf.Converters
{
    public class TimeSpanToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is TimeSpan ts)
            {
                // Math.Floor zapobiega zaokrąglaniu w górę przy sekundach
                int totalMinutes = (int)Math.Floor(ts.TotalMinutes);
                return $"{totalMinutes:00}:{ts.Seconds:00}";
            }
            return "00:00";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            //próba dla formatu m:ss
            if (value is string str && TimeSpan.TryParseExact(str, @"m\:ss", CultureInfo.InvariantCulture, out TimeSpan result))
            {
                return result;
            }
            //próba dla formatu mm:ss
            if (value is string str2 && TimeSpan.TryParseExact(str2, @"mm\:ss", CultureInfo.InvariantCulture, out TimeSpan result2))
            {
                return result2;
            }
            return TimeSpan.Zero;
        }
    }
}
