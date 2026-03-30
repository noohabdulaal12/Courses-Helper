using CoursesHelperWebAPI.Models.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoursesHelperWebAPI.Models.App
{
    public class CertificationCourse
    {
        public required int CertificationId { get; set; }
        public required int CourseId { get; set; }
        public required virtual Certification Certification { get; set; } 
        public required virtual Course Course { get; set; } 
    }
}
