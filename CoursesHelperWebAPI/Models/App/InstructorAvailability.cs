using CoursesHelperWebAPI.Models.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoursesHelperWebAPI.Models.App
{
    public class InstructorAvailability
    {
        public int Id { get; set; }
        public required string InstructorId { get; set; }
        public required DayOfWeek DayOfWeek { get; set; }
        public required TimeSpan StartingTime { get; set; }
        public required TimeSpan EndingTime { get; set; }

        [ForeignKey("InstructorId")]
        public virtual User Instructor { get; set; } = null!;
    }
}
