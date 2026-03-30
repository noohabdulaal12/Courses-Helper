using CoursesHelperWebAPI.Models.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoursesHelperWebAPI.Models.App
{
    public class TraineeCertification
    {
        public required string TraineeId { get; set; }
        public required int CertificationId { get; set; }
        public virtual User Trainee { get; set; } = null!;
        public virtual Certification Certification { get; set; } = null!;
    }
}
