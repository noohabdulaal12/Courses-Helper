using System.ComponentModel.DataAnnotations;

namespace CoursesHelperMVC.Models;

public class SubjectFormViewModel
{
    public int Id { get; set; }

    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(3000)]
    public string? Description { get; set; }
}
