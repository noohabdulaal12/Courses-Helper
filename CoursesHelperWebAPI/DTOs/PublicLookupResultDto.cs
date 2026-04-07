namespace CoursesHelperWebAPI.DTOs
{
    public class PublicLookupResultDto
    {
        public string TraineeId { get; set; } = string.Empty;
        public string TraineeName { get; set; } = string.Empty;

        public int CertificationId { get; set; }
        public string CertificationName { get; set; } = string.Empty;

        public int TotalRequiredCourses { get; set; }
        public int CompletedCount { get; set; }

        public List<string> CompletedCourses { get; set; } = new();
        public List<string> PendingCourses { get; set; } = new();

        public bool IsEligible { get; set; }
    }
}