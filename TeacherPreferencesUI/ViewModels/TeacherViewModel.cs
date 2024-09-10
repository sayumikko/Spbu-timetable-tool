using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using TeacherPreferencesDataModel;
using TeacherPreferencesUI.ViewModels.SpecificPreferenceViewModels;

namespace TeacherPreferencesUI.ViewModels
{
    public class TeacherViewModel : INotifyPropertyChanged
    {
        string? name;
        string? surname;
        string? patronymic;
        Department department;
        private ObservableCollection<GeneralPreferenceViewModel> generalPreferences;
        private ObservableCollection<CourseViewModel> courses;
        private ObservableCollection<TimeSlotViewModel> timeslots;

        public int Id { get; set; }

        public string? Name
        {
            get { return name; }
            set
            {
                name = value;
                OnPropertyChanged();
            }
        }

        public string? Surname
        {
            get { return surname; }
            set
            {
                surname = value;
                OnPropertyChanged();
            }
        }

        public string? Patronymic
        {
            get { return patronymic; }
            set
            {
                patronymic = value;
                OnPropertyChanged();
            }
        }

        public Department Department
        {
            get { return department; }
            set
            {
                department = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<GeneralPreferenceViewModel> GeneralPreferences
        {
            get { return generalPreferences; }
            set
            {
                generalPreferences = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<CourseViewModel> Courses
        {
            get { return courses; }
            set
            {
                courses = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<TimeSlotViewModel> TimeSlots
        {
            get { return timeslots; }
            set
            {
                timeslots = value;
                OnPropertyChanged();
            }
        }

        public string FullName => $"{Surname} {Name} {Patronymic}".Trim();

        public static TeacherViewModel FromModel(Teacher teacher, ApplicationViewModel appViewModel)
        {
            return new TeacherViewModel
            {
                Id = teacher.Id,
                Name = teacher.Name,
                Surname = teacher.Surname,
                Patronymic = teacher.Patronymic,
                Department = teacher.Department,
                GeneralPreferences = new ObservableCollection<GeneralPreferenceViewModel>(
                    teacher.GeneralPreferences?.Select(GeneralPreferenceViewModel.FromModel)
                    ?? Enumerable.Empty<GeneralPreferenceViewModel>()),
                Courses = new ObservableCollection<CourseViewModel>(
                    teacher.Courses?.Select(sp => CourseViewModel.FromModel(sp))
                    ?? Enumerable.Empty<CourseViewModel>()),
                TimeSlots = new ObservableCollection<TimeSlotViewModel>(
                    teacher.TimeSlots?.Select(sp => TimeSlotViewModel.FromModel(sp))
                    ?? Enumerable.Empty<TimeSlotViewModel>())
            };
        }


        public Teacher ToModel()
        {
            return new Teacher
            {
                Id = this.Id,
                Name = this.Name,
                Surname = this.Surname,
                Patronymic = this.Patronymic,
                Department = this.Department,
                GeneralPreferences = this.GeneralPreferences?.Select(gp => gp.ToModel()).ToList(),
                Courses = this.Courses?.Select(sp => sp.ToModel()).ToList(),
                TimeSlots = this.TimeSlots?.Select(sp => sp.ToModel()).ToList()
            };
        }


        public event PropertyChangedEventHandler? PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}
