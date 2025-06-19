using System.Linq.Expressions;
using FitFlare.Application.Contracts.Requests;
using FitFlare.Application.Contracts.Responses;
using FitFlare.Application.DTOs.Account;
using FitFlare.Application.DTOs.AppUser;
using FitFlare.Core.Entities;
using Microsoft.EntityFrameworkCore.Query;

namespace FitFlare.Application.Services.Interfaces;

public interface IAppUserService
{
    public Task UpdateAsync(AppUserUpdateDto appUser, string userId);
    public Task<AuthResponse> SignUpAsync(AppUserSignUpDto appUser);
    public Task<string> SignInAsync(AppUserSignInDto appUserDto);
    public Task<bool> DeleteAsync(string userId);

    public Task<AppUserDto?> GetById(string id, Func<IQueryable<AppUser>,
        IIncludableQueryable<AppUser, object>>? include = null, bool tracking = true);
    public Task<IEnumerable<AppUserDto>> GetAll(int page, string? sort, int pageSize = 5, bool tracking = true,
        string? searchText = null);
    public Task<IEnumerable<AppUserDto?>> Find(Expression<Func<AppUser, bool>> predicate, Func<IQueryable<AppUser>,
        IIncludableQueryable<AppUser, object>>? include = null, bool tracking = true);
    public Task<IEnumerable<AppUserContextDto?>> SearchAsync(string? searchText);
    public Task ChangePrivacy(string userId);
    public Task<bool> VerifyPassword(string userId, string password);
    public Task<bool> ChangePassword(string userId,PasswordChangeRequest request);
    public Task<bool> ConfirmEmail(string userId, string token);
    public Task<bool> ResetPassword(ResetPasswordDto request);
    public Task ForgotPassword(ForgotPasswordDto forgotPassword);
}