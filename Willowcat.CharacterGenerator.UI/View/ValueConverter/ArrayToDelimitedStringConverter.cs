using System;
using System.Collections;
using System.Globalization;
using System.Text;
using System.Windows.Data;

namespace Willowcat.CharacterGenerator.UI.View.ValueConverter
{
    public class ArrayToDelimitedStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            object result = string.Empty;
            string delimiter = parameter?.ToString() ?? ", ";
            if (value is IEnumerable list)
            {
                StringBuilder builder = new();
                foreach (object item in list)
                {
                    if (builder.Length > 0)
                    {
                        builder.Append(delimiter);
                    }
                    builder.Append(item);
                }
                result = builder.ToString();
            }
            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
