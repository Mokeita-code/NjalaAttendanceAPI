// NjalaUniversityAttendanceAPI/Controllers/AuthController.cs

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NjalaUniversityAttendanceAPI.Data;
using NjalaUniversityAttendanceAPI.DTOs;
using NjalaUniversityAttendanceAPI.Models;
using NjalaUniversityAttendanceAPI.Services.Interfaces;
using System;
using System.Threading.Tasks;

namespace NjalaUniversityAttendanceAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly IAuthService _auth;
        private readonly IOtpService _otp;

        public AuthController(
            AppDbContext db,
            IAuthService auth,
            IOtpService otp)
        {
            _db = db;
            _auth = auth;
            _otp = otp;
        }

        /// <summary>
        /// Register a new student and send an OTP email for verification.
        /// </summary>
        [HttpPost("register")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> RegisterStudent([FromBody] RegisterStudentDto dto)
        {
            if (await _db.Students.AnyAsync(s => s.Email == dto.Email))
                return BadRequest("Email already registered.");

            // Hash the password
            var hash = _auth.HashPassword(dto.Password);

            // Generate OTP
            var otpCode = new Random().Next(100000, 999999).ToString();
            var expiry = DateTime.UtcNow.AddMinutes(5);

            // Create student record
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

            // Send OTP email
            var subject = "Your Njala University OTP Code";
            var html = $@"
                <p>Dear {dto.FullName},</p>
                <p>Your OTP code is: <strong>{otpCode}</strong></p>
                <p>This code will expire at <em>{expiry:u}</em> UTC.</p>
                <p>If you did not request this, please ignore this email.</p>";
            try
            {
                await _otp.SendOtpEmailAsync(dto.Email, subject, html);
            }
            catch (Exception ex)
            {
                // Log the exception for diagnostics; do not fail the registration entirely
                Console.Error.WriteLine($"[OtpService] SendOtpEmailAsync failed: {ex}");
            }

            return Ok(new { Message = "Registration successful. Check your email for the OTP code." });
        }

        /// <summary>
        /// Verify the OTP sent to the student’s email.
        /// </summary>
        [HttpPost("verify")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> VerifyOtp([FromBody] VerifyOtpDto dto)
        {
            var student = await _db.Students.SingleOrDefaultAsync(s => s.Email == dto.Email);
            if (student == null)
                return BadRequest("Invalid email address.");

            if (student.IsVerified)
                return BadRequest("Email already verified.");

            if (student.OtpExpiry == null || DateTime.UtcNow > student.OtpExpiry)
                return BadRequest("OTP expired. Please register again.");

            if (student.OtpCode != dto.Otp)
                return BadRequest("Invalid OTP code.");

            student.IsVerified = true;
            student.OtpCode = null;
            student.OtpExpiry = null;
            await _db.SaveChangesAsync();

            return Ok(new { Message = "Email verification successful. You may now log in." });
        }

        /// <summary>
        /// Authenticate a verified student and issue a JWT.
        /// </summary>
        [HttpPost("login")]
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        public async Task<IActionResult> LoginStudent([FromBody] LoginDto dto)
        {
            var student = await _db.Students.SingleOrDefaultAsync(s => s.Email == dto.Email);
            if (student == null || !_auth.VerifyPassword(dto.Password, student.PasswordHash))
                return Unauthorized("Invalid credentials.");

            if (!student.IsVerified)
                return Unauthorized("Email not verified. Please complete OTP verification.");

            var token = _auth.GenerateJwtToken(student.Id.ToString(), "Student");
            return Ok(new { Token = token, Role = "Student" });
        }

        /// <summary>
        /// Register a new lecturer. Admin-only can call this.
        /// </summary>
        [HttpPost("lecturer/register")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
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

            return Ok(new { Message = "Lecturer registered successfully." });
        }

        /// <summary>
        /// Authenticate a lecturer and issue a JWT.
        /// </summary>
        [HttpPost("lecturer/login")]
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        public async Task<IActionResult> LoginLecturer([FromBody] LoginDto dto)
        {
            var lecturer = await _db.Lecturers.SingleOrDefaultAsync(l => l.Email == dto.Email);
            if (lecturer == null || !_auth.VerifyPassword(dto.Password, lecturer.PasswordHash))
                return Unauthorized("Invalid lecturer credentials.");

            var token = _auth.GenerateJwtToken(lecturer.Id.ToString(), "Lecturer");
            return Ok(new { Token = token, Role = "Lecturer" });
        }
    }
}
