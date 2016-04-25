using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Tuto.BatchWorks
{
    public class StatusConverterForeground : IValueConverter
    {
        //for foregrounds
        public Dictionary<BatchWorkStatus, string> colors = new Dictionary<BatchWorkStatus, string>()
        {
            {BatchWorkStatus.Running, "LightGreen" },
            {BatchWorkStatus.Aborted, "Red" },
            {BatchWorkStatus.Cancelled, "DarkGray" },
            {BatchWorkStatus.Pending, "White" },
            {BatchWorkStatus.Failure, "Red" },
            {BatchWorkStatus.Success, "LightGreen" },
            {BatchWorkStatus.Attention, "Yellow" }
        };

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return colors[(BatchWorkStatus)value];
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
    public class StatusConverterBackGround : IValueConverter
    {
        //for backgrounds
        public Dictionary<BatchWorkStatus, string> colors = new Dictionary<BatchWorkStatus, string>()
        {
            {BatchWorkStatus.Running, "LightGray" },
            {BatchWorkStatus.Aborted, "Red" },
            {BatchWorkStatus.Cancelled, "DarkGray" },
            {BatchWorkStatus.Pending, "LightGray" },
            {BatchWorkStatus.Failure, "Red" },
            {BatchWorkStatus.Success, "LightGray" },
            {BatchWorkStatus.Attention, "LightGray" }
        };

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return colors[(BatchWorkStatus)value];
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}
