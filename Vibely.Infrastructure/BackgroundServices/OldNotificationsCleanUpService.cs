using FitFlare.Infrastructure.Data;
using FitFlare.Infrastructure.Repositories.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace FitFlare.Infrastructure.BackgroundServices;

public class OldNotificationsCleanUpService(IServiceScopeFactory scopeFactory, ILogger<OldNotificationsCleanUpService> l) : BackgroundService
{

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = scopeFactory.CreateScope();
                var notificationRepo = scope.ServiceProvider.GetRequiredService<INotificationRepository>();

                var thresholdDate = DateTime.UtcNow.AddDays(-30);
                var oldNotifications = await notificationRepo.FindAsync(n => n.CreatedAt < thresholdDate);

                if (oldNotifications.Any())
                {
                    await notificationRepo.RemoveRange(oldNotifications);
                }

                await Task.Delay(TimeSpan.FromHours(24), stoppingToken); //running once a day
            }
            catch (Exception e)
            {
                l.LogError("[{DT}]: {NAME} error: {MSG}", DateTime.Now, nameof(OldNotificationsCleanUpService), e.Message);
            }
        }

    }
}
