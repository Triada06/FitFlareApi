using FitFlare.Application.DTOs.AppUser;
using FitFlare.Application.DTOs.Story;
using FitFlare.Application.Helpers.Exceptions;
using FitFlare.Application.Mappings;
using FitFlare.Application.Services.Interfaces;
using FitFlare.Core.Entities;
using FitFlare.Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FitFlare.Application.Services;

public class StoryService(
    IAppUserRepository userRepository,
    IBlobService blobService,
    IStoryRepository storyRepository,
    IFollowRepository followRepository,
    IStoryViewRepository viewRepository)
    : IStoryService
{
    public async Task<List<StoryDto>> GetAllStoriesByUserId(string userId)
    {
        var user = await userRepository.GetByIdAsync(userId);
        if (user == null)
            throw new UserNotFoundException();
        var data = await storyRepository.FindAsync(m => m.UserId == userId);
        if (!data.Any())
            return [];
        return data.Select(m => m!.MapToStoryDto(blobService.GetBlobSasUri(m!.MediaUri),
            user.ProfilePictureUri is not null ? blobService.GetBlobSasUri(user.ProfilePictureUri) : null)).ToList();
    }

    public async Task<StoryDto?> GetStoryById(string storyId)
    {
        var story = await storyRepository.GetByIdAsync(storyId, i => i.Include(m => m.User));
        if (story == null)
            throw new NotFoundException("Story not found");

        return story.MapToStoryDto(blobService.GetBlobSasUri(story.MediaUri),
            story.User.ProfilePictureUri is not null ? blobService.GetBlobSasUri(story.User.ProfilePictureUri) : null);
    }

    public async Task<StoryDto> UploadStory(StoryCreateDto story, string userId)
    {
        var user = await userRepository.GetByIdAsync(userId, tracking: true);
        if (user == null)
            throw new UserNotFoundException();

        var newStory = new Story
        {
            MediaUri = story.Media.FileName + Guid.NewGuid(),
            MediaType = GetMediaType(story.Media.ContentType),
            UserId = userId,
        };
        if (await storyRepository.CreateAsync(newStory))
            await blobService.UploadBlobAsync(story.Media, newStory.MediaUri);
        return newStory.MapToStoryDto(blobService.GetBlobSasUri(newStory.MediaUri),
            user.ProfilePictureUri is not null ? blobService.GetBlobSasUri(user.ProfilePictureUri) : null);
    }

    public async Task DeleteStory(string storyId)
    {
        var story = await storyRepository.GetByIdAsync(storyId, tracking: true);
        if (story == null)
            throw new NotFoundException("Story not found");
        if (await storyRepository.DeleteAsync(story))
            await blobService.DeleteBlobAsync(story.MediaUri);
    }

    public async Task<IEnumerable<AppUserContextDto>> GetStoryViewers(string storyId)
    {
        var story = await storyRepository.GetByIdAsync(storyId, tracking: false);
        if (story == null)
            throw new NotFoundException("Story not found");
        var data = await viewRepository.GetStoryViewers(storyId);
        if (data.Count == 0)
            return [];
        return data.Select(m =>
            m!.MapToAppUserContextDto(m.ProfilePictureUri is not null
                ? blobService.GetBlobSasUri(m.ProfilePictureUri)
                : null));
    }

    public async Task<IEnumerable<StoryComponentDto>> GetUserFollowingStories(string userId)
    {
        var user = await userRepository.GetByIdAsync(userId, tracking: true);
        if (user == null)
            throw new UserNotFoundException();
        var followingUserIds = await followRepository.FindAsync(m => m.FollowerId == userId);

        if (!followingUserIds.Any())
            return [];
        var storyComponents = new List<StoryComponentDto>();

        foreach (var followingUserId in followingUserIds)
        {
            var stories = await GetAllStoriesByUserId(followingUserId!.FollowingId);
            storyComponents.Add(new StoryComponentDto
            {
                Stories = stories
            });
        }
        return storyComponents;
    }

    public async Task ViewStory(string storyId, string userId)
    {
        var story = await storyRepository.GetByIdAsync(storyId, tracking: false);
        if (story == null)
            throw new NotFoundException("Story not found");
        var user = await userRepository.GetByIdAsync(userId, tracking: false);
        if (user == null)
            throw new UserNotFoundException();
        await viewRepository.CreateAsync(new StoryView
        {
            StoryId = storyId,
            UserId = userId
        });
    }

    private static string GetMediaType(string contentType)
    {
        var mediaType = contentType.ToLower();
        string result;
        if (mediaType.StartsWith("image/"))
            result = "image";
        else if (mediaType.StartsWith("video/"))
            result = "video";
        else throw new ContentTypeException();

        return result;
    }
}