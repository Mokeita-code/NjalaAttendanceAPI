namespace NjalaUniversityAttendanceAPI.DTOs
{
    public class MarkAttendanceDto
    {
        public int StudentId { get; set; }
        // LecturerId will be derived from JWT; no need in DTO.
        public string? CourseName { get; set; }
        public string? Status { get; set; }
        public int? LecturerId { get; set; }
    }

    public class UpdateAttendanceDto
    {
        public string? CourseName { get; set; }
        public string? Status { get; set; }
        public DateTime Date { get; set; }
    }

    public class AttendanceFilterDto
    {
        public string? CourseName { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}
