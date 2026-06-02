namespace CoursesHelperReports.Models;

public class ReportsDashboardViewModel
{
    public IReadOnlyList<EnrollmentByCourseReportItem> EnrollmentsByCourse { get; set; } = [];
    public IReadOnlyList<EnrollmentBySubjectReportItem> EnrollmentsBySubject { get; set; } = [];
    public IReadOnlyList<InstructorWorkloadReportItem> InstructorWorkload { get; set; } = [];
    public IReadOnlyList<RevenueSummaryReportItem> RevenueSummary { get; set; } = [];
    public IReadOnlyList<CertificationCompletionReportItem> CertificationCompletion { get; set; } = [];

    public int TotalEnrollments => EnrollmentsByCourse.Sum(x => x.TotalEnrollments);
    public decimal CollectedRevenue => RevenueSummary.Sum(x => x.CollectedRevenue);
    public decimal OutstandingRevenue => RevenueSummary.Sum(x => x.OutstandingRevenue);
}

public class EnrollmentByCourseReportItem
{
    public int CourseId { get; set; }
    public string CourseName { get; set; } = string.Empty;
    public int TotalEnrollments { get; set; }
    public int Completed { get; set; }
    public int Requested { get; set; }
}

public class EnrollmentBySubjectReportItem
{
    public int SubjectId { get; set; }
    public string SubjectName { get; set; } = string.Empty;
    public int TotalEnrollments { get; set; }
}

public class InstructorWorkloadReportItem
{
    public string InstructorId { get; set; } = string.Empty;
    public string InstructorName { get; set; } = string.Empty;
    public int TotalSessions { get; set; }
    public int TotalAssignedSeats { get; set; }
    public int TotalEnrollments { get; set; }
}

public class RevenueSummaryReportItem
{
    public int CourseId { get; set; }
    public string CourseName { get; set; } = string.Empty;
    public int EnrollmentCount { get; set; }
    public decimal ExpectedRevenue { get; set; }
    public decimal CollectedRevenue { get; set; }
    public decimal OutstandingRevenue { get; set; }
}

public class CertificationCompletionReportItem
{
    public int CertificationId { get; set; }
    public string CertificationName { get; set; } = string.Empty;
    public int TotalTrackedTrainees { get; set; }
    public int EligibleTrainees { get; set; }
    public double CompletionRate { get; set; }
}
