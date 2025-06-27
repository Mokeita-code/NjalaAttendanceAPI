using NjalaUniversityAttendanceAPI.DTOs;
using NjalaUniversityAttendanceAPI.Models;

namespace NjalaUniversityAttendanceAPI.Services.Interfaces
{
    public interface ICourseService
    {
        Task<Course> CreateCourseAsync(CreateCourseDto dto);
        Task<bool> UpdateCourseAsync(int id, CreateCourseDto dto);
        Task<bool> DeleteCourseAsync(int id);
        Task<List<Course>> GetAllCoursesAsync();
    }
}
