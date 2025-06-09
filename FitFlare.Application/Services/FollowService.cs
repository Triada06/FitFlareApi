using FitFlare.Application.DTOs.AppUser;
using FitFlare.Application.DTOs.Notification;
using FitFlare.Application.Helpers.Exceptions;
using FitFlare.Application.Mappings;
using FitFlare.Application.Services.Interfaces;
using FitFlare.Core.Constants;
using FitFlare.Core.Entities;
using FitFlare.Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FitFlare.Application.Services;

public class FollowService(
    IFollowRepository followRepository,
    IAppUserRepository appUserRepository,
    IBlobService blobService,
    INotificationService notificationService) : IFollowService
{
    public async Task<IEnumerable<AppUserContextDto>?> GetFollowersByIdFollowingId(string followingId)
    {
        var followers = await followRepository.FindAsync(m => m.FollowingId == followingId,
            i => i.Include(m => m.Follower), tracking: false);
        if (!followers.Any()) return [];
        return followers
            .Select(m => m.MapFollowerToAppUserContextDto(m?.Follower.ProfilePictureUri is not null
                ? blobService.GetBlobSasUri(m.Follower.ProfilePictureUri)
                : null));
    }

    public async Task<IEnumerable<AppUserContextDto>?> GetFollowingsByIdFollowerId(string followerId)
    {
        var followers = await followRepository.FindAsync(m => m.FollowerId == followerId,
            i => i.Include(m => m.Following), tracking: false);
        if (!followers.Any()) return null;
        return followers
            .Select(m => m.MapFollowingToAppUserContextDto(m?.Following.ProfilePictureUri is not null
                ? blobService.GetBlobSasUri(m.Following.ProfilePictureUri)
                : null));
    }

    public async Task Follow(string followerId, string followingId)
    {
        var follower = await appUserRepository.GetByIdAsync(followerId, tracking: false);
        if (follower == null)
            throw new UserNotFoundException();
        var following = await appUserRepository.GetByIdAsync(followingId, tracking: false);
        if (following == null)
            throw new UserNotFoundException();
        if (following.Id == follower.Id)
            throw new BadRequestException("User can't follow itself");
        if (await followRepository.AnyAsync(m => m.FollowerId == follower.Id && m.FollowingId == following.Id))
            throw new BadRequestException("This user is already following");
        await followRepository.CreateAsync(new Follow { FollowerId = follower.Id, FollowingId = following.Id });
        await notificationService.CreateAsync(new CreateNotificationRequest
        {
            NotificationType = nameof(NotificationTypes.Follow),
            AddressedUserId = followingId,
            TriggeredUserId = followerId,
            PostId = null,
            PostMediaUri = null
        });
    }

    public async Task UnFollow(string followerId, string followingId)
    {
        var follower = await appUserRepository.GetByIdAsync(followerId, tracking: false);
        if (follower == null)
            throw new UserNotFoundException();
        var following = await appUserRepository.GetByIdAsync(followingId, tracking: false);
        if (following == null)
            throw new UserNotFoundException();
        if (following.Id == follower.Id)
            throw new BadRequestException("User can't unfollow itself");
        var follow = await followRepository
            .FindSingleAsync(f => f.FollowerId == follower.Id && f.FollowingId == following.Id);

        if (follow is null)
            throw new BadRequestException("You're already not following this user");
        await followRepository.DeleteAsync(follow);
    }
}