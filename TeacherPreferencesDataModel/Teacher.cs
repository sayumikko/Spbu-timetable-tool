namespace TeacherPreferencesDataModel
{
    public class Teacher
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string? Patronymic { get; set; }
        public Department Department { get; set; }

        public ICollection<GeneralPreference>? GeneralPreferences { get; set; }
        public ICollection<Course>? Courses { get; set; }
        public ICollection<TimeSlot>? TimeSlots { get; set; } 
    }

    public class TimeSlot
    {
        public int Id { get; set; }
        public DayOfWeek? DayOfWeek { get; set; }
        public TimeSpan? StartTime { get; set; }
        public TimeSpan? EndTime { get; set; }
        public Priority Priority { get; set; }

        public int TeacherId { get; set; } 
        public Teacher Teacher { get; set; }
    }
}
