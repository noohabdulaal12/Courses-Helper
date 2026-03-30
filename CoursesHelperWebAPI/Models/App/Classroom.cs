using CoursesHelperWebAPI.Models.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoursesHelperWebAPI.Models.App
{
    public class Classroom
    {
        public int Id { get; set; }
        public required int TypeId { get; set; }

        [MaxLength(3000)]
        public string? Description { get; set; }

        [ForeignKey("TypeId")]
        public virtual ClassroomType ClassroomType { get; set; } = null!;
        public virtual ICollection<CourseSession> CourseSessions { get; set; } = [];
    }
}
