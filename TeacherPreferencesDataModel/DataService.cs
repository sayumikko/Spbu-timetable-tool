using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace TeacherPreferencesDataModel
{
    public class DataService
    {
        private readonly ApplicationContext _context;

        public DataService(ApplicationContext context)
        {
            _context = context;
        }

        public List<Teacher> GetTeachers()
        {
            return _context.Teachers
                .Include(t => t.GeneralPreferences)
                .Include(gp => gp.TimeSlots!)
                .Include(t => t.Courses)
                    .ThenInclude(c => c.SpecificPreferences!)
                .Include(t => t.Courses)
                    .ThenInclude(c => c.Audiences!)
                .Include(t => t.Courses)
                    .ThenInclude(c => c.AudienceEquipments!)
                .ToList();
        }
    }
}
