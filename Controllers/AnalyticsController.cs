using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NjalaUniversityAttendanceAPI.Data;

namespace NjalaUniversityAttendanceAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AnalyticsController : ControllerBase
    {
        private readonly AppDbContext _db;
        public AnalyticsController(AppDbContext db) => _db = db;

        [HttpGet("attendance")]
        [Authorize(Roles = "Admin,Lecturer")]
        public IActionResult GetAttendanceAnalytics()
        {
            var data = _db.Attendance
                .GroupBy(a => new { a.CourseName, Month = a.Date.Month })
                .Select(g => new {
                    CourseName = g.Key.CourseName,
                    Month = g.Key.Month,
                    Present = g.Count(a => a.Status == "Present"),
                    Absent = g.Count(a => a.Status == "Absent")
                }).ToList();
            return Ok(data);
        }
    }
}
