using FitFlare.Core.Entities;

namespace FitFlare.Infrastructure.Repositories.Interfaces;

public interface INotificationRepository : IBaseRepository<Notification>
{
    public Task MarkAllAsReadAsync(List<Notification> notifications);
    public Task MarkAsReadAsync(Notification notification);
    public Task RemoveRange(List<Notification?> notifications);
}