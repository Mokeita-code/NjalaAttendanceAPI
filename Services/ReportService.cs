using NjalaUniversityAttendanceAPI.DTOs;
using NjalaUniversityAttendanceAPI.Helpers;
using NjalaUniversityAttendanceAPI.Services.Interfaces;

namespace NjalaUniversityAttendanceAPI.Services
{
    public class ReportService : IReportService
    {
        private readonly IAttendanceService _attSvc;

        public ReportService(IAttendanceService attSvc) => _attSvc = attSvc;

        public async Task<byte[]> GenerateExcelReportAsync(AttendanceFilterDto filter, int? lecturerId, string? role)
        {
            var attendanceList = await _attSvc.GetAttendanceAsync(filter, lecturerId, role);
            var excelRows = attendanceList
                .Select(a => new
                {
                    StudentName = a.Student?.FullName ?? "(Unknown)",
                    CourseName = a.CourseName,
                    Date = a.Date.ToString("yyyy-MM-dd"),
                    Status = a.Status
                })
                .ToList();

            // Use the ExcelHelper to generate the .xlsx byte array.
            var excelBytes = ExcelHelper.GenerateExcel(excelRows);
            return excelBytes;
        }
    }
}
