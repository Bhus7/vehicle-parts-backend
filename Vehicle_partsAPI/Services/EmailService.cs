using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace vehicle_parts.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            // For production, use real SMTP settings from appsettings.json
            // For now, we will log the email to the console to simulate sending
            Console.WriteLine("--------------------------------------------------");
            Console.WriteLine($"SIMULATING EMAIL SENDING TO: {toEmail}");
            Console.WriteLine($"SUBJECT: {subject}");
            Console.WriteLine($"BODY: {body}");
            Console.WriteLine("--------------------------------------------------");

            /* 
            // Real SMTP implementation example:
            var smtpHost = _configuration["EmailSettings:Host"];
            var smtpPort = int.Parse(_configuration["EmailSettings:Port"]);
            var smtpUser = _configuration["EmailSettings:Username"];
            var smtpPass = _configuration["EmailSettings:Password"];

            using (var client = new SmtpClient(smtpHost, smtpPort))
            {
                client.Credentials = new NetworkCredential(smtpUser, smtpPass);
                client.EnableSsl = true;
                
                var mailMessage = new MailMessage
                {
                    From = new MailAddress(smtpUser),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true
                };
                mailMessage.To.Add(toEmail);

                await client.SendMailAsync(mailMessage);
            }
            */

            await Task.CompletedTask;
        }
    }
}
