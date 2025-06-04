using FitFlare.Core.Entities;

namespace FitFlare.Infrastructure.Repositories.Interfaces;

public interface ICommentLikeRepository
{
    Task<bool> ExistsAsync(string commentId, string userId);
    Task AddAsync(CommentLike like);
    Task RemoveAsync(CommentLike like);
    Task<CommentLike?> GetAsync(string commentId, string userId);
}