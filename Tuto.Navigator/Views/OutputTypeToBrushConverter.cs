using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;
using Tuto.Model;

namespace Tuto.Navigator.Views
{
	class OutputTypeToBrushConverter :  IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			var tp = (OutputTypes)value;
			switch(tp)
			{
				case OutputTypes.None: return Brushes.LightPink;
				case OutputTypes.Patch: return Brushes.LightSeaGreen;
				default: return Brushes.LightGray;
			}
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
