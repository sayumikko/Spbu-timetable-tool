using System;
using TeacherPreferencesDataModel;

namespace TeacherPreferencesUI.Converters
{
    public static class EnumSource
    {
        public static Array GeneralPreferenceTypes => Enum.GetValues(typeof(GeneralPreferenceType));
        public static Array Priorities => Enum.GetValues(typeof(Priority));
        public static Array DaysOfWeek => Enum.GetValues(typeof(TeacherPreferencesDataModel.DayOfWeek));
        public static Array AudienceEquipmentTypes => Enum.GetValues(typeof(AudienceEquipmentType));
        public static Array ClassTypes => Enum.GetValues(typeof(ClassType));
        public static Array SpecificPreferenceTypes => Enum.GetValues(typeof(SpecificPreferenceType));
    }
}
