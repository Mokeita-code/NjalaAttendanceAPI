using Microsoft.EntityFrameworkCore;
using NjalaUniversityAttendanceAPI.Data;
using NjalaUniversityAttendanceAPI.DTOs;
using NjalaUniversityAttendanceAPI.Models;
using NjalaUniversityAttendanceAPI.Services.Interfaces;

namespace NjalaUniversityAttendanceAPI.Services
{
    public class AttendanceService : IAttendanceService
    {
        private readonly AppDbContext _db;
        public AttendanceService(AppDbContext db) => _db = db;

        public async Task<(bool Success, string? ErrorMessage)> MarkAttendanceAsync(
             int lecturerId,
             int studentId,
             string courseName,
             string status,
             string performedBy)
        {
            // Validate Student exists
            var student = await _db.Students.FindAsync(studentId);
            if (student == null)
            {
                return (false, $"Student with Id={studentId} not found.");
            }

            // Validate Lecturer exists
            var lecturer = await _db.Lecturers.FindAsync(lecturerId);
            if (lecturer == null)
            {
                return (false, $"Lecturer with Id={lecturerId} not found.");
            }

            // Optional: Validate that the lecturer actually teaches this course (if you track that relation).
            // If you have a LecturerCourse table, check here:
            // bool teaches = await _db.LecturerCourses.AnyAsync(lc => lc.LecturerId == lecturerId && lc.CourseId == ...);
            // if (!teaches) return (false, "Lecturer does not teach this course.");

            // Create attendance record
            var record = new Attendance
            {
                StudentId = studentId,
                LecturerId = lecturerId,
                CourseName = courseName,
                Status = status,
                Date = DateTime.UtcNow
            };
            _db.Attendance.Add(record);

            // Audit log
            _db.AuditLogs.Add(new AuditLog
            {
                Action = $"Marked attendance: StudentId={studentId}, Course={courseName}, Status={status}",
                PerformedBy = performedBy,
                Timestamp = DateTime.UtcNow
            });

            await _db.SaveChangesAsync();
            return (true, null);
        }

        public async Task<List<Attendance>> GetAttendanceAsync(
            AttendanceFilterDto filter,
            int? lecturerId,
            string? role)
        {
            var query = _db.Attendance.Include(a => a.Student).AsQueryable();

            // If caller is Lecturer, restrict to their records
            if (role == "Lecturer" && lecturerId.HasValue)
            {
                query = query.Where(a => a.LecturerId == lecturerId.Value);
            }

            if (!string.IsNullOrWhiteSpace(filter.CourseName))
                query = query.Where(a => a.CourseName == filter.CourseName);

            if (filter.StartDate.HasValue)
                query = query.Where(a => a.Date >= filter.StartDate.Value);

            if (filter.EndDate.HasValue)
                query = query.Where(a => a.Date <= filter.EndDate.Value);

            return await query.ToListAsync();
        }


        public async Task<bool> UpdateAttendanceAsync(
             int attendanceId,
             string courseName,
             string status,
             DateTime date,
             string performedBy)
        {
            var record = await _db.Attendance.FindAsync(attendanceId);
            if (record == null) return false;

            record.CourseName = courseName;
            record.Status = status;
            record.Date = date;

            // Audit
            _db.AuditLogs.Add(new AuditLog
            {
                Action = $"Updated attendance Id={attendanceId}: Course={courseName}, Status={status}, Date={date:yyyy-MM-dd}",
                PerformedBy = performedBy,
                Timestamp = DateTime.UtcNow
            });

            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAttendanceAsync(
            int attendanceId,
            string performedBy)
        {
            var record = await _db.Attendance.FindAsync(attendanceId);
            if (record == null) return false;

            _db.Attendance.Remove(record);
            _db.AuditLogs.Add(new AuditLog
            {
                Action = $"Deleted attendance Id={attendanceId}",
                PerformedBy = performedBy,
                Timestamp = DateTime.UtcNow
            });
            await _db.SaveChangesAsync();
            return true;
        }
    }
}
