using System.ComponentModel.DataAnnotations;

namespace CoursesHelperWebAPI.Models.App
{
    public class Certification
    {
        public int Id { get; set; }

        [MaxLength(200)]
        public required string Name { get; set; }

        [MaxLength(3000)]
        public string? Description { get; set; }
        public virtual ICollection<CertificationCourse> CertificationCourses { get; set; } = [];
        public virtual ICollection<TraineeCertification> TraineeCertifications { get; set; } = [];
    }
}
