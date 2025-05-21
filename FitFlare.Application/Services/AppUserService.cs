using System.IdentityModel.Tokens.Jwt;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Text;
using FitFlare.Application.Contracts.Responses;
using FitFlare.Application.DTOs.AppUserDTos;
using FitFlare.Application.Helpers.Exceptions;
using FitFlare.Application.Mappings;
using FitFlare.Application.Services.Interfaces;
using FitFlare.Core;
using FitFlare.Core.Entities;
using FitFlare.Infrastructure.Repositories.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace FitFlare.Application.Services;

public class AppUserService(
    IAppUserRepository repository,
    IBlobService blobService,
    UserManager<AppUser> userManager,
    IPostService postService,
    IConfiguration config)
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
            if (user.ProfilePictureUri != null)
                await blobService.DeleteBlobAsync(user.ProfilePictureUri);

            await blobService.UploadBlobAsync(appUser.ProfilePicture, user.ProfilePictureUri!);
            await repository.UpdateAsync(user);
            return true;
        }

        appUser.MapToAppUser(user);
        return await repository.UpdateAsync(user);
    }

    public async Task<AuthResponse> SignUpAsync(AppUserSignUpDto appUser)
    {
        var template = appUser.UserName.Trim();
        var isExists = await repository.AnyAsync(u =>
            u.UserName != null && u.UserName.Trim() == template);
        if (isExists)
            throw new UserAlreadyExistsException();

        var user = appUser.MapToAppUser();
        var res = await userManager.CreateAsync(user,appUser.PassWord);
        if (!res.Succeeded) throw new InternalServerErrorException();
        await userManager.AddToRoleAsync(user, nameof(AppRoles.Member)); 
        return new AuthResponse
        {
            ExpireTime = DateTime.UtcNow.AddDays(7),
            Token = await GenerateJwtToken(user),
            UserDto = user.MapToAppUserDto()
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
        var user = await repository.GetByIdAsync(userId);
        if (user == null)
            throw new UserNotFoundException();
        if (!string.IsNullOrWhiteSpace(user.ProfilePictureUri))
            await blobService.DeleteBlobAsync(user.ProfilePictureUri);
        return await repository.DeleteAsync(user);
    }

    public async Task<AppUserDto?> GetById(string id,
        Func<IQueryable<AppUser>, IIncludableQueryable<AppUser, object>>? include = null, bool tracking = true)
    {
        var user = await repository.GetByIdAsync(id, include, tracking);
        if (user == null)
            throw new UserNotFoundException();

        if (!string.IsNullOrWhiteSpace(user.ProfilePictureUri))
        {
            string profilePictureUri = blobService.GetBlobSasUri(user.ProfilePictureUri);
            var userPosts = await postService.FindAsync(p=>p.UserId == user.Id,null,false);
            return user.MapToAppUserDto(profilePictureUri,userPosts);
        }

        var posts = await postService.FindAsync(p=>p.UserId == user.Id,null,false);
        return user.MapToAppUserDto(null,posts);
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
            .Include(au => au.Comments);

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
            if (!string.IsNullOrWhiteSpace(user.ProfilePictureUri))
            {
                var uri = blobService.GetBlobSasUri(user.ProfilePictureUri);
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

    private async Task<string> GenerateJwtToken(AppUser user)
    {
        var roles = await userManager.GetRolesAsync(user);

        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
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