using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using TeacherPreferencesDataModel;

namespace TeacherPreferencesUI.Converters
{
    public class PreferenceTypeToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is GeneralPreferenceType preferenceType)
            {
                if (preferenceType == GeneralPreferenceType.MaxDaysPerWeek ||
                    preferenceType == GeneralPreferenceType.MinDaysPerWeek ||
                    preferenceType == GeneralPreferenceType.FreeDaysPerWeek ||
                    preferenceType == GeneralPreferenceType.MaxClassesPerDay ||
                    preferenceType == GeneralPreferenceType.MinClassesPerDay)
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
