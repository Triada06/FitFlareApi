using System.IdentityModel.Tokens.Jwt;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Text;
using FitFlare.Application.Contracts.Requests;
using FitFlare.Application.Contracts.Responses;
using FitFlare.Application.DTOs.AppUser;
using FitFlare.Application.Helpers.Exceptions;
using FitFlare.Application.Mappings;
using FitFlare.Application.Services.Interfaces;
using FitFlare.Core;
using FitFlare.Core.Entities;
using FitFlare.Infrastructure.Repositories.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace FitFlare.Application.Services;

public class AppUserService(
    IAppUserRepository appUserRepository,
    IBlobService blobService,
    UserManager<AppUser> userManager,
    IPostService postService,
    IPostSaveRepository postSaveRepository,
    IPostLikeRepository postLikeRepository,
    IConfiguration config)
    : IAppUserService
{
    public async Task UpdateAsync(AppUserUpdateDto appUser, string userId)
    {
        if (string.IsNullOrWhiteSpace(userId))
            throw new BadRequestException("UserId is required");
        var user = await appUserRepository.GetByIdAsync(userId);
        if (user == null)
            throw new UserNotFoundException();

        var template = appUser.UserName.Trim();
        if (await appUserRepository.AnyAsync(n => n.UserName == template && n.Id != userId))
            throw new UserAlreadyExistsException("This username already taken");

        if (appUser.ProfilePicture != null)
        {
            appUser.MapToAppUser(user, appUser.ProfilePicture.FileName);
            if (user.ProfilePictureUri != null)
                await blobService.DeleteBlobAsync(user.ProfilePictureUri);

            await blobService.UploadBlobAsync(appUser.ProfilePicture, user.ProfilePictureUri!);
            await userManager.UpdateAsync(user);
            return;
        }

        appUser.MapToAppUser(user);
        var res = await userManager.UpdateAsync(user);
        if (!res.Succeeded)
            throw new InternalServerErrorException("Failed to update user");
    }

    public async Task<AuthResponse> SignUpAsync(AppUserSignUpDto appUser)
    {
        var template = appUser.UserName.Trim();
        var isExists = await appUserRepository.AnyAsync(u =>
            u.UserName != null && u.UserName.Trim() == template);
        if (isExists)
            throw new UserAlreadyExistsException("This username already taken, try a different one");

        var user = appUser.MapToAppUser();
        var res = await userManager.CreateAsync(user, appUser.PassWord);
        if (!res.Succeeded) throw new InternalServerErrorException("Failed to SignUp, try again later");
        await userManager.AddToRoleAsync(user, nameof(AppRoles.Member));
        return new AuthResponse
        {
            ExpireTime = DateTime.UtcNow.AddDays(7),
            Token = await GenerateJwtToken(user),
        };
    }

    public async Task<string> SignInAsync(AppUserSignInDto appUserDto)
    {
        var user = await userManager.FindByEmailAsync(appUserDto.EmailOrUserName)
                   ?? await userManager.FindByNameAsync(appUserDto.EmailOrUserName);
        if (user is null)
            throw new InvalidLoginCredentialsException();
        var result = await userManager.CheckPasswordAsync(user, appUserDto.PassWord);
        if (!result)
            throw new InvalidLoginCredentialsException();
        return await GenerateJwtToken(user);
    }

    public async Task<bool> DeleteAsync(string userId)
    {
        //TODO:FIX TS METHOD AFTER DEALING WITH ALL RELATED STUFF (FOLLOWERS,FOLLOWING,COMMENTS AND ETC.) OR PMO
        var user = await appUserRepository.GetByIdAsync(userId);
        if (user == null)
            throw new UserNotFoundException();
        if (!string.IsNullOrWhiteSpace(user.ProfilePictureUri))
            await blobService.DeleteBlobAsync(user.ProfilePictureUri);
        var res = await userManager.DeleteAsync(user);
        if (res.Succeeded) return true;
        throw new InternalServerErrorException("Failed to delete user");
    }

    public async Task<AppUserDto?> GetById(
        string id,
        Func<IQueryable<AppUser>, IIncludableQueryable<AppUser, object>>? include = null,
        bool tracking = true)
    {
        var user = await appUserRepository.GetByIdAsync(id, i => i
                           .Include(m => m.Followers)
                           .Include(m => m.Following),
                       tracking)
                   ?? throw new UserNotFoundException();

        var userPosts = await postService.FindAsync(p => p.UserId == user.Id && p.Status == "Published",
            include: query => query
                .Include(m => m.Tags)
                .Include(m => m.LikedBy)
                .Include(m => m.Comments)
                .Include(m => m.SavedBy),
            false, user.Id);

        var savedPosts = await postSaveRepository.GetAllSavedPostsByUserAsync(user);
        var orderedSavedPosts = savedPosts.OrderByDescending(p => p.CreatedAt).ToList();
        var orderedPosts = userPosts.OrderByDescending(p => p!.PostedWhen).ToList();
        var profilePicUri = !string.IsNullOrWhiteSpace(user.ProfilePictureUri)
            ? blobService.GetBlobSasUri(user.ProfilePictureUri)
            : null;

        var orderedSavedPostsDto = orderedSavedPosts.Select(post => post.MapToPostDto(
            blobService.GetBlobSasUri(post.Media), user.UserName!,
            post.Tags.Select(m => m.Name).ToList(),
            post.LikedBy.FirstOrDefault(m => m.UserId == user.Id) is not null,
            true, profilePicUri)).ToList();

        return user.MapToAppUserDto(profilePicUri, orderedPosts, orderedSavedPostsDto);
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
            .Include(au => au.Followers)
            .Include(au => au.Following);

        var users = await appUserRepository.GetAllAsync(page, pageSize, tracking, includeFunc);
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
            if (!string.IsNullOrWhiteSpace(user.ProfilePictureUri))
            {
                var uri = blobService.GetBlobSasUri(user.ProfilePictureUri);
                rerDtos.Add(user.MapToAppUserDto(uri));
            }
            else rerDtos.Add(user.MapToAppUserDto());
        }

        return rerDtos;
    }


    public async Task<IEnumerable<AppUserDto?>> Find(Expression<Func<AppUser, bool>> predicate,
        Func<IQueryable<AppUser>, IIncludableQueryable<AppUser, object>>? include = null, bool tracking = true)
    {
        var data = await appUserRepository.FindAsync(predicate, include, tracking);
        var returnDtos = new List<AppUserDto>();
        if (!data.Any()) return returnDtos;
        foreach (var appUser in data)
        {
            if (!string.IsNullOrWhiteSpace(appUser.ProfilePictureUri))
            {
                var uri = blobService.GetBlobSasUri(appUser.ProfilePictureUri);
                returnDtos.Add(appUser.MapToAppUserDto(uri));
            }
            else
            {
                returnDtos.Add(appUser.MapToAppUserDto());
            }
        }

        return returnDtos;
    }

    public async Task<IEnumerable<AppUserContextDto?>> SearchAsync(string? searchText)
    {
        var data = await appUserRepository.FindAsync(m => searchText != null && m.UserName!.Contains(searchText),
            tracking: false);
        return data.Select(m =>
            m?.MapToAppUserContextDto(m.ProfilePictureUri is not null
                ? blobService.GetBlobSasUri(m.ProfilePictureUri)
                : null));
    }

    public async Task ChangePrivacy(string userId)
    {
        var user = await appUserRepository.GetByIdAsync(userId);
        if (user == null)
            throw new UserNotFoundException();
        user.IsPrivate = !user.IsPrivate;
        await appUserRepository.UpdateAsync(user);
    }

    public async Task<bool> VerifyPassword(string userId, string password)
    {
        var user = await userManager.FindByIdAsync(userId);
        if (user is null)
            throw new UserNotFoundException();
        var result = await userManager.CheckPasswordAsync(user, password);
        return result;
    }

    public async Task<bool> ChangePassword(string userId, PasswordChangeRequest request)
    {
        var user = await userManager.FindByIdAsync(userId);
        if (user is null)
            throw new UserNotFoundException();
        var result = await userManager.ChangePasswordAsync(user, request.OldPassword, request.NewPassword);
        if (result.Succeeded) return true;
        throw new BadRequestException("Failed to change password, please try again later");
    }

    private async Task<string> GenerateJwtToken(AppUser user)
    {
        var roles = await userManager.GetRolesAsync(user);

        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id),
            new Claim(JwtRegisteredClaimNames.Email, user.Email!),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(ClaimTypes.NameIdentifier, user.Id)
        };
        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));


        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:Key"]));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddHours(3),
            SigningCredentials = creds,
            Issuer = config["Jwt:Issuer"],
            Audience = config["Jwt:Audience"]
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);
        var jwt = tokenHandler.WriteToken(token);

        return jwt;
    }
}