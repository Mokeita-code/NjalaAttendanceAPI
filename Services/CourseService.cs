using Microsoft.EntityFrameworkCore;
using NjalaUniversityAttendanceAPI.Data;
using NjalaUniversityAttendanceAPI.DTOs;
using NjalaUniversityAttendanceAPI.Models;
using NjalaUniversityAttendanceAPI.Services.Interfaces;

namespace NjalaUniversityAttendanceAPI.Services
{
    public class CourseService : ICourseService
    {
        private readonly AppDbContext _db;
        public CourseService(AppDbContext db) => _db = db;

        public async Task<Course> CreateCourseAsync(CreateCourseDto dto)
        {
            var course = new Course { Name = dto.Name };
            _db.Courses.Add(course);
            await _db.SaveChangesAsync();

            // Audit
            _db.AuditLogs.Add(new AuditLog
            {
                Action = $"Created course Id={course.Id}, Name={dto.Name}",
                PerformedBy = "Admin",
                Timestamp = System.DateTime.UtcNow
            });
            await _db.SaveChangesAsync();

            return course;
        }

        public async Task<bool> UpdateCourseAsync(int id, CreateCourseDto dto)
        {
            var c = await _db.Courses.FindAsync(id);
            if (c == null) return false;
            c.Name = dto.Name;
            await _db.SaveChangesAsync();

            _db.AuditLogs.Add(new AuditLog
            {
                Action = $"Updated course Id={id} to Name={dto.Name}",
                PerformedBy = "Admin",
                Timestamp = System.DateTime.UtcNow
            });
            await _db.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteCourseAsync(int id)
        {
            var c = await _db.Courses.FindAsync(id);
            if (c == null) return false;
            _db.Courses.Remove(c);
            _db.AuditLogs.Add(new AuditLog
            {
                Action = $"Deleted course Id={id}",
                PerformedBy = "Admin",
                Timestamp = System.DateTime.UtcNow
            });
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<List<Course>> GetAllCoursesAsync()
        {
            return await _db.Courses.ToListAsync();
        }
    }
}
