using FitFlare.Application.DTOs.AppUserDTos;
using FitFlare.Application.DTOs.Posts;
using FitFlare.Core.Entities;
using Microsoft.AspNetCore.Identity;

namespace FitFlare.Application.Mappings;

public static class AppUserMapping
{
    public static AppUserDto MapToAppUserDto(this AppUser appUser,string? profilePictureUri = null,IEnumerable<PostDto?>? posts = null )
    {
        return new AppUserDto
        {
            Id = appUser.Id,
            Email = appUser.Email!,
            FullName = appUser.FullName,
            UserName = appUser.UserName!,
            IsBanned = appUser.IsBanned,
            PostsCount = appUser.Posts.Count,
            ProfilePictureUri = profilePictureUri ?? profilePictureUri,
            Posts = posts?.ToList() ?? [],
        };
    }

    public static AppUser MapToAppUser(this AppUserUpdateDto userToMap, AppUser appUser, string? profilePicture = null)
    {
        appUser.FullName = userToMap.FullName;
        appUser.UserName = userToMap.UserName;
        appUser.ProfilePictureUri = profilePicture ?? appUser.ProfilePictureUri;

        return appUser;
    }

    public static AppUser MapToAppUser(this AppUserSignUpDto userToMap, string? profilePicture = null)
    {
        return new AppUser
        {
            UserName = userToMap.UserName,
            Email = userToMap.Email,
            ProfilePictureUri = $"{Guid.NewGuid()} - {profilePicture}",
        };
    }
}