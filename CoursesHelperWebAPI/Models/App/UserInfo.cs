using CoursesHelperWebAPI.Models.Identity;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoursesHelperWebAPI.Models.App
{
    public class UserInfo
    {
        [Key] 
        public required string UserId { get; set; }

        [PersonalData]
        [MaxLength(200)]
        public required string FirstName { get; set; }

        [PersonalData]
        [MaxLength(200)]
        public required string LastName { get; set; }

        [PersonalData]
        public required DateOnly DateOfBirth {  get; set; }
        
        [PersonalData]
        [MaxLength(3000)]
        public string? Description { get; set; }

        [ForeignKey("UserId")]
        public virtual User User { get; set; } = null!;
        
    }
}
