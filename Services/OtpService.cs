using NjalaUniversityAttendanceAPI.Services.Interfaces;
using System.Net;
using System.Net.Mail;

namespace NjalaUniversityAttendanceAPI.Services
{
  
        public class OtpService : IOtpService
        {
            private readonly IConfiguration _cfg;
            public OtpService(IConfiguration cfg) => _cfg = cfg;

            public void SendOtpEmail(string toEmail, string subject, string htmlContent)
            {
                var smtpSection = _cfg.GetSection("Smtp");
                var host = smtpSection["Host"];
                var portStr = smtpSection["Port"];
                var enableSsl = bool.Parse(smtpSection["EnableSsl"] ?? "true");
                var username = smtpSection["Username"];
                var password = smtpSection["Password"];
                var fromEmail = smtpSection["FromEmail"];
                var fromName = smtpSection["FromName"];

                if (string.IsNullOrEmpty(host) || string.IsNullOrEmpty(portStr))
                    throw new InvalidOperationException("SMTP configuration is missing.");

                int port = int.Parse(portStr);

                var message = new MailMessage();
                message.From = new MailAddress(fromEmail!, fromName);
                message.To.Add(new MailAddress(toEmail));
                message.Subject = subject;
                message.IsBodyHtml = true;
                message.Body = htmlContent;

                using var client = new SmtpClient(host, port)
                {
                    EnableSsl = enableSsl,
                    Credentials = new NetworkCredential(username, password)
                };
                client.Send(message);
            }
        }
    }

