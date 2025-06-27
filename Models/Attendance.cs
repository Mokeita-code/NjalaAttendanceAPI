using System.ComponentModel.DataAnnotations;

namespace NjalaUniversityAttendanceAPI.Models
{
    public class Attendance
    {
        public int Id { get; set; }

        // The student whose attendance is recorded
        [Required]
        public int StudentId { get; set; }
        public Student? Student { get; set; }

        public int? LecturerId { get; set; }
        public Lecturer? Lecturer { get; set; }

        [Required]
        public string? CourseName { get; set; }

        [Required]
        public DateTime Date { get; set; }

        [Required]
        public string? Status { get; set; } 
    }
}
