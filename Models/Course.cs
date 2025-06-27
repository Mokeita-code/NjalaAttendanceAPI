using System.ComponentModel.DataAnnotations;

namespace NjalaUniversityAttendanceAPI.Models
{
    public class Course
    {
        public int Id { get; set; }

        [Required]
        public string? Name { get; set; }
    }
}
