using NjalaUniversityAttendanceAPI.DTOs;

namespace NjalaUniversityAttendanceAPI.Services.Interfaces
{
    public interface IReportService
    {
        Task<byte[]> GenerateExcelReportAsync(AttendanceFilterDto filter, int? lecturerId, string? role);
    }
}
