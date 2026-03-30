using System.ComponentModel.DataAnnotations;

namespace CoursesHelperWebAPI.Models.App
{
    public class TakenTogether
    {
        public int Id { get; set; }

        [MaxLength(200)]
        public required string Name { get; set; }

        [MaxLength(3000)]
        public string? Description { get; set; }
        public virtual ICollection<TakenTogetherCourse> TakenTogetherCourses { get; set; } = [];
    }
}
