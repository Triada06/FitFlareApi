using FitFlare.Core.Entities;

namespace FitFlare.Infrastructure.Repositories.Interfaces;

public interface IPostLikeRepository
{
    Task<bool> ExistsAsync(string postId, string userId);
    Task AddAsync(PostLike like);
    Task RemoveAsync(PostLike like);
    Task<PostLike?> GetAsync(string postId, string userId);
}