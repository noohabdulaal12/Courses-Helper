using CoursesHelperWebAPI.Models.App;
using CoursesHelperWebAPI.Models.Enums;
using CoursesHelperWebAPI.Models.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CoursesHelperWebAPI.Data
{
    public static class SeedData
    {
        // Deterministic hash for TestingPassword! so migrations do not rewrite seed users on every scaffold.
        private const string _staticPasswordHash = "AQAAAAIAAYagAAAAEFHS0Dcj5cV5gNNRjaUV+EZ2VKgFC6dRyt21ae+SVV+PVRBOo7GF7uI5wtH7IwltIw==";

        public static void Seed(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Certification>().HasData(
                new { Id = 1, Name = "IT Cert" , Description = "A certification in Information Technology." },
                new { Id = 2, Name = "Programming Cert", Description = "A certification in programming." }
            );

            modelBuilder.Entity<Subject>().HasData(
                new { Id = 1, Name = "IT", Description = "Information Technology: A subject focusing on hardware configuration and repair." },
                new { Id = 2, Name = "Programming", Description = "Programming: A subject focusing on software development." },
                new { Id = 3, Name = "Business", Description = "Business: A subject focusing on managing companies and finances." },
                new { Id = 4, Name = "Mechanical Engineering", Description = "Mechanical Engineering: A subject focusing on moving hardware systems" }
            );

            modelBuilder.Entity<Course>().HasData(
                new { Id = 1, Name = "IT 1", Description = "Introduction to Information Technology (IT).", SubjectId = 1, NumberOfSessions = 10, Price = 100m },
                new { Id = 2, Name = "Programming 1", Description = "Introduction to Programming.", SubjectId = 2, NumberOfSessions = 10, Price = 100m },
                new { Id = 3, Name = "Business 1", Description = "Introduction to Business.", SubjectId = 3, NumberOfSessions = 12, Price = 120m },
                new { Id = 4, Name = "Mechanical Engineering 1", Description = "Introduction to Mechanical Engineering.", SubjectId = 4, NumberOfSessions = 15, Price = 150m },
                new { Id = 5, Name = "IT 2", Description = "Continuation to Information Technology (IT).", SubjectId = 1, NumberOfSessions = 10, Price = 100m },
                new { Id = 6, Name = "IT 3", Description = "Continuation to Information Technology (IT).", SubjectId = 1, NumberOfSessions = 10, Price = 100m }
            );

            modelBuilder.Entity<CertificationCourse>().HasData(
                new { CertificationId = 1, CourseId = 1 },
                new { CertificationId = 2, CourseId = 2 },
                new { CertificationId = 1, CourseId = 5},
                new { CertificationId = 1, CourseId = 6 }
            );

            modelBuilder.Entity<ClassroomType>().HasData(
                new { Id = 1, Name = "Computer lab", Capacity = 20, Description = "A room with 20+ computers and a smart board." },
                new { Id = 2, Name = "Engineering room", Capacity = 20, Description = "A room wtih engineering equipment for 20+ students." }
            );

            modelBuilder.Entity<Classroom>().HasData(
                new { Id = 1, TypeId = 1, Description = "Computer lab 19.101" },
                new { Id = 2, TypeId = 1, Description = "Computer lab 19.102" },
                new { Id = 3, TypeId = 1, Description = "Computer lab 19.103" },
                new { Id = 4, TypeId = 2, Description = "Engineering room 2.1" }
            );

            modelBuilder.Entity<CoursePrerequisite>().HasData(
                new { CourseId = 5, PrerequisiteId = 1 },
                new { CourseId = 6, PrerequisiteId = 1 }
            );

            modelBuilder.Entity<IdentityRole>().HasData(
                new IdentityRole { Id = "trainee-role", Name = "Trainee", NormalizedName = "TRAINEE" },
                new IdentityRole { Id = "instructor-role", Name = "Instructor", NormalizedName = "INSTRUCTOR" },
                new IdentityRole { Id = "coordinator-role", Name = "Coordinator", NormalizedName = "COORDINATOR" },
                new IdentityRole { Id = "admin-role", Name = "Admin", NormalizedName = "ADMIN" }
            );

            modelBuilder.Entity<User>().HasData(
                new
                {
                    Id = "admin-user",
                    UserName = "admin@uni.edu",
                    NormalizedUserName = "ADMIN@UNI.EDU",
                    Email = "admin@uni.edu",
                    NormalizedEmail = "ADMIN@UNI.EDU",
                    EmailConfirmed = true,
                    UserType = UserType.Admin,
                    SecurityStamp = "STAMP_ADMIN",
                    PasswordHash = _staticPasswordHash,
                    ConcurrencyStamp = "ADMIN_STAMP",
                    AccessFailedCount = 0,
                    TwoFactorEnabled = false,
                    PhoneNumberConfirmed = false,
                    LockoutEnabled = true
                },
                new
                {
                    Id = "coordinator-user",
                    UserName = "coord@uni.edu",
                    NormalizedUserName = "COORD@UNI.EDU",
                    Email = "coord@uni.edu",
                    NormalizedEmail = "COORD@UNI.EDU",
                    EmailConfirmed = true,
                    UserType = UserType.Coordinator,
                    SecurityStamp = "STAMP_COORD",
                    PasswordHash = _staticPasswordHash,
                    ConcurrencyStamp = "COORDINATOR_STAMP",
                    AccessFailedCount = 0,
                    TwoFactorEnabled = false,
                    PhoneNumberConfirmed = false,
                    LockoutEnabled = true
                },
                new
                {
                    Id = "instructor-user",
                    UserName = "teacher@uni.edu",
                    NormalizedUserName = "TEACHER@UNI.EDU",
                    Email = "teacher@uni.edu",
                    NormalizedEmail = "TEACHER@UNI.EDU",
                    EmailConfirmed = true,
                    UserType = UserType.Instructor,
                    SecurityStamp = "STAMP_INST",
                    PasswordHash = _staticPasswordHash,
                    ConcurrencyStamp = "INSTRUCTOR_STAMP",
                    AccessFailedCount = 0,
                    TwoFactorEnabled = false,
                    PhoneNumberConfirmed = false,
                    LockoutEnabled = true
                },
                new
                {
                    Id = "instructor-user2",
                    UserName = "fatima.alharbi@uni.edu",
                    NormalizedUserName = "FATIMA.ALHARBI@UNI.EDU",
                    Email = "fatima.alharbi@uni.edu",
                    NormalizedEmail = "FATIMA.ALHARBI@UNI.EDU",
                    EmailConfirmed = true,
                    UserType = UserType.Instructor,
                    SecurityStamp = "STAMP_INST2",
                    PasswordHash = _staticPasswordHash,
                    ConcurrencyStamp = "INSTRUCTOR_STAMP2",
                    AccessFailedCount = 0,
                    TwoFactorEnabled = false,
                    PhoneNumberConfirmed = false,
                    LockoutEnabled = true
                },
                new
                {
                    Id = "instructor-user3",
                    UserName = "khalid.almansour@uni.edu",
                    NormalizedUserName = "KHALID.ALMANSOUR@UNI.EDU",
                    Email = "khalid.almansour@uni.edu",
                    NormalizedEmail = "KHALID.ALMANSOUR@UNI.EDU",
                    EmailConfirmed = true,
                    UserType = UserType.Instructor,
                    SecurityStamp = "STAMP_INST3",
                    PasswordHash = _staticPasswordHash,
                    ConcurrencyStamp = "INSTRUCTOR_STAMP3",
                    AccessFailedCount = 0,
                    TwoFactorEnabled = false,
                    PhoneNumberConfirmed = false,
                    LockoutEnabled = true
                },
                new
                {
                    Id = "trainee-user",
                    UserName = "student@uni.edu",
                    NormalizedUserName = "STUDENT@UNI.EDU",
                    Email = "student@uni.edu",
                    NormalizedEmail = "STUDENT@UNI.EDU",
                    EmailConfirmed = true,
                    UserType = UserType.Trainee,
                    SecurityStamp = "STAMP_TRAINEE",
                    PasswordHash = _staticPasswordHash,
                    ConcurrencyStamp = "TRAINEE_STAMP",
                    AccessFailedCount = 0,
                    TwoFactorEnabled = false,
                    PhoneNumberConfirmed = false,
                    LockoutEnabled = true
                },
                new
                {
                    Id = "trainee-user2",
                    UserName = "student2@uni.edu",
                    NormalizedUserName = "STUDENT2@UNI.EDU",
                    Email = "student2@uni.edu",
                    NormalizedEmail = "STUDENT2@UNI.EDU",
                    EmailConfirmed = true,
                    UserType = UserType.Trainee,
                    SecurityStamp = "STAMP_TRAINEE2",
                    PasswordHash = _staticPasswordHash,
                    ConcurrencyStamp = "TRAINEE_STAMP2",
                    AccessFailedCount = 0,
                    TwoFactorEnabled = false,
                    PhoneNumberConfirmed = false,
                    LockoutEnabled = true
                },
                new
                {
                    Id = "trainee-user3",
                    UserName = "aisha.alqahtani@uni.edu",
                    NormalizedUserName = "AISHA.ALQAHTANI@UNI.EDU",
                    Email = "aisha.alqahtani@uni.edu",
                    NormalizedEmail = "AISHA.ALQAHTANI@UNI.EDU",
                    EmailConfirmed = true,
                    UserType = UserType.Trainee,
                    SecurityStamp = "STAMP_TRAINEE3",
                    PasswordHash = _staticPasswordHash,
                    ConcurrencyStamp = "TRAINEE_STAMP3",
                    AccessFailedCount = 0,
                    TwoFactorEnabled = false,
                    PhoneNumberConfirmed = false,
                    LockoutEnabled = true
                },
                new
                {
                    Id = "trainee-user4",
                    UserName = "mohammed.alfarsi@uni.edu",
                    NormalizedUserName = "MOHAMMED.ALFARSI@UNI.EDU",
                    Email = "mohammed.alfarsi@uni.edu",
                    NormalizedEmail = "MOHAMMED.ALFARSI@UNI.EDU",
                    EmailConfirmed = true,
                    UserType = UserType.Trainee,
                    SecurityStamp = "STAMP_TRAINEE4",
                    PasswordHash = _staticPasswordHash,
                    ConcurrencyStamp = "TRAINEE_STAMP4",
                    AccessFailedCount = 0,
                    TwoFactorEnabled = false,
                    PhoneNumberConfirmed = false,
                    LockoutEnabled = true
                },
                new
                {
                    Id = "trainee-user5",
                    UserName = "layla.alnajjar@uni.edu",
                    NormalizedUserName = "LAYLA.ALNAJJAR@UNI.EDU",
                    Email = "layla.alnajjar@uni.edu",
                    NormalizedEmail = "LAYLA.ALNAJJAR@UNI.EDU",
                    EmailConfirmed = true,
                    UserType = UserType.Trainee,
                    SecurityStamp = "STAMP_TRAINEE5",
                    PasswordHash = _staticPasswordHash,
                    ConcurrencyStamp = "TRAINEE_STAMP5",
                    AccessFailedCount = 0,
                    TwoFactorEnabled = false,
                    PhoneNumberConfirmed = false,
                    LockoutEnabled = true
                },
                new
                {
                    Id = "trainee-user6",
                    UserName = "youssef.alhassan@uni.edu",
                    NormalizedUserName = "YOUSSEF.ALHASSAN@UNI.EDU",
                    Email = "youssef.alhassan@uni.edu",
                    NormalizedEmail = "YOUSSEF.ALHASSAN@UNI.EDU",
                    EmailConfirmed = true,
                    UserType = UserType.Trainee,
                    SecurityStamp = "STAMP_TRAINEE6",
                    PasswordHash = _staticPasswordHash,
                    ConcurrencyStamp = "TRAINEE_STAMP6",
                    AccessFailedCount = 0,
                    TwoFactorEnabled = false,
                    PhoneNumberConfirmed = false,
                    LockoutEnabled = true
                },
                new
                {
                    Id = "trainee-user7",
                    UserName = "noura.alsaleh@uni.edu",
                    NormalizedUserName = "NOURA.ALSALEH@UNI.EDU",
                    Email = "noura.alsaleh@uni.edu",
                    NormalizedEmail = "NOURA.ALSALEH@UNI.EDU",
                    EmailConfirmed = true,
                    UserType = UserType.Trainee,
                    SecurityStamp = "STAMP_TRAINEE7",
                    PasswordHash = _staticPasswordHash,
                    ConcurrencyStamp = "TRAINEE_STAMP7",
                    AccessFailedCount = 0,
                    TwoFactorEnabled = false,
                    PhoneNumberConfirmed = false,
                    LockoutEnabled = true
                }
            );

            modelBuilder.Entity<IdentityUserRole<string>>().HasData(
                new { UserId = "admin-user", RoleId = "admin-role" },
                new { UserId = "coordinator-user", RoleId = "coordinator-role" },
                new { UserId = "instructor-user", RoleId = "instructor-role" },
                new { UserId = "instructor-user2", RoleId = "instructor-role" },
                new { UserId = "instructor-user3", RoleId = "instructor-role" },
                new { UserId = "trainee-user", RoleId = "trainee-role" },
                new { UserId = "trainee-user2", RoleId = "trainee-role" },
                new { UserId = "trainee-user3", RoleId = "trainee-role" },
                new { UserId = "trainee-user4", RoleId = "trainee-role" },
                new { UserId = "trainee-user5", RoleId = "trainee-role" },
                new { UserId = "trainee-user6", RoleId = "trainee-role" },
                new { UserId = "trainee-user7", RoleId = "trainee-role" }
            );

            modelBuilder.Entity<UserInfo>().HasData(
                new { UserId = "admin-user", FirstName = "Example", LastName = "Admin", DateOfBirth = new DateOnly(1980, 1, 1) },
                new { UserId = "coordinator-user", FirstName = "Example", LastName = "Coordinator", DateOfBirth = new DateOnly(1985, 5, 15) },
                new { UserId = "instructor-user", FirstName = "Example", LastName = "Instructor", DateOfBirth = new DateOnly(1975, 10, 20) },
                new { UserId = "instructor-user2", FirstName = "Fatima", LastName = "Al-Harbi", DateOfBirth = new DateOnly(1981, 3, 12) },
                new { UserId = "instructor-user3", FirstName = "Khalid", LastName = "Al-Mansour", DateOfBirth = new DateOnly(1978, 7, 9) },
                new { UserId = "trainee-user", FirstName = "Example", LastName = "Trainee", DateOfBirth = new DateOnly(2000, 12, 5) },
                new { UserId = "trainee-user2", FirstName = "Example", LastName = "Trainee2", DateOfBirth = new DateOnly(2000, 12, 5) },
                new { UserId = "trainee-user3", FirstName = "Aisha", LastName = "Al-Qahtani", DateOfBirth = new DateOnly(2001, 2, 14) },
                new { UserId = "trainee-user4", FirstName = "Mohammed", LastName = "Al-Farsi", DateOfBirth = new DateOnly(1999, 8, 3) },
                new { UserId = "trainee-user5", FirstName = "Layla", LastName = "Al-Najjar", DateOfBirth = new DateOnly(2002, 4, 18) },
                new { UserId = "trainee-user6", FirstName = "Youssef", LastName = "Al-Hassan", DateOfBirth = new DateOnly(2000, 6, 21) },
                new { UserId = "trainee-user7", FirstName = "Noura", LastName = "Al-Saleh", DateOfBirth = new DateOnly(2001, 9, 7) }
            );

            modelBuilder.Entity<InstructorQualification>().HasData(
                new { InstructorId = "instructor-user", CourseId = 5 },
                new { InstructorId = "instructor-user", CourseId = 6 }
            );

            modelBuilder.Entity<CourseSession>().HasData(
                new { Id = 1, CourseId = 5, InstructorId = "instructor-user", ClassroomId = 1, MaxSeats = 20, StartingDate = new DateOnly(2026, 1, 1), StartingTime = new TimeSpan(14, 0, 0), EndingTime = new TimeSpan(16, 0, 0), DayOfWeek = DayOfWeek.Monday },
                new { Id = 2, CourseId = 5, InstructorId = "instructor-user", ClassroomId = 1, MaxSeats = 20, StartingDate = new DateOnly(2026, 1, 1), StartingTime = new TimeSpan(14, 0, 0), EndingTime = new TimeSpan(16, 0, 0), DayOfWeek = DayOfWeek.Monday }
            );

            modelBuilder.Entity<TakenTogether>().HasData(
                new { Id = 1, Name = "IT Program", Description = "Two IT courses that must be taken together after completing the introduction." }
            );

            modelBuilder.Entity<TakenTogetherCourse>().HasData(
                new { TakenTogetherId = 1, CourseId = 5 },
                new { TakenTogetherId = 1, CourseId = 6 }
            );

            modelBuilder.Entity<TraineeQualification>().HasData(
                new { TraineeId = "trainee-user", CourseId = 1 },
                new { TraineeId = "trainee-user2", CourseId = 2 }
            );

            modelBuilder.Entity<TraineeCertification>().HasData(
                new { TraineeId = "trainee-user2", CertificationId = 2 }
            );

            modelBuilder.Entity<TraineeSession>().HasData(
                new { TraineeId = "trainee-user", SessionId = 1, AmountPaid = 100m, Status = Status.Requested },
                new { TraineeId = "trainee-user", SessionId = 2, AmountPaid = 100m, Status = Status.Requested }
            );

            modelBuilder.Entity<ExtraEmail>().HasData(
                new { Id = 1, UserId = "trainee-user", Email = "stu1extra@uni.edu" },
                new { Id = 2, UserId = "trainee-user2", Email = "stu2extra@uni.edu" }
            );

            modelBuilder.Entity<ExtraPhone>().HasData(
                new { Id = 1, UserId = "trainee-user", Phone = "9310134141242141" },
                new { Id = 2, UserId = "trainee-user2", Phone = "901204124014021" }
            );
        }
    }
}
