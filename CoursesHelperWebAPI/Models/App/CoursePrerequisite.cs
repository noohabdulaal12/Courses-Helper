using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoursesHelperWebAPI.Models.App
{
    public class CoursePrerequisite
    {
        public required int CourseId { get; set; }
        public required int PrerequisiteId { get; set; }
        public virtual Course Course { get; set; } = null!;
        public virtual Course Prerequisite { get; set; } = null!;
    }
}
