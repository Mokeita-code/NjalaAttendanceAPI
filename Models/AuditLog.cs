namespace NjalaUniversityAttendanceAPI.Models
{
    public class AuditLog
    {
        public int Id { get; set; }
        public string? Action { get; set; } 
        public string? PerformedBy { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
