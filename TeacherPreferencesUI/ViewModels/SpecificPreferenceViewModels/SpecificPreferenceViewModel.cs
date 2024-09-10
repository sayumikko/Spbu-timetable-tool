using DocumentFormat.OpenXml.Wordprocessing;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using TeacherPreferencesUI.Commands;
using TeacherPreferencesDataModel;

namespace TeacherPreferencesUI.ViewModels.SpecificPreferenceViewModels
{
    public class SpecificPreferenceViewModel : INotifyPropertyChanged
    {
        private SpecificPreferenceType specificPreferenceType;
        private Priority priority;

        public int Id { get; set; }
        public int CourseId { get; set; }

        public SpecificPreferenceType SpecificPreferenceType
        {
            get { return specificPreferenceType; }
            set
            {
                specificPreferenceType = value;
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

        public static SpecificPreferenceViewModel FromModel(SpecificPreference preference)
        {
            return new SpecificPreferenceViewModel()
            {
                Id = preference.Id,
                CourseId = preference.CourseId,
                SpecificPreferenceType = preference.PreferenceType,
                Priority = preference.Priority
            };
        }

        public SpecificPreference ToModel()
        {
            return new SpecificPreference
            {
                Id = this.Id,
                CourseId = this.CourseId,
                PreferenceType = this.SpecificPreferenceType,
                Priority = this.Priority
            };
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }
    }
}
