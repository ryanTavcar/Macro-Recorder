using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace Names.Helpers
{
    public class DarkenColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is SolidColorBrush brush && parameter is string paramString && int.TryParse(paramString, out int percentage))
            {
                Color originalColor = brush.Color;
                double factor = 1.0 - (percentage / 100.0);

                byte r = (byte)Math.Max(0, originalColor.R * factor);
                byte g = (byte)Math.Max(0, originalColor.G * factor);
                byte b = (byte)Math.Max(0, originalColor.B * factor);

                return new SolidColorBrush(Color.FromArgb(originalColor.A, r, g, b));
            }

            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
