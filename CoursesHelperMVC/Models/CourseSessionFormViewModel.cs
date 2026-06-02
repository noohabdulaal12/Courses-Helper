using System.ComponentModel.DataAnnotations;

namespace CoursesHelperMVC.Models;

public class CourseSessionFormViewModel
{
    public int Id { get; set; }

    [Display(Name = "Course")]
    [Range(1, int.MaxValue, ErrorMessage = "Please choose a course.")]
    public int CourseId { get; set; }

    [Display(Name = "Instructor")]
    [Required(ErrorMessage = "Please choose an instructor.")]
    public string InstructorId { get; set; } = string.Empty;

    [Display(Name = "Classroom")]
    [Range(1, int.MaxValue, ErrorMessage = "Please choose a classroom.")]
    public int ClassroomId { get; set; }

    [Display(Name = "Max seats")]
    [Range(1, int.MaxValue, ErrorMessage = "Max seats must be greater than 0.")]
    public int MaxSeats { get; set; } = 1;

    [Display(Name = "Starting date")]
    [DataType(DataType.Date)]
    public DateOnly StartingDate { get; set; } = DateOnly.FromDateTime(DateTime.Today);

    [Display(Name = "Starting time")]
    [DataType(DataType.Time)]
    public TimeSpan StartingTime { get; set; } = new(9, 0, 0);

    [Display(Name = "Ending time")]
    [DataType(DataType.Time)]
    public TimeSpan EndingTime { get; set; } = new(10, 0, 0);
}
