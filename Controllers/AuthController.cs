using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NjalaUniversityAttendanceAPI.Data;
using NjalaUniversityAttendanceAPI.DTOs;
using NjalaUniversityAttendanceAPI.Models;
using NjalaUniversityAttendanceAPI.Services.Interfaces;

namespace NjalaUniversityAttendanceAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly IAuthService _auth;
        private readonly IOtpService _otp;

        public AuthController(AppDbContext db, IAuthService auth, IOtpService otp)
        {
            _db = db;
            _auth = auth;
            _otp = otp;
        }

        // Register student: send OTP email
        [HttpPost("register")]
        public async Task<IActionResult> RegisterStudent([FromBody] RegisterStudentDto dto)
        {
            if (await _db.Students.AnyAsync(s => s.Email == dto.Email))
                return BadRequest("Email already registered.");

            var hash = _auth.HashPassword(dto.Password);
            var otpCode = new Random().Next(100000, 999999).ToString();
            var expiry = DateTime.UtcNow.AddMinutes(5);

            var student = new Student
            {
                FullName = dto.FullName,
                Email = dto.Email,
                Phone = dto.Phone,
                PasswordHash = hash,
                OtpCode = otpCode,
                OtpExpiry = expiry,
                IsVerified = false
            };
            _db.Students.Add(student);
            await _db.SaveChangesAsync();

            // Send OTP via SMTP
            var subject = "Your OTP Code";
            var html = $"<p>Your OTP code is <strong>{otpCode}</strong>. It expires at {expiry:u} UTC.</p>";
            try
            {
                _otp.SendOtpEmail(dto.Email, subject, html);
            }
            catch (Exception ex)
            {
                // Log error, but continue so user can retry
                Console.Error.WriteLine($"Failed to send OTP email: {ex.Message}");
            }

            return Ok(new { Message = "Registered. Check your email for OTP." });
        }

        // Verify OTP
        [HttpPost("verify")]
        public async Task<IActionResult> VerifyOtp([FromBody] VerifyOtpDto dto)
        {
            var student = await _db.Students.SingleOrDefaultAsync(s => s.Email == dto.Email);
            if (student == null) return BadRequest("Invalid email.");

            if (student.IsVerified)
                return BadRequest("Already verified.");

            if (student.OtpCode == null || student.OtpExpiry == null || DateTime.UtcNow > student.OtpExpiry)
                return BadRequest("OTP expired. Please register again.");

            if (student.OtpCode != dto.Otp)
                return BadRequest("Invalid OTP.");

            student.IsVerified = true;
            student.OtpCode = null;
            student.OtpExpiry = null;
            await _db.SaveChangesAsync();
            return Ok(new { Message = "Verification successful. You can now log in." });
        }

        // Student login
        [HttpPost("login")]
        public async Task<IActionResult> LoginStudent([FromBody] LoginDto dto)
        {
            var student = await _db.Students.SingleOrDefaultAsync(s => s.Email == dto.Email);
            if (student == null || !_auth.VerifyPassword(dto.Password, student.PasswordHash))
                return Unauthorized("Invalid credentials.");
            if (!student.IsVerified)
                return Unauthorized("Email not verified.");

            var token = _auth.GenerateJwtToken(student.Id.ToString(), "Student");
            return Ok(new { Token = token, Role = "Student" });
        }

        // Lecturer registration (optional; or seed manually)
        [HttpPost("lecturer/register")]
        public async Task<IActionResult> RegisterLecturer([FromBody] LecturerRegisterDto dto)
        {
            if (await _db.Lecturers.AnyAsync(l => l.Email == dto.Email))
                return BadRequest("Email already registered.");

            var hash = _auth.HashPassword(dto.Password);
            var lecturer = new Lecturer
            {
                FullName = dto.FullName,
                Email = dto.Email,
                PasswordHash = hash
            };
            _db.Lecturers.Add(lecturer);
            await _db.SaveChangesAsync();
            return Ok(new { Message = "Lecturer registered." });
        }

        // Lecturer login
        [HttpPost("lecturer/login")]
        public async Task<IActionResult> LoginLecturer([FromBody] LoginDto dto)
        {
            var lecturer = await _db.Lecturers.SingleOrDefaultAsync(l => l.Email == dto.Email);
            if (lecturer == null || !_auth.VerifyPassword(dto.Password, lecturer.PasswordHash))
                return Unauthorized("Invalid credentials.");

            var token = _auth.GenerateJwtToken(lecturer.Id.ToString(), "Lecturer");
            return Ok(new { Token = token, Role = "Lecturer" });
        }
    }
}
