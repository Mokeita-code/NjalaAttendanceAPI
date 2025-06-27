using NjalaUniversityAttendanceAPI.DTOs;
using NjalaUniversityAttendanceAPI.Models;

namespace NjalaUniversityAttendanceAPI.Services.Interfaces
{
    public interface IAttendanceService
    {
        Task<(bool Success, string? ErrorMessage)> MarkAttendanceAsync(
             int lecturerId,
             int studentId,
             string courseName,
             string status,
             string performedBy);
        Task<List<Attendance>> GetAttendanceAsync(
            AttendanceFilterDto filter,
            int? lecturerId,
            string? role);

        Task<bool> UpdateAttendanceAsync(
            int attendanceId,
            string courseName,
            string status,
            DateTime date,
            string performedBy);
        Task<bool> DeleteAttendanceAsync(
            int attendanceId,
            string performedBy);
    }
}
