using FitFlare.Application.DTOs.AppUser;
using FitFlare.Core.Entities;

namespace FitFlare.Application.Services.Interfaces;

public interface IFollowService
{
    public Task<IEnumerable<AppUserContextDto>?> GetFollowersByIdFollowingId(string followingId);
    public Task<IEnumerable<AppUserContextDto>?> GetFollowingsByIdFollowerId(string followerId);
    public Task Follow(string followerId, string followingId);
    public Task UnFollow(string followerId, string followingId);
}