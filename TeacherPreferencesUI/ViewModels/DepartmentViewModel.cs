using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using TeacherPreferencesDataModel;

namespace TeacherPreferencesUI.ViewModels
{
    public class DepartmentViewModel : INotifyPropertyChanged
    {
        private int departmentId;
        private string? name;
        private ObservableCollection<TeacherViewModel> teachers;

        public int DepartmentId
        {
            get { return departmentId; }
            set
            {
                departmentId = value;
                OnPropertyChanged();
            }
        }

        public string? Name
        {
            get { return name; }
            set
            {
                name = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<TeacherViewModel> Teachers
        {
            get { return teachers; }
            set
            {
                teachers = value;
                OnPropertyChanged();
            }
        }

        public static DepartmentViewModel FromModel(Department department, ApplicationViewModel appViewModel)
        {
            return new DepartmentViewModel
            {
                DepartmentId = department.DepartmentId,
                Name = department.Name,
                Teachers = new ObservableCollection<TeacherViewModel>(
                    department.Teachers?.Select(teacher => TeacherViewModel.FromModel(teacher, appViewModel))
                    ?? Enumerable.Empty<TeacherViewModel>())
            };
        }

        public Department ToModel()
        {
            return new Department
            {
                DepartmentId = this.DepartmentId,
                Name = this.Name,
                Teachers = this.Teachers?.Select(t => t.ToModel()).ToList()
            };
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null!)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
