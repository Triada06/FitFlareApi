using FitFlare.Application.DTOs.Notification;
using FitFlare.Application.Helpers.Exceptions;
using FitFlare.Application.Mappings;
using FitFlare.Application.Services.Interfaces;
using FitFlare.Core.Constants;
using FitFlare.Core.Entities;
using FitFlare.Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FitFlare.Application.Services.Shared;

public class NotificationService(
    INotificationRepository notificationRepository,
    IBlobService blobService,
    IAppUserRepository userRepository)
    : INotificationService
{
    public async Task<NotificationDto> CreateAsync(CreateNotificationRequest request)
    {
        var triggeredUser = await userRepository.GetByIdAsync(request.TriggeredUserId);
        if (triggeredUser == null)
            throw new UserNotFoundException();
        var notification = new Notification
        {
            UserId = request.AddressedUserId,
            Type = request.NotificationType,
            TriggeredById = request.TriggeredUserId,
            PostId = request.PostId,
            Message = GenerateNotificationMessage(request.NotificationType),
            IsRead = false
        };
        await notificationRepository.CreateAsync(notification);
        return notification.MapToNotificationDto(triggeredUser.UserName!,
            triggeredUser.ProfilePictureUri is not null
                ? blobService.GetBlobSasUri(triggeredUser.ProfilePictureUri)
                : null, request.PostId,
            request.PostMediaUri is not null ? blobService.GetBlobSasUri(request.PostMediaUri) : null);
    }

    public async Task MarkAsReadAsync(string notificationId)
    {
        var notification = await notificationRepository.GetByIdAsync(notificationId);
        if (notification == null)
            throw new NotFoundException("Notification not found");
        await notificationRepository.MarkAsReadAsync(notification);
    }

    public async Task MarkAllAsReadAsync(string userId)
    {
        var user = await userRepository.GetByIdAsync(userId);
        if (user == null)
            throw new NotFoundException("User not found");
        var notifications = await notificationRepository.FindAsync(m => m.UserId == userId && !m.IsRead);
        if (!notifications.Any()) return;
        await notificationRepository.MarkAllAsReadAsync(notifications!);
    }

    public async Task DeleteASync(string notificationId)
    {
        var notification = await notificationRepository.GetByIdAsync(notificationId);
        if (notification is null)
            throw new NotFoundException($"Notification was not found");
        await notificationRepository.DeleteAsync(notification);
    }

    public async Task<IEnumerable<NotificationDto>> GetAllByUserIdAsync(string userId)
    {
        var user = await userRepository.GetByIdAsync(userId);
        if (user == null)
            throw new UserNotFoundException();

        var notifications = await notificationRepository.FindAsync(
            m => m.UserId == userId,
            i => i
                .Include(m => m.TriggeredBy)
                .Include(m => m.Post)
        );

        if (!notifications.Any())
            return [];
    
        notifications = notifications.OrderByDescending(m=>m?.CreatedAt).ToList();
        return notifications.Select(n =>
        {
            var profilePicUri = n.TriggeredBy.ProfilePictureUri != null
                ? blobService.GetBlobSasUri(n.TriggeredBy.ProfilePictureUri)
                : null;

            string? postId = null;
            string? postMediaUri = null;

            if (n.Type is "Like" or "Comment" && n.Post != null)
            {
                postId = n.PostId;
                postMediaUri = blobService.GetBlobSasUri(n.Post.Media);
            }

            return n.MapToNotificationDto(
                n.TriggeredBy.UserName!,
                profilePicUri,
                postId,
                postMediaUri
            );
        });
    }


    public async Task<Notification?> GetByIdAsync(string notificationId)
    {
        var data = await notificationRepository.GetByIdAsync(notificationId);
        if (data is null)
            throw new NotFoundException($"Notification was not found");
        return data;
    }

    private static string GenerateNotificationMessage(string type) =>
        type switch
        {
            nameof(NotificationTypes.FollowRequest) => "requested to follow you",
            nameof(NotificationTypes.Follow) => "started following you",
            nameof(NotificationTypes.Like) => "liked your post",
            nameof(NotificationTypes.Comment) => "commented on your post",
            _ => throw new InternalServerErrorException("Notification type not is supported"),
        };
}