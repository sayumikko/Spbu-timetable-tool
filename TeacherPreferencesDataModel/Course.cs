using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeacherPreferencesDataModel
{
    public class Course
    {
        public int Id { get; set; }
        public string Group { get; set; }
        public string SubjectName { get; set; }
        public ClassType ClassType { get; set; }

        public int TeacherId { get; set; }
        public Teacher Teacher { get; set; }

        public List<SpecificPreference> SpecificPreferences { get; set; } = new();

        public List<Audience> Audiences { get; set; } = new();
        public List<AudienceEquipment> AudienceEquipments { get; set; } = new();
        public List<TimeSlot> TimeSlots { get; set; } = new();
    }

    public class Audience
    {
        public int Id { get; set; }
        public int Number { get; set; }
        public Priority Priority { get; set; }

        public int CourseId { get; set; }
        public Course Course { get; set; } 
    }

    public class AudienceEquipment
    {
        public int Id { get; set; }
        public AudienceEquipmentType EquipmentType { get; set; }
        public int? IntValue { get; set; }
        public Priority Priority { get; set; }

        public int CourseId { get; set; } 
        public Course Course { get; set; } 
    }
}

