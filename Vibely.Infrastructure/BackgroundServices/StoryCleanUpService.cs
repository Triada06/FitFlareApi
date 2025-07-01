using FitFlare.Infrastructure.Repositories.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace FitFlare.Infrastructure.BackgroundServices;

public class StoryCleanUpService(IServiceScopeFactory scopeFactory) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using var scope = scopeFactory.CreateScope();
            var storyRepository = scope.ServiceProvider.GetRequiredService<IStoryRepository>();
            
            var thresholdDate = DateTime.UtcNow.AddDays(-1);
            var expiredStories = await storyRepository.FindAsync(n => n.CreatedAt < thresholdDate);
            
            if (expiredStories.Any())
            {
                await storyRepository.RemoveRange(expiredStories);
            }

            await Task.Delay(TimeSpan.FromMinutes(3), stoppingToken); //running every 3 minutes
        }
    }
}