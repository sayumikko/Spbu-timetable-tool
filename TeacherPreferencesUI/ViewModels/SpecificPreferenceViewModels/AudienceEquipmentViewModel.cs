using System.ComponentModel;
using System.Runtime.CompilerServices;
using TeacherPreferencesDataModel;

namespace TeacherPreferencesUI.ViewModels
{
    public class AudienceEquipmentViewModel : INotifyPropertyChanged
    {
        private AudienceEquipmentType equipmentType;
        private int? intValue;
        private Priority priority;
        private int courseId;

        public int Id { get; set; }

        public AudienceEquipmentType EquipmentType
        {
            get { return equipmentType; }
            set
            {
                equipmentType = value;
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

        public static AudienceEquipmentViewModel FromModel(AudienceEquipment equipment, CourseViewModel parent)
        {
            return new AudienceEquipmentViewModel
            {
                Id = equipment.Id,
                EquipmentType = equipment.EquipmentType,
                IntValue = equipment.IntValue,
                Priority = equipment.Priority,
                CourseId = equipment.CourseId,
                ParentCourse = parent
            };
        }

        public AudienceEquipment ToModel()
        {
            return new AudienceEquipment
            {
                Id = this.Id,
                EquipmentType = this.EquipmentType,
                IntValue = this.IntValue,
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