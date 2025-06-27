using System.ComponentModel.DataAnnotations;

namespace NjalaUniversityAttendanceAPI.Models
{
    public class Student
    {
        public int Id { get; set; }

        [Required]
        public string? FullName { get; set; }

        [Required]
        public string? Email { get; set; }

        [Required]
        public string? Phone { get; set; }

        [Required]
        public string? PasswordHash { get; set; }

        // OTP properties
        public string? OtpCode { get; set; }
        public DateTime? OtpExpiry { get; set; }
        public bool IsVerified { get; set; }

        // Navigation
        public ICollection<Attendance> Attendances { get; set; } = new List<Attendance>();
    }
}
