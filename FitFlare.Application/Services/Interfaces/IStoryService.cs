using FitFlare.Application.DTOs.AppUser;
using FitFlare.Application.DTOs.Story;

namespace FitFlare.Application.Services.Interfaces;

public interface IStoryService
{
    public Task<List<StoryDto>> GetAllStoriesByUserId(string userId);
    public Task<StoryDto?> GetStoryById(string storyId);
    public Task<StoryDto>   UploadStory(StoryCreateDto story, string userId);
    public Task DeleteStory(string storyId);
    
    public Task<IEnumerable<AppUserContextDto>> GetStoryViewers(string storyId);
    public Task<IEnumerable<StoryComponentDto>> GetUserFollowingStories(string userId);
    public Task ViewStory(string storyId, string userId);
}