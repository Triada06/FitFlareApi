using System.Linq.Expressions;
using FitFlare.Application.DTOs.AppUserDTos;
using FitFlare.Application.Helpers.Exceptions;
using FitFlare.Application.Mappings;
using FitFlare.Application.Services.Interfaces;
using FitFlare.Core.Entities;
using FitFlare.Infrastructure.Repositories.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;

namespace FitFlare.Application.Services;

public class AppUserService(IAppUserRepository repository, IBlobService blobService, UserManager<AppUser> userManager)
    : IAppUserService
{
    public async Task<bool> UpdateAsync(AppUserUpdateDto appUser, string userId)
    {
        if (string.IsNullOrWhiteSpace(userId))
            return false;
        var user = await repository.GetByIdAsync(userId);
        if (user == null)
            throw new UserNotFoundException();

        var template = appUser.UserName.Trim();
        if (await repository.AnyAsync(n => n.UserName == template && n.Id != userId))
            throw new UserAlreadyExistsException();

        if (appUser.ProfilePicture != null)
        {
            appUser.MapToAppUser(user, appUser.ProfilePicture.FileName);
            if (user.ProfilePicture != null)
                await blobService.DeleteBlobAsync(user.ProfilePicture);

            await blobService.UploadBlobAsync(appUser.ProfilePicture, appUser.ProfilePicture.FileName);
            await repository.UpdateAsync(user);
            return true;
        }

        appUser.MapToAppUser(user);
        return await repository.UpdateAsync(user);
    }

    public async Task<AppUserDto> CreateAsync(AppUserCreateDto appUser)
    {
        var template = appUser.UserName.Trim();
        var isExists = await repository.AnyAsync(u =>
            u.UserName != null && u.UserName.Trim() == template);
        if (isExists)
            throw new UserAlreadyExistsException();

        AppUser user;
        if (appUser.ProfilePicture != null)
        {
            user = appUser.MapToAppUser(appUser.ProfilePicture.FileName);
            await blobService.UploadBlobAsync(appUser.ProfilePicture, user.ProfilePicture);
            await repository.CreateAsync(user);
            await userManager.AddPasswordAsync(user, appUser.PassWord);
            string profilePictureUri = blobService.GetBlobSasUri(user.ProfilePicture);
            return user.MapToAppUserDto(profilePictureUri);
        }

        user = appUser.MapToAppUser();
        if (!await repository.CreateAsync(user)) throw new InternalServerErrorException();
        await userManager.AddPasswordAsync(user, appUser.PassWord);
        return user.MapToAppUserDto();
    }

    public async Task<bool> DeleteAsync(string userId)
    {
        var user = await repository.GetByIdAsync(userId);
        if (user == null)
            throw new UserNotFoundException();
        if (!string.IsNullOrWhiteSpace(user.ProfilePicture))
            await blobService.DeleteBlobAsync(user.ProfilePicture);
        return await repository.DeleteAsync(user);
    }

    public async Task<AppUserDto?> GetById(string id,
        Func<IQueryable<AppUser>, IIncludableQueryable<AppUser, object>>? include = null, bool tracking = true)
    {
        var user = await repository.GetByIdAsync(id, include, tracking);
        if (user == null)
            throw new UserNotFoundException();

        if (!string.IsNullOrWhiteSpace(user.ProfilePicture))
        {
            string profilePictureUri = blobService.GetBlobSasUri(user.ProfilePicture);
            return user.MapToAppUserDto(profilePictureUri);
        }

        return user.MapToAppUserDto();
    }

    public async Task<IEnumerable<AppUserDto>> GetAll(
        int page,
        string? sort,
        int pageSize = 5,
        bool tracking = true,
        string? searchText = null)
    {
        Func<IQueryable<AppUser>, IQueryable<AppUser>>? includeFunc = null;

        includeFunc = query => query
            .Include(au => au.Following)
            .Include(au => au.Followers)
            .Include(au => au.CommentLikes)
            .Include(au => au.Comments)
            .Include(au => au.Ratings);

        var users = await repository.GetAllAsync(page, pageSize, tracking, includeFunc);
        users = users.ToList();
        if (!users.Any())
            return [];

        var sorted = !string.IsNullOrWhiteSpace(sort)
            ? sort.Trim().ToLower() switch
            {
                "asc" => users.OrderBy(u => u.UserName),
                "desc" => users.OrderByDescending(u => u.UserName),
                _ => users
            }
            : users;

        if (!string.IsNullOrWhiteSpace(searchText))
        {
            searchText = searchText.Trim();
            sorted = sorted.Where(u =>
                u.UserName != null &&
                u.UserName.Contains(searchText, StringComparison.OrdinalIgnoreCase)
            );
        }

        List<AppUserDto> rerDtos = [];

        foreach (var user in sorted)
        {
            if (!string.IsNullOrWhiteSpace(user.ProfilePicture))
            {
                var uri = blobService.GetBlobSasUri(user.ProfilePicture);
                rerDtos.Add(user.MapToAppUserDto(uri));
            }
            else
            {
                rerDtos.Add(user.MapToAppUserDto());
            }
        }

        return rerDtos;
    }


    public async Task<IEnumerable<AppUserDto?>> Find(Expression<Func<AppUser, bool>> predicate,
        Func<IQueryable<AppUser>, IIncludableQueryable<AppUser, object>>? include = null, bool tracking = true)
    {
        var data = await repository.FindAsync(predicate, include, tracking);
        var appUsers = data.ToList();
        var returnDtos = new List<AppUserDto>();
        if (!appUsers.Any()) return returnDtos;
        foreach (var appUser in appUsers)
        {
            if (!string.IsNullOrWhiteSpace(appUser.ProfilePicture))
            {
                var uri = blobService.GetBlobSasUri(appUser.ProfilePicture);
                returnDtos.Add(appUser.MapToAppUserDto(uri));
            }
            else
            {
                returnDtos.Add(appUser.MapToAppUserDto());
            }
        }

        return returnDtos;
    }
}