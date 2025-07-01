using FitFlare.Application.DTOs.AppUser;
using FitFlare.Application.Services.Interfaces;
using FitFlare.Core.Entities;

namespace FitFlare.Application.Mappings;

public static class FollowMapping
{
    public static AppUserContextDto? MapFollowingToAppUserContextDto(this Follow follow,
        string? followingUserProfilePictureSasUri)
    {
        return new AppUserContextDto
        {
            Id = follow.FollowingId,
            ProfilePictureUri = followingUserProfilePictureSasUri,
            UserName = follow.Following.UserName!
        };
    }

    public static AppUserContextDto? MapFollowerToAppUserContextDto(this Follow follow,
        string? followerUserProfilePictureSasUri)
    {
        return new AppUserContextDto
        {
            Id = follow.FollowerId,
            ProfilePictureUri = followerUserProfilePictureSasUri,
            UserName = follow.Follower.UserName!
        };
    }
}