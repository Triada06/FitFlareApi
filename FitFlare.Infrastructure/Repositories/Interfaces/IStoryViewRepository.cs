using FitFlare.Core.Entities;

namespace FitFlare.Infrastructure.Repositories.Interfaces;

public interface IStoryViewRepository
{
    public Task CreateAsync(StoryView storyView);
    public Task DeleteAsync(StoryView storyView);
    public Task<List<AppUser>> GetStoryViewers(string storyId, bool tracking = false);
    public Task<bool> AnyAsync(string storyId, string userId);
}