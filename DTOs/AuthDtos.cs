namespace NjalaUniversityAttendanceAPI.DTOs
{
   
        public class RegisterStudentDto
        {
            public string? FullName { get; set; }
            public string? Email { get; set; }
            public string? Phone { get; set; }
            public string? Password { get; set; }
        }

        public class VerifyOtpDto
        {
            public string? Email { get; set; }
            public string? Otp { get; set; }
        }

        public class LoginDto
        {
            public string? Email { get; set; }
            public string? Password { get; set; }
        }

        public class LecturerRegisterDto
        {
            public string? FullName { get; set; }
            public string? Email { get; set; }
            public string? Password { get; set; }
        }
    }

