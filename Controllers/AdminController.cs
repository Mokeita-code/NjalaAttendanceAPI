using Microsoft.AspNetCore.Mvc;
using NjalaUniversityAttendanceAPI.DTOs;
using NjalaUniversityAttendanceAPI.Services.Interfaces;

namespace NjalaUniversityAttendanceAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AdminController : ControllerBase
    {
        private readonly IConfiguration _cfg;
        private readonly IAuthService _auth;

        public AdminController(IConfiguration cfg, IAuthService auth)
        {
            _cfg = cfg;
            _auth = auth;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginDto dto)
        {
            var adminUser = _cfg["AdminUser:Username"];
            var adminPwd = _cfg["AdminUser:Password"];
            if (dto.Email != adminUser || dto.Password != adminPwd)
                return Unauthorized("Invalid admin credentials.");
            var token = _auth.GenerateJwtToken(adminUser, "Admin");
            return Ok(new { Token = token, Role = "Admin" });
        }
    }
}
