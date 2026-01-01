using CassetteCatalog.Core.Enums;
using System.Globalization;
using System.Windows.Data;

namespace CassetteCatalog.Wpf.Converters
{
    public class TapeTypeToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value is eTapeType tapeType)
            {
                return tapeType switch
                {
                    eTapeType.TypeI_Fe => "Type I (Fe)",
                    eTapeType.TypeII_CrO2 => "Type II (CrO2)",
                    eTapeType.TypeIII_FeCr => "Type III (FeCr)",
                    eTapeType.TypeIV_Metal => "Type IV (Metal)",
                    _ => "Unknown Type"
                };
            }
            return value?.ToString() ?? string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
