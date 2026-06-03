using System.ComponentModel.DataAnnotations;
using CoursesHelperWebAPI.Models.Enums;

namespace CoursesHelperMVC.Models;

public class EnrollmentFormViewModel
{
    [Display(Name = "Trainee")]
    [Required(ErrorMessage = "Please choose a trainee.")]
    public string TraineeId { get; set; } = string.Empty;

    [Display(Name = "Course session")]
    [Range(1, int.MaxValue, ErrorMessage = "Please choose a course session.")]
    public int SessionId { get; set; }

    [Display(Name = "Amount paid")]
    [Range(0, 999999999, ErrorMessage = "Amount paid cannot be negative.")]
    [DataType(DataType.Currency)]
    public decimal AmountPaid { get; set; }

    [Display(Name = "Payment due date")]
    [DataType(DataType.Date)]
    public DateOnly? PaymentDueDate { get; set; }

    public Status Status { get; set; } = Status.Requested;
}
