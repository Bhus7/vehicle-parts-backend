using System.Threading.Tasks;

namespace vehicle_parts.Services
{
    public interface IEmailService
    {
        Task SendEmailAsync(string toEmail, string subject, string body);
    }
}
