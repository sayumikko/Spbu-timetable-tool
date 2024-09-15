using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeacherPreferencesDataModel
{
    public class GeneralPreference
    {
        public int Id { get; set; }
        public int TeacherId { get; set; }
        public Teacher Teacher { get; set; }

        public GeneralPreferenceType PreferenceType { get; set; }
        public int? IntValue { get; set; }
        public int? TeacherRefId { get; set; }
        public Priority Priority { get; set; }
    }
}
