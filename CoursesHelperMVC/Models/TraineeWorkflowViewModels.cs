using CoursesHelperWebAPI.Models.Enums;

namespace CoursesHelperMVC.Models;

public class TraineeDashboardViewModel
{
    public int ActiveEnrollmentCount { get; set; }
    public decimal OutstandingBalance { get; set; }
    public int EligibleCertificationCount { get; set; }
    public int TotalCertificationCount { get; set; }
    public IReadOnlyList<TraineeEnrollmentViewModel> ActiveEnrollments { get; set; } = [];
}

public class AvailableSessionViewModel
{
    public int SessionId { get; set; }
    public string CourseName { get; set; } = string.Empty;
    public string SubjectName { get; set; } = string.Empty;
    public string InstructorName { get; set; } = string.Empty;
    public string ClassroomName { get; set; } = string.Empty;
    public DateOnly StartingDate { get; set; }
    public TimeSpan StartingTime { get; set; }
    public TimeSpan EndingTime { get; set; }
    public int MaxSeats { get; set; }
    public int CurrentEnrollments { get; set; }
    public int RemainingSeats => Math.Max(0, MaxSeats - CurrentEnrollments);
    public decimal Price { get; set; }
    public bool IsAlreadyEnrolled { get; set; }
}

public class TraineeEnrollmentViewModel
{
    public int SessionId { get; set; }
    public string CourseName { get; set; } = string.Empty;
    public DateOnly StartingDate { get; set; }
    public TimeSpan StartingTime { get; set; }
    public TimeSpan EndingTime { get; set; }
    public Status Status { get; set; }
    public decimal CoursePrice { get; set; }
    public decimal AmountPaid { get; set; }
    public DateOnly? PaymentDueDate { get; set; }
    public decimal RemainingBalance => Math.Max(0, CoursePrice - AmountPaid);
    public PaymentStatus PaymentStatus => PaymentStatusHelper.Calculate(CoursePrice, AmountPaid, PaymentDueDate);
}

public class CertificationProgressViewModel
{
    public string CertificationName { get; set; } = string.Empty;
    public IReadOnlyList<string> RequiredCourses { get; set; } = [];
    public IReadOnlyList<string> CompletedCourses { get; set; } = [];
    public IReadOnlyList<string> PendingCourses { get; set; } = [];
    public bool IsEligible => PendingCourses.Count == 0 && RequiredCourses.Count > 0;
}
