using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using TeacherPreferencesUI.Commands;
using TeacherPreferencesDataModel;
using TeacherPreferencesUI.ViewModels.SpecificPreferenceViewModels;

namespace TeacherPreferencesUI.ViewModels
{
    public class CourseViewModel : INotifyPropertyChanged
    {
        private string subjectName;
        private string group;
        private ClassType classType;
        private int teacherId;
        private ObservableCollection<SpecificPreferenceViewModel> specificPreferences = new();
        private ObservableCollection<AudienceViewModel> audiences = new();
        private ObservableCollection<AudienceEquipmentViewModel> audienceEquipments = new();

        public string FullInfoAboutCourse => $"{subjectName}, {group}, {classType}";

        public int Id { get; set; }

        public string SubjectName
        {
            get { return subjectName; }
            set
            {
                subjectName = value;
                OnPropertyChanged();
            }
        }

        public string Group
        {
            get { return group; }
            set
            {
                group = value;
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

        public ClassType ClassType
        {
            get { return classType; }
            set
            {
                classType = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<SpecificPreferenceViewModel> SpecificPreferences
        {
            get { return specificPreferences; }
            set
            {
                specificPreferences = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<AudienceViewModel> Audiences
        {
            get { return audiences; }
            set
            {
                audiences = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<AudienceEquipmentViewModel> AudienceEquipments
        {
            get { return audienceEquipments; }
            set
            {
                audienceEquipments = value;
                OnPropertyChanged();
            }
        }

        private bool isEditing;
        public bool IsEditing
        {
            get { return isEditing; }
            set
            {
                isEditing = value;
                OnPropertyChanged();
            }
        }

        public RelayCommand EditCommand => EditSPCommand(this);
        public RelayCommand SaveCommand => SaveSPCommand(this);

        public static RelayCommand EditSPCommand(CourseViewModel viewModel)
        {
            return new RelayCommand((o) =>
            {
                viewModel.IsEditing = true;
            });
        }

        public static RelayCommand SaveSPCommand(CourseViewModel viewModel)
        {
            return new RelayCommand((o) =>
            {
                viewModel.IsEditing = false;
            });
        }

        public static CourseViewModel FromModel(Course course)
        {
            var courseViewModel = new CourseViewModel
            {
                Id = course.Id,
                SubjectName = course.SubjectName,
                Group = course.Group,
                ClassType = course.ClassType,
                TeacherId = course.TeacherId,
            };

            courseViewModel.Audiences = new ObservableCollection<AudienceViewModel>(
                course.Audiences.Select(a => AudienceViewModel.FromModel(a, courseViewModel)).ToList()
            );

            courseViewModel.AudienceEquipments = new ObservableCollection<AudienceEquipmentViewModel>(
                course.AudienceEquipments.Select(e => AudienceEquipmentViewModel.FromModel(e, courseViewModel)).ToList()
            );

            courseViewModel.SpecificPreferences = new ObservableCollection<SpecificPreferenceViewModel>(
                course.SpecificPreferences.Select(e => SpecificPreferenceViewModel.FromModel(e)).ToList()
            );

            return courseViewModel;
        }


        public Course ToModel()
        {
            var audiences = Audiences.Select(a => a.ToModel()).ToList();
            var audienceEquipments = AudienceEquipments.Select(e => e.ToModel()).ToList();
            var specificPreferences = SpecificPreferences.Select(sp => sp.ToModel()).ToList();

            return new Course
            {
                Id = this.Id,
                SubjectName = this.SubjectName,
                Group = this.Group,
                ClassType = this.ClassType,
                TeacherId = this.TeacherId,
                Audiences = audiences,
                AudienceEquipments = audienceEquipments,
                SpecificPreferences = specificPreferences
            };
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }
    }

}
