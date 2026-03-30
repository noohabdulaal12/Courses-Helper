using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoursesHelperWebAPI.Models.App
{
    public class Course
    {
        public int Id { get; set; }

        [MaxLength(200)]
        public required string Name { get; set; }
        
        [MaxLength(3000)]
        public string? Description { get; set; }
        public required int SubjectId { get; set; }
        public required int NumberOfSessions { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public required Decimal Price { get; set; }

        [ForeignKey("SubjectId")]
        public virtual Subject Subject { get; set; } = null!;
        public virtual ICollection<CoursePrerequisite> CoursePrerequisites { get; set; } = [];
        public virtual ICollection<CoursePrerequisite> IsPrerequisiteFor { get; set; } = [];
        public virtual ICollection<CertificationCourse> CertificationCourses { get; set; } = [];
        public virtual ICollection<InstructorQualification> InstructorQualifications { get; set; } = [];
        public virtual ICollection<TakenTogetherCourse> TakenTogetherCourses { get; set; } = [];
        public virtual ICollection<TraineeQualification> TraineeQualifications { get; set; } = [];
        public virtual ICollection<CourseSession> CourseSessions { get; set; } = [];
    }
}
