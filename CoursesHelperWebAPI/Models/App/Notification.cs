using CoursesHelperWebAPI.Models.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoursesHelperWebAPI.Models.App
{
    public class Notification
    {
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; } = string.Empty;

        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required]
        [MaxLength(1000)]
        public string Message { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Link { get; set; }

        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

        public DateTime? ReadAtUtc { get; set; }

        [NotMapped]
        public bool IsRead => ReadAtUtc is not null;

        public virtual User User { get; set; } = null!;
    }
}
