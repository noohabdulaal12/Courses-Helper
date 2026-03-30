using CoursesHelperWebAPI.Models.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoursesHelperWebAPI.Models.App
{
    public class InstructorQualification
    {
        public required string InstructorId { get; set; }
        public required int CourseId { get; set; }
        public required virtual User Instructor { get; set; } 
        public required virtual Course Course { get; set; } 
    }
}
