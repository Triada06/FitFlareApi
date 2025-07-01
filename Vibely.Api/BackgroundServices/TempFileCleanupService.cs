using FitFlare.Application.Services.Interfaces;

namespace FitFlare.Api.BackgroundServices;

public class TempFileCleanupService(
    IWebHostEnvironment env,
    ILogger<TempFileCleanupService> logger,
    IServiceScopeFactory serviceScopeFactory) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var tempPath = Path.Combine(env.WebRootPath, "temp");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                if (Directory.Exists(tempPath))
                {
                    var files = Directory.GetFiles(tempPath);
                    foreach (var file in files)
                    {
                        var lastModified = File.GetLastWriteTime(file);
                        if ((DateTime.Now - lastModified).TotalHours >= 3)
                        {
                            File.Delete(file);
                            logger.LogInformation("Deleted temp file: {File}", file);

                            //create a scope so we can safely use scoped services
                            using var scope = serviceScopeFactory.CreateScope();
                            var postService = scope.ServiceProvider.GetRequiredService<IPostService>();
                            await postService.DeleteDraftedMediaAsync();

                            logger.LogInformation("Deleted drafted media from DB.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error deleting temp files.");
            }

            await Task.Delay(TimeSpan.FromHours(3), stoppingToken); // TODO: Change to 6 hrs later
        }
    }
}