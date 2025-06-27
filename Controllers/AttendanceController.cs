using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NjalaUniversityAttendanceAPI.DTOs;
using NjalaUniversityAttendanceAPI.Services.Interfaces;
using System.Security.Claims;

namespace NjalaUniversityAttendanceAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AttendanceController : ControllerBase
    {
        private readonly IAttendanceService _attSvc;

        public AttendanceController(IAttendanceService attSvc)
        {
            _attSvc = attSvc;
        }

        // Mark attendance: Admin or Lecturer
        [HttpPost("mark")]
        [Authorize(Roles = "Lecturer,Admin")]
        public async Task<IActionResult> MarkAttendance([FromBody] MarkAttendanceDto dto)
        {
            // Determine caller role and lecturerId
            var role = User.FindFirst(ClaimTypes.Role)?.Value;
            int? lecturerId = null;
            string performedBy;

            if (role == "Lecturer")
            {
                // Extract lecturerId from token
                var idStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (!int.TryParse(idStr, out int lecId))
                {
                    return Unauthorized("Invalid lecturer identifier in token.");
                }
                lecturerId = lecId;
                performedBy = $"Lecturer:{lecturerId}";
            }
            else if (role == "Admin")
            {
                // Admin must supply which lecturer this attendance is for
                if (dto.LecturerId == null)
                {
                    return BadRequest("Admin marking requires a valid LecturerId in the request.");
                }
                lecturerId = dto.LecturerId;
                performedBy = $"Admin:{User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "unknown"}";
            }
            else
            {
                return Forbid();
            }

            // Call service
            var result = await _attSvc.MarkAttendanceAsync(lecturerId.Value, dto.StudentId, dto.CourseName, dto.Status, performedBy);
            if (!result.Success)
            {
                // Return 400 Bad Request with error message from service
                return BadRequest(new { Error = result.ErrorMessage });
            }

            return Ok(new { Message = "Attendance marked." });
        }

        // Get filtered attendance: Admin or Lecturer
        [HttpPost("filter")]
        [Authorize(Roles = "Admin,Lecturer")]
        public async Task<IActionResult> FilterAttendance([FromBody] AttendanceFilterDto filter)
        {
            // If Lecturer, optionally filter to only their records. Service can handle that if needed.
            var role = User.FindFirst(ClaimTypes.Role)?.Value;
            int? lecturerId = null;
            if (role == "Lecturer")
            {
                var idStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (int.TryParse(idStr, out int lecId))
                    lecturerId = lecId;
            }
            var list = await _attSvc.GetAttendanceAsync(filter, lecturerId, role);
            return Ok(list);
        }

        // Update attendance
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateAttendance(int id, [FromBody] UpdateAttendanceDto dto)
        {
            // performedBy for audit:
            var performedBy = $"Admin:{User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "unknown"}";
            var result = await _attSvc.UpdateAttendanceAsync(id, dto.CourseName, dto.Status, dto.Date, performedBy);
            if (!result)
                return NotFound();
            return NoContent();
        }

        // Delete attendance
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteAttendance(int id)
        {
            var performedBy = $"Admin:{User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "unknown"}";
            var result = await _attSvc.DeleteAttendanceAsync(id, performedBy);
            if (!result)
                return NotFound();
            return NoContent();
        }
    }
}
