using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoursesHelperWebAPI.Models.Identity
{
    public class ExtraPhone
    {
        public int Id { get; set; }
        public required string UserId { get; set; }

        [PersonalData]
        [Phone]
        [MaxLength(500)]
        public required string Phone { get; set; }

        [ForeignKey("UserId")]
        public virtual User User { get; set; } = null!;
    }
}
