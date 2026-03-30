using System.ComponentModel.DataAnnotations;

namespace CoursesHelperWebAPI.Models.App
{
    public class ClassroomType
    {
        public int Id { get; set; }

        [MaxLength(200)]
        public required string Name { get; set; }
        public required int Capacity { get; set; }

        [MaxLength(3000)]
        public string? Description { get; set; }
        public virtual ICollection<Classroom> Classrooms { get; set; } = [];
    }
}
