using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NjalaUniversityAttendanceAPI.DTOs;
using NjalaUniversityAttendanceAPI.Services.Interfaces;
using System.Security.Claims;

namespace NjalaUniversityAttendanceAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReportController : ControllerBase
    {
        private readonly IReportService _reportSvc;

        public ReportController(IReportService reportSvc)
        {
            _reportSvc = reportSvc;
        }
        [HttpPost("excel")]
        [Authorize(Roles = "Admin,Lecturer")]
        public async Task<IActionResult> DownloadExcelReport([FromBody] AttendanceFilterDto filter)
        {
            // Determine caller role and lecturerId
            var role = User.FindFirst(ClaimTypes.Role)?.Value;
            int? lecturerId = null;
            if (role == "Lecturer")
            {
                var idStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (!int.TryParse(idStr, out int lecId))
                    return Unauthorized("Invalid lecturer identifier in token.");
                lecturerId = lecId;
            }
            // Admin: lecturerId remains null, so ReportService returns all matching records

            var excelBytes = await _reportSvc.GenerateExcelReportAsync(filter, lecturerId, role);

            // Return as file:
            var fileName = "AttendanceReport.xlsx";
            const string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            return File(excelBytes, contentType, fileName);
        }
    }
}
