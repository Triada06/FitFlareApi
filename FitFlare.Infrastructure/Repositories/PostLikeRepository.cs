using FitFlare.Core.Entities;
using FitFlare.Infrastructure.Data;
using FitFlare.Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FitFlare.Infrastructure.Repositories;

public class PostLikeRepository(AppDbContext context) : IPostLikeRepository
{
    public async Task<bool> ExistsAsync(string postId, string userId)
    { 
        var data = await context.PostLikes.FirstOrDefaultAsync(x => x.PostId == postId && x.UserId == userId);
        return data != null;
    }

    public async Task AddAsync(PostLike like)
    {
        await context.PostLikes.AddAsync(like);
        await context.SaveChangesAsync();
    }

    public async Task RemoveAsync(PostLike like)
    {
         context.PostLikes.Remove(like);
        await context.SaveChangesAsync();
    }

    public async Task<PostLike?> GetAsync(string postId, string userId)
    {
      var data = await context.PostLikes.FirstOrDefaultAsync(x => x.PostId == postId && x.UserId == userId);
      return data;
    }
}