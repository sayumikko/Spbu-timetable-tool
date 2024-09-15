using System.Globalization;
using System.Windows.Data;
using TeacherPreferencesDataModel;

namespace TeacherPreferencesUI.Converters
{
    public class GeneralPreferencePriorityFilterConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is GeneralPreferenceType preferenceType)
            {
                switch (preferenceType)
                {
                    case GeneralPreferenceType.MaxDaysPerWeek:
                        return new List<Priority> { Priority.Mandatory, Priority.Desirable, Priority.Neutral };

                    case GeneralPreferenceType.MinDaysPerWeek:
                        return new List<Priority> { Priority.Mandatory, Priority.Desirable, Priority.Neutral };

                    case GeneralPreferenceType.MaxClassesPerDay:
                        return new List<Priority> { Priority.Mandatory, Priority.Desirable, Priority.Neutral };

                    case GeneralPreferenceType.MinClassesPerDay:
                        return new List<Priority> { Priority.Mandatory, Priority.Desirable, Priority.Neutral };

                    default:
                        return Enum.GetValues(typeof(Priority)).Cast<Priority>().ToList();
                }
            }
            return Enum.GetValues(typeof(Priority)).Cast<Priority>().ToList();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

}