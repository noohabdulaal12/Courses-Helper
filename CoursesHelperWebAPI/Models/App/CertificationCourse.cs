using CoursesHelperWebAPI.Models.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoursesHelperWebAPI.Models.App
{
    public class CertificationCourse
    {
        public required int CertificationId { get; set; }
        public required int CourseId { get; set; }
        public virtual Certification? Certification { get; set; } 
        public virtual Course? Course { get; set; } 
    }
}
