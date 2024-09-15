using System;
using System.Globalization;
using System.Windows.Data;
using TeacherPreferencesDataModel;

namespace TeacherPreferencesUI.Converters
{
    public class EnumDescriptionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Priority priority)
            {
                return priority switch
                {
                    Priority.Mandatory => "Обязательно",
                    Priority.Neutral => "Нейтральное отношение",
                    Priority.Desirable => "Желательно",
                    Priority.NotDesirable => "Нежелательно",
                    Priority.Avoidable => "Недопустимо",
                    _ => priority.ToString()
                };
            }

            else if (value is GeneralPreferenceType preference)
            {
                return preference switch
                {
                    GeneralPreferenceType.MaxDaysPerWeek => "Максимальное количество дней в неделю",
                    GeneralPreferenceType.MinDaysPerWeek => "Минимальное количество дней в неделю",
                    GeneralPreferenceType.FreeDaysPerWeek => "Количество свободных дней в неделю",
                    GeneralPreferenceType.MaxClassesPerDay => "Максимальное количество занятий в день",
                    GeneralPreferenceType.MinClassesPerDay => "Минимальное количество занятий в день",
                    GeneralPreferenceType.Compactness => "Компактность расписания",
                    GeneralPreferenceType.IntersectTimeWithTeacher => "Пересечение времени с другим преподавателем",
                    GeneralPreferenceType.Gaps => "Наличие промежутков между занятиями",
                    _ => preference.ToString()
                };
            }

            else if (value is SpecificPreferenceType specificPreference)
            {
                return specificPreference switch
                {
                    SpecificPreferenceType.UniteGroups => "Объединить группы",
                    SpecificPreferenceType.UniteClasses => "Ставить занятия подряд",
                    SpecificPreferenceType.AlternateBySubgroups => "Чередовать по подгруппам",
                    SpecificPreferenceType.UniteSubgroups => "Объединить подгруппы",
                    SpecificPreferenceType.OneClassInTwoWeeks => "Одно занятие в две недели",
                    SpecificPreferenceType.InOneDay => "Занятия в один день",
                    _ => specificPreference.ToString()
                };
            }

            else if (value is ClassType classType)
            {
                return classType switch
                {
                    ClassType.Lecture => "Лекция",
                    ClassType.Laboratory => "Лабораторная работа",
                    ClassType.Seminar => "Семинар",
                    ClassType.Practical => "Практическое занятие",
                    ClassType.None => "Без типа",
                    _ => classType.ToString()
                };
            }

            else if (value is TeacherPreferencesDataModel.DayOfWeek dayOfWeek)
            {
                return dayOfWeek switch
                {
                    TeacherPreferencesDataModel.DayOfWeek.AllWeek => "Вся неделя",
                    TeacherPreferencesDataModel.DayOfWeek.Monday => "Понедельник",
                    TeacherPreferencesDataModel.DayOfWeek.Tuesday => "Вторник",
                    TeacherPreferencesDataModel.DayOfWeek.Wednesday => "Среда",
                    TeacherPreferencesDataModel.DayOfWeek.Thursday => "Четверг",
                    TeacherPreferencesDataModel.DayOfWeek.Friday => "Пятница",
                    TeacherPreferencesDataModel.DayOfWeek.Saturday => "Суббота",
                    _ => dayOfWeek.ToString()
                };
            }

            else if (value is AudienceEquipmentType equipmentType)
            {
                return equipmentType switch
                {
                    AudienceEquipmentType.Projector => "Проектор",
                    AudienceEquipmentType.WhiteBoard => "Магнитно-маркерная доска",
                    AudienceEquipmentType.NumberOfBoards => "Количество досок",
                    AudienceEquipmentType.ComputerAudience => "Компьютерный класс",
                    AudienceEquipmentType.Computer => "Компьютер",
                    AudienceEquipmentType.TimeService => "Служба времени",
                    AudienceEquipmentType.Capacity => "Вместимость",
                    AudienceEquipmentType.Blackboard => "Классическая доска",
                    _ => equipmentType.ToString()
                };
            }

            return value?.ToString();
        }


        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
