using FitFlare.Core.Entities;
using FitFlare.Infrastructure.Data;
using FitFlare.Infrastructure.Repositories.Interfaces;

namespace FitFlare.Infrastructure.Repositories;

public class NotificationRepository(AppDbContext context)
    : BaseRepository<Notification>(context), INotificationRepository
{
    private readonly AppDbContext _context1 = context;

    public async Task MarkAllAsReadAsync(List<Notification> notifications)
    {
        foreach (var notification in notifications)
            notification.IsRead = true;
        await _context1.SaveChangesAsync();
    }

    public async Task MarkAsReadAsync(Notification notification)
    {
        notification.IsRead = true;
        await _context1.SaveChangesAsync();
    }
}