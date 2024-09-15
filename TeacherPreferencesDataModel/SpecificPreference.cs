namespace TeacherPreferencesDataModel
{
    public class SpecificPreference
    {
        public int Id { get; set; }

        public int CourseId { get; set; }
        public Course Course { get; set; }

        public SpecificPreferenceType PreferenceType { get; set; }
        public Priority Priority { get; set; }
    }
}
