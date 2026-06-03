using CoursesHelperWebAPI.Models.App;
using CoursesHelperWebAPI.Models.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace CoursesHelperWebAPI.Data
{
    public class AppDbContext : IdentityDbContext<User>
    {
        // Added this to allow fixed and simple seeding passwords to be used without running into a warning due to the changing salt
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.ConfigureWarnings(w => w.Ignore(RelationalEventId.PendingModelChangesWarning));
        }

        // Allows program.cs to inject the connection string
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Certification> Certifications { get; set; }
        public DbSet<CertificationCourse> CertificationCourses { get; set; }
        public DbSet<Classroom> Classrooms { get; set; }
        public DbSet<ClassroomType> ClassroomsTypes { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<CoursePrerequisite> CoursePrerequisites { get; set; }
        public DbSet<CourseSession> CourseSessions { get; set; }
        public DbSet<InstructorAvailability> InstructorAvailabilities { get; set; }
        public DbSet<InstructorQualification> instructorQualifications { get; set; }
        public DbSet<Subject> Subjects { get; set; }
        public DbSet<TakenTogether> TakenTogethers { get; set; }
        public DbSet<TakenTogetherCourse> TakenTogetherCourses { get; set; }
        public DbSet<TraineeCertification> TraineeCertifications { get; set; }
        public DbSet<TraineeQualification> TraineeQualifications { get; set; }
        public DbSet<TraineeSession> TraineeSessions { get; set; }
        public DbSet<UserInfo> UserInfos { get; set; }
        public DbSet<Notification> Notifications { get; set; }
            
        // FLuent API allows customizing the relationship between classes and tables
        // For example we need to set up composite keys here
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<CoursePrerequisite>(entity =>
            {

                entity.HasKey(cp => new { cp.CourseId, cp.PrerequisiteId });

                entity.HasOne(cp => cp.Course).WithMany(c => c.CoursePrerequisites).HasForeignKey(cp => cp.CourseId).OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(cp => cp.Prerequisite).WithMany(c => c.IsPrerequisiteFor).HasForeignKey(cp => cp.PrerequisiteId).OnDelete(DeleteBehavior.Restrict);

            });

            modelBuilder.Entity<CertificationCourse>(entity =>
            {
                entity.HasKey(cc => new { cc.CertificationId, cc.CourseId });

                entity.HasOne(cc => cc.Certification).WithMany(c => c.CertificationCourses).HasForeignKey(cc => cc.CertificationId).OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(cc => cc.Course).WithMany(c => c.CertificationCourses).HasForeignKey(cc => cc.CourseId).OnDelete(DeleteBehavior.Restrict);

            });

            modelBuilder.Entity<InstructorQualification>(entity =>
            {
                entity.HasKey(iq => new { iq.InstructorId, iq.CourseId });

                entity.HasOne(iq => iq.Course).WithMany(c => c.InstructorQualifications).HasForeignKey(iq => iq.CourseId).OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(iq => iq.Instructor).WithMany(ui => ui.InstructorQualifications).HasForeignKey(iq => iq.InstructorId).OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<InstructorAvailability>(entity =>
            {
                entity.HasKey(ia => ia.Id);

                entity.Property(ia => ia.DayOfWeek).HasConversion<int>();
                entity.Property(ia => ia.StartingTime).HasColumnType("Time");
                entity.Property(ia => ia.EndingTime).HasColumnType("Time");

                entity.HasIndex(ia => new { ia.InstructorId, ia.DayOfWeek, ia.StartingTime, ia.EndingTime });

                entity.HasOne(ia => ia.Instructor)
                    .WithMany(u => u.InstructorAvailabilities)
                    .HasForeignKey(ia => ia.InstructorId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<TakenTogetherCourse>(entity =>
            {
                entity.HasKey(ttc => new { ttc.TakenTogetherId, ttc.CourseId });

                entity.HasOne(ttc => ttc.TakenTogether).WithMany(tt => tt.TakenTogetherCourses).HasForeignKey(ttc => ttc.TakenTogetherId).OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(ttc => ttc.Course).WithMany(c => c.TakenTogetherCourses).HasForeignKey(ttc => ttc.CourseId).OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<TraineeCertification>(entity =>
            {
                entity.HasKey(tc => new { tc.TraineeId, tc.CertificationId });

                entity.HasOne(tc => tc.Trainee).WithMany(ui => ui.TraineeCertifications).HasForeignKey(tc => tc.TraineeId).OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(tc => tc.Certification).WithMany(c => c.TraineeCertifications).HasForeignKey(tc => tc.CertificationId).OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<TraineeQualification>(entity =>
            {
                entity.HasKey(tq => new { tq.TraineeId, tq.CourseId });

                entity.HasOne(tq => tq.Trainee).WithMany(ui => ui.TraineeQualifications).HasForeignKey(tq => tq.TraineeId).OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(tq => tq.Course).WithMany(c => c.TraineeQualifications).HasForeignKey(tq => tq.CourseId).OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<CourseSession>(entity =>
            {
                entity.HasKey(cs => cs.Id);

                entity.HasOne(cs => cs.Instructor).WithMany(ui => ui.CourseSessions).HasForeignKey(cs => cs.InstructorId).OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(cs => cs.Course).WithMany(c => c.CourseSessions).HasForeignKey(cs => cs.CourseId).OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(cs => cs.Classroom).WithMany(cl => cl.CourseSessions).HasForeignKey(cs => cs.ClassroomId).OnDelete(DeleteBehavior.Restrict);

                entity.Property(cs => cs.StartingDate).HasColumnType("date");
                entity.Property(cs => cs.StartingTime).HasColumnType("Time");
                entity.Property(cs => cs.EndingTime).HasColumnType("Time");
                entity.Property(cs => cs.DayOfWeek).HasConversion<int>();
            });

            modelBuilder.Entity<TraineeSession>(entity =>
            {
                entity.HasKey(ts => new { ts.TraineeId, ts.SessionId });

                entity.HasOne(ts => ts.Trainee).WithMany(ui => ui.TraineeSessions).HasForeignKey(ts => ts.TraineeId).OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(ts => ts.CourseSession).WithMany(cs => cs.TraineeSessions).HasForeignKey(ts => ts.SessionId).OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<UserInfo>(entity =>
            {
                entity.HasKey(ui => ui.UserId);

                entity.HasOne(ui => ui.User).WithOne(u => u.UserInfo).HasForeignKey<UserInfo>(ui => ui.UserId).OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Notification>(entity =>
            {
                entity.HasKey(n => n.Id);

                entity.Property(n => n.Title).HasMaxLength(200).IsRequired();
                entity.Property(n => n.Message).HasMaxLength(1000).IsRequired();
                entity.Property(n => n.Link).HasMaxLength(500);
                entity.Property(n => n.CreatedAtUtc).HasDefaultValueSql("GETUTCDATE()");

                entity.HasIndex(n => new { n.UserId, n.ReadAtUtc, n.CreatedAtUtc });

                entity.HasOne(n => n.User)
                    .WithMany(u => u.Notifications)
                    .HasForeignKey(n => n.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Classroom>(entity =>
            {
                entity.HasKey(c => c.Id);

                entity.HasOne(c => c.ClassroomType).WithMany(ct => ct.Classrooms).HasForeignKey(c => c.TypeId).OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Certification>(entity =>
            {
                entity.HasKey(c => c.Id);

                entity.HasMany(c => c.CertificationCourses).WithOne(cc => cc.Certification).HasForeignKey(cc => cc.CertificationId).OnDelete(DeleteBehavior.Cascade); 

                entity.HasMany(c => c.TraineeCertifications).WithOne(tc => tc.Certification).HasForeignKey(tc => tc.CertificationId).OnDelete(DeleteBehavior.Restrict); 
            });

            modelBuilder.Entity<Course>(entity =>
            {
                entity.HasKey(c => c.Id);

                entity.Property(c => c.Price).HasPrecision(18, 2);

                entity.HasOne(c => c.Subject).WithMany(s => s.Courses).HasForeignKey(c => c.SubjectId).OnDelete(DeleteBehavior.Restrict);

                entity.HasMany(c => c.CoursePrerequisites).WithOne(cp => cp.Course).HasForeignKey(cp => cp.CourseId).OnDelete(DeleteBehavior.Cascade);

                entity.HasMany(c => c.CourseSessions).WithOne(cs => cs.Course).HasForeignKey(cs => cs.CourseId).OnDelete(DeleteBehavior.Restrict);

                entity.HasMany(c => c.InstructorQualifications).WithOne(iq => iq.Course).HasForeignKey(iq => iq.CourseId).OnDelete(DeleteBehavior.Cascade);

                entity.HasMany(c => c.TraineeQualifications).WithOne(tq => tq.Course).HasForeignKey(tq => tq.CourseId).OnDelete(DeleteBehavior.Cascade);

                entity.HasMany(c => c.CertificationCourses).WithOne(cc => cc.Course).HasForeignKey(cc => cc.CourseId).OnDelete(DeleteBehavior.Cascade);

                entity.HasMany(c => c.TakenTogetherCourses).WithOne(ttc => ttc.Course).HasForeignKey(ttc => ttc.CourseId).OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Subject>(entity =>
            {
                entity.HasKey(s => s.Id);

                entity.HasMany(s => s.Courses).WithOne(c => c.Subject).HasForeignKey(c => c.SubjectId).OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<TakenTogether>(entity =>
            {
                entity.HasKey(tt => tt.Id);

                entity.HasMany(tt => tt.TakenTogetherCourses).WithOne(ttc => ttc.TakenTogether).HasForeignKey(ttc => ttc.TakenTogetherId).OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<ExtraEmail>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.HasIndex(e => new { e.UserId, e.Email }).IsUnique();

                entity.HasOne(e => e.User).WithMany(u => u.ExtraEmails).HasForeignKey(e => e.UserId).OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<ExtraPhone>(entity =>
            {
                entity.HasKey(p => p.Id);

                entity.HasIndex(p => new { p.UserId, p.Phone }).IsUnique();

                entity.HasOne(p => p.User).WithMany(u => u.ExtraPhones).HasForeignKey(p => p.UserId).OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.Property(u => u.UserType).IsRequired();

                entity.HasMany(u => u.ExtraEmails).WithOne(e => e.User).HasForeignKey(e => e.UserId).OnDelete(DeleteBehavior.Cascade);
 
                entity.HasMany(u => u.ExtraPhones).WithOne(p => p.User).HasForeignKey(p => p.UserId).OnDelete(DeleteBehavior.Cascade);

                entity.HasMany(u => u.InstructorAvailabilities).WithOne(ia => ia.Instructor).HasForeignKey(ia => ia.InstructorId).OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(u => u.UserInfo) .WithOne(ui => ui.User).HasForeignKey<UserInfo>(ui => ui.UserId).OnDelete(DeleteBehavior.Cascade);

                entity.HasMany(u => u.InstructorQualifications).WithOne(iq => iq.Instructor).HasForeignKey(iq => iq.InstructorId).OnDelete(DeleteBehavior.Cascade);

                entity.HasMany(u => u.CourseSessions).WithOne(cs => cs.Instructor).HasForeignKey(cs => cs.InstructorId).OnDelete(DeleteBehavior.Restrict);

                entity.HasMany(u => u.TraineeQualifications).WithOne(tq => tq.Trainee).HasForeignKey(tq => tq.TraineeId).OnDelete(DeleteBehavior.Cascade);

                entity.HasMany(u => u.TraineeCertifications).WithOne(tc => tc.Trainee).HasForeignKey(tc => tc.TraineeId).OnDelete(DeleteBehavior.Cascade);

                entity.HasMany(u => u.TraineeSessions).WithOne(ts => ts.Trainee).HasForeignKey(ts => ts.TraineeId).OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Seed();
        }
    }
}
