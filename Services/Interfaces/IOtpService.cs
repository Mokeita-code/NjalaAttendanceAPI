namespace NjalaUniversityAttendanceAPI.Services.Interfaces
{
    public interface IOtpService
    {
        void SendOtpEmail(string toEmail, string subject, string htmlContent);
    }
}
