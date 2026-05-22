namespace CoursesHelperMVC.Models;

public class ReportsDashboardViewModel
{
    public int TotalCourses { get; set; }
    public int TotalSubjects { get; set; }
    public int TotalSessions { get; set; }
    public int TotalEnrollments { get; set; }
    public decimal TotalRevenueCollected { get; set; }
    public decimal OutstandingRevenue { get; set; }
    public IReadOnlyList<EnrollmentsByCourseReportItem> EnrollmentsByCourse { get; set; } = [];
    public IReadOnlyList<SessionsBySubjectReportItem> SessionsBySubject { get; set; } = [];
    public IReadOnlyList<InstructorWorkloadReportItem> InstructorWorkload { get; set; } = [];
    public IReadOnlyList<RevenueByCourseReportItem> RevenueByCourse { get; set; } = [];
}

public class EnrollmentsByCourseReportItem
{
    public string CourseName { get; set; } = string.Empty;
    public int EnrollmentCount { get; set; }
}

public class SessionsBySubjectReportItem
{
    public string SubjectName { get; set; } = string.Empty;
    public int SessionCount { get; set; }
}

public class InstructorWorkloadReportItem
{
    public string InstructorName { get; set; } = string.Empty;
    public int SessionCount { get; set; }
    public int TotalSeats { get; set; }
}

public class RevenueByCourseReportItem
{
    public string CourseName { get; set; } = string.Empty;
    public int EnrollmentCount { get; set; }
    public decimal RevenueCollected { get; set; }
    public decimal OutstandingRevenue { get; set; }
}
