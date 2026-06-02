namespace CoursesHelperWebAPI.DTOs
{
    public class CreateCourseDto
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int SubjectId { get; set; }
        public int NumberOfSessions { get; set; }
        public decimal Price { get; set; }
    }
}