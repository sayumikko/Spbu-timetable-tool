using Microsoft.EntityFrameworkCore;

namespace TeacherPreferencesDataModel
{
    public class ApplicationContext : DbContext
    {
        public DbSet<Teacher> Teachers { get; set; }
        public DbSet<GeneralPreference> GeneralPreferences { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<SpecificPreference> SpecificPreferences { get; set; }
        public DbSet<Audience> Audiences { get; set; }
        public DbSet<AudienceEquipment> AudienceEquipments { get; set; }
        public DbSet<TimeSlot> TimeSlots { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=teacherpreferences.db");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Teacher>()
                .HasMany(t => t.GeneralPreferences)
                .WithOne(gp => gp.Teacher)
                .HasForeignKey(gp => gp.TeacherId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Teacher>()
                .HasMany(t => t.Courses)
                .WithOne(c => c.Teacher)
                .HasForeignKey(c => c.TeacherId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Course>()
                .HasMany(c => c.SpecificPreferences)
                .WithOne(sp => sp.Course)
                .HasForeignKey(sp => sp.CourseId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Course>()
                .HasMany(c => c.Audiences)
                .WithOne(a => a.Course)
                .HasForeignKey(a => a.CourseId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Course>()
                .HasMany(c => c.AudienceEquipments)
                .WithOne(ae => ae.Course)
                .HasForeignKey(ae => ae.CourseId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Teacher>()
                .HasMany(t => t.TimeSlots)
                .WithOne(ts => ts.Teacher)
                .HasForeignKey(ts => ts.TeacherId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
