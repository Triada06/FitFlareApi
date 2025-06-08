using FitFlare.Application.DTOs.Notification;
using FitFlare.Core.Entities;

namespace FitFlare.Application.Services.Interfaces;

public interface INotificationService
{
    public Task<NotificationDto> CreateAsync(CreateNotificationRequest request);
    public Task MarkAsReadAsync(string notificationId);
    public Task MarkAllAsReadAsync(string userId);
    public Task DeleteASync(string notificationId);
    public Task<IEnumerable<Notification>> GetAllByUserIdAsync(string userId);
    public Task<Notification?> GetByUserIdAsync(string userId);
}