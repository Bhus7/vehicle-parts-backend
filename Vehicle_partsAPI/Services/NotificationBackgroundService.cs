using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace vehicle_parts.Services
{
    public class NotificationBackgroundService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<NotificationBackgroundService> _logger;
        private readonly TimeSpan _checkInterval = TimeSpan.FromHours(24); // Run once a day

        public NotificationBackgroundService(IServiceProvider serviceProvider, ILogger<NotificationBackgroundService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Notification Background Service is starting.");

            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Notification Background Service is running checks...");

                try
                {
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var notificationService = scope.ServiceProvider.GetRequiredService<INotificationService>();
                        await notificationService.ProcessNotificationsAsync();
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred while running notification checks.");
                }

                // Wait for the next interval
                await Task.Delay(_checkInterval, stoppingToken);
            }

            _logger.LogInformation("Notification Background Service is stopping.");
        }
    }
}
