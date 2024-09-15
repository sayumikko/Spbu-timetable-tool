using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using TeacherPreferencesDataModel;

namespace TeacherPreferencesUI.ViewModels.SpecificPreferenceViewModels
{
    public class TimeSlotViewModel : INotifyPropertyChanged
    {
        private TeacherPreferencesDataModel.DayOfWeek? dayOfWeek;
        private TimeSpan? startTime;
        private TimeSpan? endTime;
        private Priority priority;
        private int teacherId;

        public int Id { get; set; }

        public TeacherPreferencesDataModel.DayOfWeek? DayOfWeek
        {
            get { return dayOfWeek; }
            set
            {
                dayOfWeek = value;
                OnPropertyChanged();
            }
        }

        public TimeSpan? StartTime
        {
            get { return startTime; }
            set
            {
                startTime = value;
                OnPropertyChanged();
            }
        }

        public TimeSpan? EndTime
        {
            get { return endTime; }
            set
            {
                endTime = value;
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

        public static TimeSlotViewModel FromModel(TimeSlot timeSlot)
        {
            return new TimeSlotViewModel
            {
                Id = timeSlot.Id,
                DayOfWeek = timeSlot.DayOfWeek,
                StartTime = timeSlot.StartTime,
                EndTime = timeSlot.EndTime,
                Priority = timeSlot.Priority,
                TeacherId = timeSlot.TeacherId,
            };
        }

        public TimeSlot ToModel()
        {
            return new TimeSlot
            {
                Id = Id,
                DayOfWeek = DayOfWeek,
                StartTime = StartTime,
                EndTime = EndTime,
                Priority = Priority,
                TeacherId = this.TeacherId,
            };
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }
    }
}
