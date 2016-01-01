using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace Tuto.Navigator
{
    public class BoolToVisibilityConverter : IValueConverter
    {
        public object Convert(object _value, Type targetType, object _parameter, System.Globalization.CultureInfo culture)
        {
            var value = (bool)_value;
            return value ? Visibility.Visible : Visibility.Collapsed;
        }
        public object ConvertBack(object _value, Type targetType, object _parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
