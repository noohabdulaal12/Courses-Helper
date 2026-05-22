using System.ComponentModel.DataAnnotations;

namespace CoursesHelperMVC.Models;

public class CourseFormViewModel
{
    public int Id { get; set; }

    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(3000)]
    public string? Description { get; set; }

    [Display(Name = "Subject")]
    [Range(1, int.MaxValue, ErrorMessage = "Please choose a subject.")]
    public int SubjectId { get; set; }

    [Display(Name = "Number of sessions")]
    [Range(1, int.MaxValue, ErrorMessage = "Number of sessions must be at least 1.")]
    public int NumberOfSessions { get; set; } = 1;

    [Range(0.01, 999999999, ErrorMessage = "Price must be greater than 0.")]
    [DataType(DataType.Currency)]
    public decimal Price { get; set; } = 0.01m;
}
