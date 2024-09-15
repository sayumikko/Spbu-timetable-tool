using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using TeacherPreferencesDataModel;
using TeacherPreferencesUI.ViewModels.SpecificPreferenceViewModels;

namespace TeacherPreferencesUI.ViewModels
{
    public class GeneralPreferenceViewModel : INotifyPropertyChanged
    {
        private GeneralPreferenceType preferenceType;
        private int? intValue;
        private Priority priority;
        private int teacherId;
        private int? teacherRefId;

        public int Id { get; set; }

        public GeneralPreferenceType PreferenceType
        {
            get { return preferenceType; }
            set
            {
                preferenceType = value;
                OnPropertyChanged();
            }
        }

        public int? IntValue
        {
            get { return intValue; }
            set
            {
                intValue = value;
                OnPropertyChanged();
            }
        }

        public Priority Priority
        {
            get { return priority; }
            set
            {
                priority = value;
                OnPropertyChanged();
            }
        }

        public int TeacherId
        {
            get { return teacherId; }
            set
            {
                teacherId = value;
                OnPropertyChanged();
            }
        }

        public int? TeacherRefId
        {
            get { return teacherRefId; }
            set
            {
                teacherRefId = value;
                OnPropertyChanged();
            }
        }


        public static GeneralPreferenceViewModel FromModel(GeneralPreference preference)
        {
            return new GeneralPreferenceViewModel
            {
                Id = preference.Id,
                TeacherId = preference.TeacherId,
                PreferenceType = preference.PreferenceType,
                IntValue = preference.IntValue,
                Priority = preference.Priority,
                TeacherRefId = preference.TeacherRefId
            };
        }

        public GeneralPreference ToModel()
        {
            return new GeneralPreference
            {
                Id = this.Id,
                TeacherId = this.TeacherId,
                PreferenceType = this.PreferenceType,
                IntValue = this.IntValue,
                Priority = this.Priority,
                TeacherRefId = this.TeacherRefId,
            };
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }
    }
}
