namespace CoursesHelperMVC.Models;

public enum PaymentStatus
{
    Unpaid,
    Partial,
    Paid,
    Overdue
}

public class PaymentSummaryViewModel
{
    public string TraineeId { get; set; } = string.Empty;
    public string TraineeName { get; set; } = string.Empty;
    public int SessionId { get; set; }
    public string SessionDetails { get; set; } = string.Empty;
    public decimal CoursePrice { get; set; }
    public decimal AmountPaid { get; set; }
    public decimal RemainingBalance => Math.Max(0, CoursePrice - AmountPaid);
    public DateOnly? PaymentDueDate { get; set; }
    public PaymentStatus PaymentStatus => PaymentStatusHelper.Calculate(CoursePrice, AmountPaid, PaymentDueDate);
}

public class PaymentsIndexViewModel
{
    public string Filter { get; set; } = "All";
    public IReadOnlyList<PaymentSummaryViewModel> Payments { get; set; } = [];
}

public static class PaymentStatusHelper
{
    public static PaymentStatus Calculate(decimal coursePrice, decimal amountPaid, DateOnly? paymentDueDate)
    {
        var balance = Math.Max(0, coursePrice - amountPaid);
        if (balance <= 0)
        {
            return PaymentStatus.Paid;
        }

        if (paymentDueDate.HasValue && paymentDueDate.Value < DateOnly.FromDateTime(DateTime.Today))
        {
            return PaymentStatus.Overdue;
        }

        return amountPaid <= 0 ? PaymentStatus.Unpaid : PaymentStatus.Partial;
    }

    public static string Format(PaymentStatus status)
    {
        return status switch
        {
            PaymentStatus.Unpaid => "Unpaid",
            PaymentStatus.Partial => "Partial",
            PaymentStatus.Paid => "Paid",
            PaymentStatus.Overdue => "Overdue",
            _ => status.ToString()
        };
    }
}
