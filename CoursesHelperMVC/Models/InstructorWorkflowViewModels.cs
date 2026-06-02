using CoursesHelperWebAPI.Models.Enums;

namespace CoursesHelperMVC.Models;

public class InstructorDashboardViewModel
{
    public int AssignedSessionCount { get; set; }
    public IReadOnlyList<InstructorSessionViewModel> UpcomingSessions { get; set; } = [];
}

public class InstructorSessionViewModel
{
    public int SessionId { get; set; }
    public string CourseName { get; set; } = string.Empty;
    public string ClassroomName { get; set; } = string.Empty;
    public DateOnly StartingDate { get; set; }
    public TimeSpan StartingTime { get; set; }
    public TimeSpan EndingTime { get; set; }
    public int EnrolledCount { get; set; }
}

public class InstructorSessionTraineeViewModel
{
    public int SessionId { get; set; }
    public string CourseName { get; set; } = string.Empty;
    public IReadOnlyList<SessionTraineeViewModel> Trainees { get; set; } = [];
}

public class SessionTraineeViewModel
{
    public string TraineeId { get; set; } = string.Empty;
    public string TraineeName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public Status Status { get; set; }
    public decimal AmountPaid { get; set; }
}

public class RecordAssessmentViewModel
{
    public int SessionId { get; set; }
    public string TraineeId { get; set; } = string.Empty;
    public string TraineeName { get; set; } = string.Empty;
    public string CourseName { get; set; } = string.Empty;
    public Status Status { get; set; }
}
