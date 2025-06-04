using FitFlare.Core.Entities;
using FitFlare.Infrastructure.Data;
using FitFlare.Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FitFlare.Infrastructure.Repositories;

public class CommentLikeRepository(AppDbContext context) : ICommentLikeRepository
{
    public async Task<bool> ExistsAsync(string commentId, string userId) =>
        await context.CommentLikes.AnyAsync(l => l.CommentId == commentId && l.UserId == userId);


    public async Task AddAsync(CommentLike like)
    {
        await context.CommentLikes.AddAsync(like);
        await context.SaveChangesAsync();
    }

    public async Task RemoveAsync(CommentLike like)
    {
        context.CommentLikes.Remove(like);
        await context.SaveChangesAsync();
    }

    public async Task<CommentLike?> GetAsync(string commentId, string userId) =>
        await context.CommentLikes
            .FirstOrDefaultAsync(x => x.CommentId == commentId && x.UserId == userId);
}