using System.ComponentModel.DataAnnotations;

namespace NjalaUniversityAttendanceAPI.Models
{
    public class Lecturer
    {
        public int Id { get; set; }

        [Required]
        public string? FullName { get; set; }

        [Required]
        public string? Email { get; set; }

        [Required]
        public string? PasswordHash { get; set; }
    }
}
