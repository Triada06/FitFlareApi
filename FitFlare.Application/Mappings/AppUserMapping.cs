using FitFlare.Application.DTOs.AppUser;
using FitFlare.Application.DTOs.Chat;
using FitFlare.Application.DTOs.Posts;
using FitFlare.Core.Entities;
using Microsoft.AspNetCore.Identity;

namespace FitFlare.Application.Mappings;

public static class AppUserMapping
{
    public static AppUserDto MapToAppUserDto(this AppUser appUser, string? profilePictureUri = null,
        IEnumerable<PostDto?>? posts = null, IEnumerable<PostDto?>? savedPosts = null)
    {
        savedPosts = savedPosts?.ToList();
        return new AppUserDto
        {
            Id = appUser.Id,
            Email = appUser.Email!,
            FullName = appUser.FullName,
            Bio = appUser.Bio,
            UserName = appUser.UserName!,
            IsBanned = appUser.IsBanned,
            IsPrivate = appUser.IsPrivate,
            PostsCount = appUser.Posts.Count,
            ProfilePictureUri = profilePictureUri ?? profilePictureUri,
            Posts = posts?.ToList() ?? [],
            SavedPosts = savedPosts?.ToList() ?? [],
            FollowersCount = appUser.Followers.Count(m => m.FollowingId == appUser.Id),
            FollowingCount = appUser.Following.Count(m => m.FollowerId == appUser.Id),
        };
    }

    public static AppUser MapToAppUser(this AppUserUpdateDto userToMap, AppUser appUser, string? profilePicture = null)
    {
        if (profilePicture != null)
            profilePicture = Guid.NewGuid() + " - " + profilePicture;
        appUser.FullName = userToMap.FullName;
        appUser.UserName = userToMap.UserName;
        appUser.Bio = userToMap.Bio;
        appUser.ProfilePictureUri = profilePicture ?? appUser.ProfilePictureUri;

        return appUser;
    }

    public static AppUser MapToAppUser(this AppUserSignUpDto userToMap)
    {
        return new AppUser
        {
            UserName = userToMap.UserName,
            Email = userToMap.Email,
        };
    }

    public static AppUserContextDto MapToAppUserContextDto(this AppUser appUser, string? profilePictureUri = null)
    {
        return new AppUserContextDto
        {
            Id = appUser.Id,
            ProfilePictureUri = profilePictureUri,
            UserName = appUser.UserName!,
        };
    }

    public static ChatDto MapToChatDto(this AppUser appUser, string? profilePictureUri, string? lastMessage,
        DateTime? lastMessageDate)
    {
        return new ChatDto
        {
            Id = appUser.Id,
            ChatPicture = profilePictureUri,
            FullNameOrUserName = appUser.FullName ?? appUser.UserName!,
            LastMessage = null,
            LastMessageTime = lastMessageDate
        };
    }
}