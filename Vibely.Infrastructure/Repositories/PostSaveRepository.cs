using FitFlare.Core.Entities;
using FitFlare.Infrastructure.Data;
using FitFlare.Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FitFlare.Infrastructure.Repositories;

public class PostSaveRepository(AppDbContext context) : IPostSaveRepository
{
    public async Task<bool> ExistsAsync(string postId, string userId)
    {
        var data = await context.PostSaves.FirstOrDefaultAsync(x => x.PostId == postId && x.UserId == userId);
        return data != null;
    }

    public async Task<List<Post>> GetAllSavedPostsByUserAsync(AppUser user)
    {
        var posts = await context.PostSaves
            .AsNoTracking()
            .Where(m => m.User == user && m.Post.Status == "Published")
            .Select(m => new Post
            {
                Id = m.Post.Id,
                Media = m.Post.Media,
                Description = m.Post.Description,
                LikeCount = m.Post.LikedBy.Count,
                Tags = m.Post.Tags,
                LikedBy = m.Post.LikedBy,
                MediaType = m.Post.MediaType,
                Status = m.Post.Status,
                CreatedAt = m.Post.CreatedAt,
                Comments = m.Post.Comments,
                SavedBy = m.Post.SavedBy,
                SaveCount = m.Post.SaveCount,
                UserId = m.User.Id
            })
            .ToListAsync();

        return posts;
    }


    public async Task SaveAsync(PostSave postSave)
    {
        await context.PostSaves.AddAsync(postSave);
        await context.SaveChangesAsync();
    }

    public async Task UnSaveAsync(PostSave postSave)
    {
        context.PostSaves.Remove(postSave);
        await context.SaveChangesAsync();
    }

    public async Task<PostSave?> GetByIdAsync(string userId, string postId) =>
        await context.PostSaves.FirstOrDefaultAsync(x => x.PostId == postId && x.UserId == userId);
}