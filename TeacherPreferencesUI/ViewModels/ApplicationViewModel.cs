using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Microsoft.EntityFrameworkCore;
using TeacherPreferencesDataModel;
using TeacherPreferencesUI.Commands;
using TeacherPreferencesUI.UserControls;
using System.Windows.Data;
using TeacherPreferencesUI.ViewModels.SpecificPreferenceViewModels;
namespace TeacherPreferencesUI.ViewModels
{
    public class ApplicationViewModel : INotifyPropertyChanged
    {
        public ApplicationContext db = new ApplicationContext();

        public ObservableCollection<TeacherViewModel> Teachers { get; set; }
        public ICollectionView SortedTeachers { get; private set; }

        public ObservableCollection<Department> Departments { get; set; }
        public ObservableCollection<GeneralPreferenceViewModel> GeneralPreferences { get; set; }
        public ObservableCollection<CourseViewModel> Courses { get; set; }
        public ObservableCollection<TimeSlotViewModel> TimeSlots { get; set; }

        private TeacherViewModel? editingTeacher;
        public TeacherViewModel? EditingTeacher
        {
            get { return editingTeacher; }
            set
            {
                editingTeacher = value;
                OnPropertyChanged();
            }
        }


        private object currentView;
        public object CurrentView
        {
            get { return currentView; }
            set
            {
                currentView = value;
                OnPropertyChanged();
            }
        }

        public bool HasValidationErrors => ValidationResult?.Count > 0;


        private ObservableCollection<string> validationResult = new ObservableCollection<string>();
        public ObservableCollection<string> ValidationResult
        {
            get { return validationResult; }
            set
            {
                validationResult = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<string> errors = new ObservableCollection<string>();
        public ObservableCollection<string> Errors
        {
            get { return errors; }
            set
            {
                errors = value;
                OnPropertyChanged();
            }
        }

        private string errorMessage;
        public string ErrorMessage
        {
            get { return errorMessage; }
            set
            {
                errorMessage = value;
                OnPropertyChanged();
            }
        }

        private TeacherViewModel? selectedTeacher;
        public TeacherViewModel? SelectedTeacher
        {
            get { return selectedTeacher; }
            set
            {
                selectedTeacher = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsTeacherSelected));

                if (selectedTeacher != null)
                {
                    CurrentView = new TeacherPreferenceUserControl { DataContext = this };
                }
                else
                {
                    CurrentView = new InitialView();
                }
            }
        }

        public ICollectionView FilteredTeachers
        {
            get
            {
                var filteredTeachers = new CollectionViewSource { Source = Teachers }.View;
                filteredTeachers.Filter = teacher =>
                {
                    var t = teacher as TeacherViewModel;
                    return t != null && t != SelectedTeacher;
                };
                return filteredTeachers;
            }
        }

        public bool IsTeacherSelected => SelectedTeacher != null;

        public ApplicationViewModel()
        {
            db.Database.EnsureCreated();

            db.Teachers.Load();
            db.Departments.Load();
            db.GeneralPreferences.Load();
            db.Courses.Load();
            db.SpecificPreferences.Load();
            db.Audiences.Load();
            db.AudienceEquipments.Load();
            db.TimeSlots.Load();

            Teachers = new ObservableCollection<TeacherViewModel>(
                db.Teachers.Local.Select(t => TeacherViewModel.FromModel(t, this))
            );

            SortedTeachers = CollectionViewSource.GetDefaultView(Teachers);
            SortedTeachers.SortDescriptions.Add(new SortDescription(nameof(TeacherViewModel.FullName), ListSortDirection.Ascending));

            Departments = new ObservableCollection<Department>(db.Departments.Local.ToObservableCollection());

            GeneralPreferences = new ObservableCollection<GeneralPreferenceViewModel>(
                db.GeneralPreferences.Local.Select(gp => GeneralPreferenceViewModel.FromModel(gp))
            );

            Courses = new ObservableCollection<CourseViewModel>(
                db.Courses.Local.Select(sp => CourseViewModel.FromModel(sp))
            );

            TimeSlots = new ObservableCollection<TimeSlotViewModel>(
                db.TimeSlots.Local.Select(ts => TimeSlotViewModel.FromModel(ts))
            );

            CurrentView = new InitialView();
        }

        public RelayCommand OpenFileCommand => Parser.OpenFileCommand(this);
        public RelayCommand LoadAcademicLoadCommand => Parser.LoadAcademicLoadCommand(this);
        public RelayCommand AddCommand => Commands.AddCommand(this);
        public RelayCommand DeleteCommand => Commands.DeleteCommand(this);
        public RelayCommand SaveTeacherCommand => Commands.DeleteTimeSlotCommand(this);
        public RelayCommand ClearAllTeachersCommand => Commands.ClearAllTeachersCommand(this);
        public RelayCommand ErrorBackCommand => Commands.ErrorBackCommand(this);
        public RelayCommand SwitchToParsingResultCommand => Commands.SwitchToParsingResultCommand(this);
        public RelayCommand BackCommand => Commands.BackCommand(this);
        public RelayCommand SavePreferencesCommand => Commands.SavePreferencesCommand(this);
        public RelayCommand DeletePreferenceCommand => Commands.DeletePreferenceCommand(this);
        public RelayCommand AddDefaultPreferenceCommand => Commands.AddDefaultPreferenceCommand(this);
        public RelayCommand AddCourseCommand => Commands.AddCourseCommand(this);
        public RelayCommand DeleteCourseCommand => Commands.DeleteCourseCommand(this);
        public RelayCommand AddAudienceCommand => Commands.AddAudienceCommand(this);
        public RelayCommand AddAudienceEquipCommand => Commands.AddAudienceEquipCommand(this);
        public RelayCommand DeleteAudienceCommand => Commands.DeleteAudienceCommand(this);
        public RelayCommand DeleteAudienceEquipCommand => Commands.DeleteAudienceEquipCommand(this);
        public RelayCommand AddDefaultSpecificPreference => Commands.AddDefaultSpecificPreference(this);
        public RelayCommand DeleteSpecificPreferenceCommand => Commands.DeleteSpecificPreferenceCommand(this);
        public RelayCommand AddTimeSlotCommand => Commands.AddTimeSlotCommand(this);
        public RelayCommand DeleteTimeSlotCommand => Commands.DeleteTimeSlotCommand(this);


        public event PropertyChangedEventHandler? PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}
