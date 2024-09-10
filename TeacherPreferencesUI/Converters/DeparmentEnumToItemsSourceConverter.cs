using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using TeacherPreferencesDataModel;

namespace TeacherPreferencesUI.Converters
{
    public class DepartmentEnumToItemsSourceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Enum.GetValues(typeof(Department))
                       .Cast<Department>()
                       .Where(e => e != Department.None)
                       .Select(DepartmentConverter.ConvertEnumToString)
                       .ToList();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
