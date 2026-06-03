using Microsoft.AspNetCore.Mvc.Rendering;

namespace CoursesHelperMVC.Models;

public class CertificationProgressIndexViewModel
{
    public string? TraineeId { get; set; }
    public int? CertificationId { get; set; }
    public string Status { get; set; } = "All";
    public IReadOnlyList<SelectListItem> TraineeOptions { get; set; } = [];
    public IReadOnlyList<SelectListItem> CertificationOptions { get; set; } = [];
    public IReadOnlyList<CertificationProgressRowViewModel> Rows { get; set; } = [];
}

public class CertificationProgressRowViewModel
{
    public string TraineeId { get; set; } = string.Empty;
    public int CertificationId { get; set; }
    public string TraineeName { get; set; } = string.Empty;
    public string TraineeEmail { get; set; } = string.Empty;
    public string CertificationName { get; set; } = string.Empty;
    public IReadOnlyList<string> RequiredCourses { get; set; } = [];
    public IReadOnlyList<string> CompletedCourses { get; set; } = [];
    public IReadOnlyList<string> MissingCourses { get; set; } = [];
    public int RequiredCount => RequiredCourses.Count;
    public int CompletedCount => CompletedCourses.Count;
    public bool IsComplete => RequiredCount > 0 && MissingCourses.Count == 0;
    public int ProgressPercentage => RequiredCount == 0 ? 0 : (int)Math.Round((double)CompletedCount / RequiredCount * 100);
}

public class CertificationReferenceViewModel
{
    public string TraineeName { get; set; } = string.Empty;
    public string TraineeEmail { get; set; } = string.Empty;
    public string CertificationName { get; set; } = string.Empty;
    public IReadOnlyList<string> CompletedRequiredCourses { get; set; } = [];
    public DateOnly IssueDate { get; set; }
    public string ReferenceNumber { get; set; } = string.Empty;
}
