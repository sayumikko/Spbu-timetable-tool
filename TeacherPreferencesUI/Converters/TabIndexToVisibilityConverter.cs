using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace TeacherPreferencesUI.Converters
{
    public class TabIndexToVisibilityConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            int selectedIndex = (int)values[0];
            string buttonType = parameter as string;

            switch (selectedIndex)
            {
                case 0:
                    return (buttonType == "AddPreference" || buttonType == "Back" || buttonType == "Save") ? Visibility.Visible : Visibility.Collapsed;
                case 1:
                    return (buttonType == "AddCourse" || buttonType == "Back" || buttonType == "Save") ? Visibility.Visible : Visibility.Collapsed;
                case 2:
                    return (buttonType == "Back" || buttonType == "Save") ? Visibility.Visible : Visibility.Collapsed;
                default:
                    return Visibility.Collapsed;
            }
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
