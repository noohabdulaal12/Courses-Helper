namespace CoursesHelperWebAPI.DTOs
{
    public class CreateEnrollmentDto
    {
        public string TraineeId { get; set; } = string.Empty;
        public int SessionId { get; set; }
        public decimal AmountPaid { get; set; }
        public DateOnly? PaymentDueDate { get; set; }
    }
}
