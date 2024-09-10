using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using TeacherPreferencesUI.ViewModels;

namespace TeacherPreferencesUI.Converters
{
    public class TeacherIdToTeacherConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int teacherId && parameter is ObservableCollection<TeacherViewModel> teachers)
            {
                return teachers.FirstOrDefault(t => t.Id == teacherId);
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is TeacherViewModel teacher)
            {
                return teacher.Id;
            }
            return null;
        }
    }
}
