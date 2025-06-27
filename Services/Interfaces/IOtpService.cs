// NjalaUniversityAttendanceAPI/Services/Interfaces/IOtpService.cs
using System.Threading.Tasks;

namespace NjalaUniversityAttendanceAPI.Services.Interfaces
{
    public interface IOtpService
    {
        Task SendOtpEmailAsync(string toEmail, string subject, string htmlContent);
    }
}
