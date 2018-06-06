using System;
using System.Windows;
using System.Windows.Data;

namespace OFDRExtractor.GUI.Mvvm.Converters
{
	public class AntiVisibilityConverter : IValueConverter
	{
		object IValueConverter.Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			if (value != null && value is bool)
			{
				return (bool)value ? Visibility.Collapsed : Visibility.Visible;
			}
			return Visibility.Visible;
		}

		object IValueConverter.ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
