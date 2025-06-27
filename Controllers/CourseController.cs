using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NjalaUniversityAttendanceAPI.DTOs;
using NjalaUniversityAttendanceAPI.Services.Interfaces;

namespace NjalaUniversityAttendanceAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CourseController : ControllerBase
    {
        private readonly ICourseService _courseSvc;

        public CourseController(ICourseService courseSvc)
        {
            _courseSvc = courseSvc;
        }

        [HttpGet]
        [Authorize(Roles = "Admin,Lecturer,Student")]
        public async Task<IActionResult> GetAll()
        {
            var list = await _courseSvc.GetAllCoursesAsync();
            return Ok(list);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Add([FromBody] CreateCourseDto dto)
        {
            var course = await _courseSvc.CreateCourseAsync(dto);
            return Ok(course);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(int id, [FromBody] CreateCourseDto dto)
        {
            var ok = await _courseSvc.UpdateCourseAsync(id, dto);
            if (!ok) return NotFound();
            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var ok = await _courseSvc.DeleteCourseAsync(id);
            if (!ok) return NotFound();
            return NoContent();
        }
    }
}
