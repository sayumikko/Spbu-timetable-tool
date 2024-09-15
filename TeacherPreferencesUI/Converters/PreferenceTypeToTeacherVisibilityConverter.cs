using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using TeacherPreferencesDataModel;

namespace TeacherPreferencesUI.Converters
{
    public class PreferenceTypeToTeacherVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is GeneralPreferenceType preferenceType)
            {
                if (preferenceType == GeneralPreferenceType.IntersectTimeWithTeacher)
                {
                    return Visibility.Visible;
                }
            }
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
