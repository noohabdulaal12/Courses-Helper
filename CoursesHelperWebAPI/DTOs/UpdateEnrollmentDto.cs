using CoursesHelperWebAPI.Models.Enums;

namespace CoursesHelperWebAPI.DTOs
{
    public class UpdateEnrollmentDto
    {
        public string TraineeId { get; set; } = string.Empty;
        public int SessionId { get; set; }
        public decimal AmountPaid { get; set; }
        public DateOnly? PaymentDueDate { get; set; }
        public Status Status { get; set; }
    }
}
