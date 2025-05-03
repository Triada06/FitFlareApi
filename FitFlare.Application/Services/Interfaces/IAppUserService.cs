using System.Linq.Expressions;
using FitFlare.Application.DTOs.AppUserDTos;
using FitFlare.Core.Entities;
using Microsoft.EntityFrameworkCore.Query;

namespace FitFlare.Application.Services.Interfaces;

public interface IAppUserService
{
    public Task<bool> UpdateAsync(AppUserUpdateDto appUser, string userId);
    public Task<AppUserDto> CreateAsync(AppUserCreateDto appUser);
    public Task<bool> DeleteAsync(string userId);

    public Task<AppUserDto?> GetById(string id, Func<IQueryable<AppUser>,
        IIncludableQueryable<AppUser, object>>? include = null, bool tracking = true);

    public Task<IEnumerable<AppUserDto>> GetAll(int page, string? sort, int pageSize = 5, bool tracking = true,
        string? searchText = null);

    public Task<IEnumerable<AppUserDto?>> Find(Expression<Func<AppUser, bool>> predicate, Func<IQueryable<AppUser>,
        IIncludableQueryable<AppUser, object>>? include = null, bool tracking = true);
}