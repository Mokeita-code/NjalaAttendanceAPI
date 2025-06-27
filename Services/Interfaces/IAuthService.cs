namespace NjalaUniversityAttendanceAPI.Services.Interfaces
{
    public interface IAuthService
    {
        string HashPassword(string password);
        bool VerifyPassword(string password, string storedHash);
        string GenerateJwtToken(string userId, string role);
    }
}
