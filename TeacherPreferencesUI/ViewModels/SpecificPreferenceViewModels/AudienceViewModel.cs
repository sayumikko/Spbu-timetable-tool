using System.ComponentModel;
using System.Runtime.CompilerServices;
using TeacherPreferencesDataModel;

namespace TeacherPreferencesUI.ViewModels
{
    public class AudienceViewModel : INotifyPropertyChanged
    {
        private int number;
        private Priority priority;
        private int courseId;

        public int Id { get; set; }

        public int Number
        {
            get { return number; }
            set
            {
                number = value;
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

        public int CourseId
        {
            get { return courseId; }
            set
            {
                courseId = value;
                OnPropertyChanged();
            }
        }

        public CourseViewModel ParentCourse { get; set; }

        public static AudienceViewModel FromModel(Audience audience, CourseViewModel parent)
        {
            return new AudienceViewModel
            {
                Id = audience.Id,
                Number = audience.Number,
                Priority = audience.Priority,
                CourseId = audience.CourseId,
                ParentCourse = parent
            };
        }

        public Audience ToModel()
        {
            return new Audience
            {
                Id = this.Id,
                Number = this.Number,
                Priority = this.Priority,
                CourseId = this.CourseId
            };
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }
    }

}