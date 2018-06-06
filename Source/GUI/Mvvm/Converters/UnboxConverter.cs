using System;
using System.Windows.Data;

namespace OFDRExtractor.GUI.Mvvm.Converters
{
	/// <summary>
	/// if target type is ValueType, then boxed Data will cause a convertion error
	/// </summary>
	sealed class UnboxConverter : IValueConverter
	{
		object IValueConverter.Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			return Unbox(value, targetType);
		}

		object IValueConverter.ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			return Unbox(value, targetType);
		}

		private object Unbox(object value, Type targetType)
		{
			if (targetType.IsValueType && value == null)
				return Activator.CreateInstance(targetType);
			return value;
		}
	}
}
