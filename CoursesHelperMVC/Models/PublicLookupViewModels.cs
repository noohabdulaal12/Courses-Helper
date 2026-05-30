using System.ComponentModel.DataAnnotations;

namespace CoursesHelperMVC.Models;

public class PublicLookupRequestViewModel
{
    [Display(Name = "Trainee ID")]
    [Required(ErrorMessage = "Trainee ID is required.")]
    public string TraineeId { get; set; } = string.Empty;

    [Display(Name = "Certification ID")]
    [Range(1, int.MaxValue, ErrorMessage = "Certification ID is required.")]
    public int CertificationId { get; set; }
}

public class PublicLookupResultViewModel
{
    public string TraineeId { get; set; } = string.Empty;
    public string TraineeName { get; set; } = string.Empty;
    public int CertificationId { get; set; }
    public string CertificationName { get; set; } = string.Empty;
    public int TotalRequiredCourses { get; set; }
    public int CompletedCount { get; set; }
    public List<string> CompletedCourses { get; set; } = [];
    public List<string> PendingCourses { get; set; } = [];
    public bool IsEligible { get; set; }
}

public class PublicLookupPageViewModel
{
    public PublicLookupRequestViewModel Request { get; set; } = new();
    public PublicLookupResultViewModel? Result { get; set; }
    public string? ErrorMessage { get; set; }
}
