// NjalaUniversityAttendanceAPI/Services/OtpService.cs

using Microsoft.Extensions.Configuration;
using NjalaUniversityAttendanceAPI.Services.Interfaces;
using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace NjalaUniversityAttendanceAPI.Services
{
    public class OtpService : IOtpService
    {
        private readonly IConfiguration _cfg;
        public OtpService(IConfiguration cfg) => _cfg = cfg;

        public async Task SendOtpEmailAsync(string toEmail, string subject, string htmlContent)
        {
            var smtp = _cfg.GetSection("Smtp");
            var host = smtp["Host"] ?? throw new InvalidOperationException("Smtp:Host is missing");
            var portStr = smtp["Port"] ?? throw new InvalidOperationException("Smtp:Port is missing");
            var enableSsl = bool.Parse(smtp["EnableSsl"] ?? "true");
            var username = smtp["Username"] ?? throw new InvalidOperationException("Smtp:Username is missing");
            var password = smtp["Password"] ?? throw new InvalidOperationException("Smtp:Password is missing");
            var fromEmail = smtp["FromEmail"] ?? username;
            var fromName = smtp["FromName"] ?? "";
            var timeoutMs = int.TryParse(smtp["Timeout"], out var t) ? t : 15000;

            if (!int.TryParse(portStr, out int port))
                throw new InvalidOperationException("Smtp:Port must be an integer");

            using var message = new MailMessage
            {
                From = new MailAddress(fromEmail, fromName),
                Subject = subject,
                Body = htmlContent,
                IsBodyHtml = true
            };
            message.To.Add(new MailAddress(toEmail));

            using var client = new SmtpClient(host, port)
            {
                EnableSsl = enableSsl,
                Credentials = new NetworkCredential(username, password),
                Timeout = timeoutMs
            };

            try
            {
                // SendMailAsync honors Timeout
                await client.SendMailAsync(message);
            }
            catch (SmtpException smtpEx) when (smtpEx.StatusCode == SmtpStatusCode.GeneralFailure)
            {
                throw new InvalidOperationException($"SMTP timeout after {timeoutMs}ms when sending to {toEmail}", smtpEx);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to send OTP email to {toEmail}: {ex.Message}", ex);
            }
        }
    }
}
