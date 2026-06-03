using CoursesHelperWebAPI.Models.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CoursesHelperWebAPI.Models.Enums;

namespace CoursesHelperWebAPI.Models.App
{
    public class TraineeSession
    {
        public required string TraineeId { get; set; }
        public required int SessionId { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public required Decimal AmountPaid { get; set; }
        public DateOnly? PaymentDueDate { get; set; }
        public Status Status { get; set; } = Status.Requested;
        public virtual User Trainee { get; set; } = null!;
        public virtual CourseSession CourseSession { get; set; } = null!;
    }
}
