using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoursesHelperWebAPI.Models.Identity
{
    public class ExtraEmail
    {
        public int Id { get; set; }
        public required string UserId { get; set; }

        [PersonalData]
        [EmailAddress]
        [MaxLength(500)]
        public required string Email { get; set; }

        [ForeignKey("UserId")]
        public virtual User User { get; set; } = null!;
    }
}
