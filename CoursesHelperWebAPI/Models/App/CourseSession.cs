using CoursesHelperWebAPI.Models.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoursesHelperWebAPI.Models.App
{
    public class CourseSession
    {
        public int Id { get; set; }
        public required int CourseId { get; set; }
        public required string InstructorId { get; set; }
        public required int ClassroomId { get; set; }
        public required int MaxSeats { get; set; }
        public required DateOnly StartingDate { get; set; }
        public required TimeSpan StartingTime { get; set; }
        public required TimeSpan EndingTime { get; set; }  
        public required DayOfWeek DayOfWeek { get; set; }

        [ForeignKey("CourseId")]
        public virtual Course Course { get; set; } = null!;

        [ForeignKey("InstructorId")]
        public virtual User Instructor { get; set; } = null!;

        [ForeignKey("ClassroomId")]
        public virtual Classroom Classroom { get; set; } = null!;
        public virtual ICollection<TraineeSession> TraineeSessions { get; set; } = [];
    }
}
