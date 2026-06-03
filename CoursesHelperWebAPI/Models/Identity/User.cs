using CoursesHelperWebAPI.Models.App;
using CoursesHelperWebAPI.Models.Enums;
using Microsoft.AspNetCore.Identity;

namespace CoursesHelperWebAPI.Models.Identity
{
    public class User: IdentityUser
    {
        public required UserType UserType { get; set; }
        public virtual UserInfo? UserInfo { get; set; }
        public virtual ICollection<ExtraEmail> ExtraEmails { get; set; } = [];
        public virtual ICollection<ExtraPhone> ExtraPhones { get; set; } = [];
        public virtual ICollection<InstructorAvailability> InstructorAvailabilities { get; set; } = [];
        public virtual ICollection<InstructorQualification> InstructorQualifications { get; set; } = [];
        public virtual ICollection<TraineeCertification> TraineeCertifications { get; set; } = [];
        public virtual ICollection<TraineeQualification> TraineeQualifications { get; set; } = [];
        public virtual ICollection<CourseSession> CourseSessions { get; set; } = [];
        public virtual ICollection<TraineeSession> TraineeSessions { get; set; } = [];
        public virtual ICollection<Notification> Notifications { get; set; } = [];
    }
}
