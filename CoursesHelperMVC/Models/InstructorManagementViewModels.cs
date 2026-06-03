using System.ComponentModel.DataAnnotations;

namespace CoursesHelperMVC.Models;

public class InstructorListItemViewModel
{
    public string InstructorId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public int QualifiedCourseCount { get; set; }
    public int AvailabilitySlotCount { get; set; }
    public int AssignedSessionCount { get; set; }
}

public class InstructorDetailsViewModel
{
    public string InstructorId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public DateOnly? DateOfBirth { get; set; }
    public IReadOnlyList<string> QualifiedCourses { get; set; } = [];
    public IReadOnlyList<InstructorAvailabilityViewModel> AvailabilitySlots { get; set; } = [];
    public IReadOnlyList<InstructorAssignedSessionViewModel> AssignedSessions { get; set; } = [];
}

public class InstructorQualificationsEditViewModel
{
    public string InstructorId { get; set; } = string.Empty;
    public string InstructorName { get; set; } = string.Empty;
    public List<CourseQualificationOptionViewModel> Courses { get; set; } = [];
}

public class CourseQualificationOptionViewModel
{
    public int CourseId { get; set; }
    public string CourseName { get; set; } = string.Empty;
    public string SubjectName { get; set; } = string.Empty;
    public bool IsSelected { get; set; }
}

public class InstructorAvailabilityViewModel
{
    public int Id { get; set; }
    public DayOfWeek DayOfWeek { get; set; }
    public TimeSpan StartingTime { get; set; }
    public TimeSpan EndingTime { get; set; }
}

public class InstructorAvailabilityFormViewModel
{
    public string InstructorId { get; set; } = string.Empty;

    [Display(Name = "Day of week")]
    public DayOfWeek DayOfWeek { get; set; } = DayOfWeek.Monday;

    [Display(Name = "Starting time")]
    [DataType(DataType.Time)]
    public TimeSpan StartingTime { get; set; } = new(9, 0, 0);

    [Display(Name = "Ending time")]
    [DataType(DataType.Time)]
    public TimeSpan EndingTime { get; set; } = new(17, 0, 0);
}

public class InstructorAssignedSessionViewModel
{
    public int SessionId { get; set; }
    public string CourseName { get; set; } = string.Empty;
    public string ClassroomName { get; set; } = string.Empty;
    public DateOnly StartingDate { get; set; }
    public TimeSpan StartingTime { get; set; }
    public TimeSpan EndingTime { get; set; }
    public int EnrolledCount { get; set; }
}
