using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace TeacherPreferencesUI.Converters
{
    public class ZeroToCollapsedConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int count = (int)value;
            bool reverse = parameter != null && bool.Parse(parameter.ToString());
            if (reverse)
            {
                return count == 0 ? Visibility.Visible : Visibility.Collapsed;
            }
            return count == 0 ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
