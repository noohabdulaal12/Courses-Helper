using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoursesHelperWebAPI.Models.App
{
    public class TakenTogetherCourse
    {
        public required int TakenTogetherId { get; set; }
        public required int CourseId { get; set; }
        public required virtual TakenTogether TakenTogether { get; set; } 
        public required virtual Course Course { get; set; } 
    }
}
