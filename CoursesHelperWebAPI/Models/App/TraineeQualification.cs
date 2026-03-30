using CoursesHelperWebAPI.Models.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoursesHelperWebAPI.Models.App
{
    public class TraineeQualification
    {
        public required string TraineeId { get; set; }
        public required int CourseId { get; set; }
        public virtual User Trainee { get; set; } = null!;
        public virtual Course Course { get; set; } = null!;
    }
}
