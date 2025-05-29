using FitFlare.Core.Entities;

namespace FitFlare.Infrastructure.Repositories.Interfaces;

public interface IPostSaveRepository
{
    public Task<bool> ExistsAsync(string postId, string userId);
    public Task<List<Post>> GetAllSavedPostsByUserAsync(AppUser user);
    public Task SaveAsync(PostSave postSave);
    public Task UnSaveAsync(PostSave postSave);
    public Task<PostSave?> GetByIdAsync(string userId, string postId);
}