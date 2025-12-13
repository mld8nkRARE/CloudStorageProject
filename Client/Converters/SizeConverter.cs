// Создай папку Converters в проекте Client
// SizeConverter.cs
using System;
using System.Globalization;
using System.Windows.Data;

namespace Client.Converters
{
    public class SizeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return "Не указан";

            if (value is long size)
            {
                if (size == 0)
                    return "0 байт";

                return $"{size:N0} байт";
            }

            return value?.ToString() ?? "Не указан";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}