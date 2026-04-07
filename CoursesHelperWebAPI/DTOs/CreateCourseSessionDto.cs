namespace CoursesHelperWebAPI.DTOs
{
    public class CreateCourseSessionDto
    {
        public int CourseId { get; set; }
        public string InstructorId { get; set; } = string.Empty;
        public int ClassroomId { get; set; }
        public int MaxSeats { get; set; }
        public DateOnly StartingDate { get; set; }
        public TimeSpan StartingTime { get; set; }
        public TimeSpan EndingTime { get; set; }
        public DayOfWeek DayOfWeek { get; set; }
    }
}