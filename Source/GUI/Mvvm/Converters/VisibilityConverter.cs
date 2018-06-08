using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;

namespace OFDRExtractor.GUI.Mvvm.Converters
{
	sealed class VisibilityConverter : IValueConverter
	{
		object IValueConverter.Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			if (value != null && value is bool)
				return (bool)value ? Visibility.Visible : Visibility.Collapsed;
			return Visibility.Hidden;
		}

		object IValueConverter.ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			if (value != null && value is Visibility)
				return (Visibility)value == Visibility.Visible;
			return false;
		}
	}
}
