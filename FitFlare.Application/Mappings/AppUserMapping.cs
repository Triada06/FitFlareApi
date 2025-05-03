using FitFlare.Application.DTOs.AppUserDTos;
using FitFlare.Core.Entities;
using Microsoft.AspNetCore.Identity;

namespace FitFlare.Application.Mappings;

public static class AppUserMapping
{
    public static AppUserDto MapToAppUserDto(this AppUser appUser, string? profilePictureUri = null)
    {
        return new AppUserDto
        {
            Id = appUser.Id,
            Email = appUser.Email,
            FullName = appUser.FullName,
            UserName = appUser.UserName,
            IsBanned = appUser.IsBanned,
            PostsCount = appUser.Posts.Count,
            ProfilePictureUri = profilePictureUri ?? profilePictureUri,
        };
    }

    public static AppUser MapToAppUser(this AppUserUpdateDto userToMap, AppUser appUser, string? profilePicture = null)
    {
        appUser.FullName = userToMap.FullName;
        appUser.UserName = userToMap.UserName;
        appUser.ProfilePicture = profilePicture ?? appUser.ProfilePicture;

        return appUser;
    }

    public static AppUser MapToAppUser(this AppUserCreateDto userToMap, string? profilePicture = null)
    {
        return new AppUser
        {
            FullName = userToMap.FullName,
            UserName = userToMap.UserName,
            Email = userToMap.Email,
            ProfilePicture = $"{Guid.NewGuid()} - {profilePicture}",
        };
    }
}