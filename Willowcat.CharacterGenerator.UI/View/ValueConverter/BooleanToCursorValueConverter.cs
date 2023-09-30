using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Input;

namespace Willowcat.CharacterGenerator.UI.View.ValueConverter
{
    public class BooleanToCursorValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            object result = Cursors.Arrow;
            if ((bool)value)
            {
                result = Cursors.Wait;
            }
            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
