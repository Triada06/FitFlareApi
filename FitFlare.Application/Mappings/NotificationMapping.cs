using FitFlare.Application.DTOs.Notification;
using FitFlare.Core.Entities;

namespace FitFlare.Application.Mappings;

public static class NotificationMapping
{
    public static NotificationDto MapToNotificationDto(this Notification notification,string triggerUserName, string? triggerUserProfilePictureSasUrl,
        string? postId, string? postMediaUri)
    {
        return new NotificationDto
        {
            Id = notification.Id,
            Type = notification.Type,
            Message = notification.Message,
            AddressedUserId = notification.UserId,
            TriggerUserName = triggerUserName,
            TriggerUserProfilePicture = triggerUserProfilePictureSasUrl,
            PostId = notification.Type is "Like" or "Comment" ? postId : null,
            PostMediaUri = notification.Type is "Like" or "Comment" ? postMediaUri : null,
            IsRead = notification.IsRead
        };
    }
}