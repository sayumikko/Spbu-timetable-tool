using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using TeacherPreferencesDataModel;
using TeacherPreferencesUI.ViewModels;

namespace TeacherPreferencesUI.Converters
{
    public class CapacityToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is AudienceEquipmentType equipmentType)
            {
                if (equipmentType == AudienceEquipmentType.Capacity || equipmentType == AudienceEquipmentType.NumberOfBoards)
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
