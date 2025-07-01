using System.Security.Cryptography;
using System.Text;
using FitFlare.Application.Contracts.Requests;
using FitFlare.Application.DTOs.Admin.Admins;
using FitFlare.Application.Helpers.Exceptions;
using FitFlare.Application.Mappings;
using FitFlare.Application.Services.Interfaces;
using FitFlare.Core;
using FitFlare.Core.Entities;
using FitFlare.Infrastructure.Repositories.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;

namespace FitFlare.Application.Services;

public class AdminService(
    UserManager<AppUser> userManager,
    RoleManager<IdentityRole> roleManager,
    IAppUserRepository userRepository,
    IEmailService emailService,
    IConfiguration configuration,
    IBlobService blobService)
    : IAdminsService
{
    public async Task<IEnumerable<AdminDto>> GetAllAdminsAsync()
    {
        var data = await userManager.GetUsersInRoleAsync(nameof(AppRoles.Admin));
        return data.Select(m =>
            m.MapToAdminDto(m.ProfilePictureUri is not null ? blobService.GetBlobSasUri(m.ProfilePictureUri) : null));
    }

    public async Task MakeAppOwnerAsync(string id)
    {
        var user = await userManager.FindByIdAsync(id);
        if (user == null || !await userManager.IsInRoleAsync(user, "Admin"))
            throw new BadRequestException("Invalid target admin");

        var token = GenerateOwnerTransferToken(user.Id);
        var confirmLink = $"https://localhost:5173/confirm-owner?token={Uri.EscapeDataString(token)}";

        var message = $@"<p>You’ve been selected to become the new app owner.</p>
                     <p><a href='{confirmLink}'>Click here to confirm</a></p>";

        await emailService.SendAsync(user.Email!, "Confirm Ownership Transfer", message);
    }

    public async Task PromoteToAdminAsync(string id)
    {
        var user = await userManager.FindByIdAsync(id);
        if (user == null)
            throw new UserNotFoundException();

        var isInRole = await userManager.IsInRoleAsync(user, nameof(AppRoles.Admin));
        if (!isInRole)
        {
            var result = await userManager.AddToRoleAsync(user, nameof(AppRoles.Admin));
            if (!result.Succeeded)
                throw new Exception("Failed to promote to admin");
        }
    }


    public async Task RemoveAdminAsync(string id)
    {
        var user = await userManager.FindByIdAsync(id);
        if (user == null)
            throw new UserNotFoundException();

        var isInRole = await userManager.IsInRoleAsync(user, nameof(AppRoles.Admin));
        if (isInRole)
        {
            var result = await userManager.RemoveFromRoleAsync(user, nameof(AppRoles.Admin));
            if (!result.Succeeded)
                throw new Exception("Failed to remove admin role");
        }
    }

    public async Task<AdminDto> FindByEmail(string email)
    {
        var data = await userManager.FindByEmailAsync(email);
        if (data is null)
            throw new UserNotFoundException();
        return data.MapToAdminDto(data.ProfilePictureUri is not null
            ? blobService.GetBlobSasUri(data.ProfilePictureUri)
            : null);
    }

    private string GenerateOwnerTransferToken(string userId)
    {
        var secretKey = configuration["Jwt:Key"]; // store in appsettings
        var timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        var raw = $"{userId}:{timestamp}";

        var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secretKey));
        var signature = Convert.ToBase64String(hmac.ComputeHash(Encoding.UTF8.GetBytes(raw)));

        var tokenPayload = Convert.ToBase64String(Encoding.UTF8.GetBytes(raw));
        return $"{tokenPayload}.{signature}";
    }

    public async Task ConfirmOwnerTransferAsync(string token)
    {
        if (!TryValidateToken(token, out var newOwnerId))
            throw new BadRequestException("Invalid or expired token");

        var newOwner = await userManager.FindByIdAsync(newOwnerId);
        if (newOwner == null || !await userManager.IsInRoleAsync(newOwner, "Admin"))
            throw new BadRequestException("Invalid target admin");

        var allUsers = userManager.Users.ToList();
        var currentOwner = allUsers.FirstOrDefault(u =>
            userManager.IsInRoleAsync(u, "Owner").Result);

        if (currentOwner != null)
        {
            await userManager.RemoveFromRoleAsync(currentOwner, "Owner");
            await userManager.AddToRoleAsync(currentOwner, "Admin");
        }

        await userManager.RemoveFromRoleAsync(newOwner, "Admin");
        await userManager.AddToRoleAsync(newOwner, "Owner");
    }


    private bool TryValidateToken(string token, out string userId)
    {
        userId = null;

        var secretKey = configuration["Jwt:Key"];
        var parts = token.Split('.');
        if (parts.Length != 2) return false;

        var payload = parts[0];
        var signature = parts[1];

        var raw = Encoding.UTF8.GetString(Convert.FromBase64String(payload));
        var expectedSig = Convert.ToBase64String(
            new HMACSHA256(Encoding.UTF8.GetBytes(secretKey))
                .ComputeHash(Encoding.UTF8.GetBytes(raw))
        );

        if (expectedSig != signature) return false;

        var pieces = raw.Split(':');
        if (pieces.Length != 2) return false;

        var timestamp = long.Parse(pieces[1]);
        var now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

        // expire after 10 mins (optional)
        if (now - timestamp > 600) return false;

        userId = pieces[0];
        return true;
    }
}