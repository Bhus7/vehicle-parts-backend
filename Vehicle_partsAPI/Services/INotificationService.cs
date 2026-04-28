using System.Threading.Tasks;

namespace vehicle_parts.Services
{
    public interface INotificationService
    {
        Task ProcessNotificationsAsync();
    }
}
