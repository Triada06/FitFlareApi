using FitFlare.Application.DTOs.Story;
using FitFlare.Core.Entities;

namespace FitFlare.Application.Mappings;

public static class StoryMappings
{
    public static StoryDto MapToStoryDto(this Story story, string mediaSasUrl, string? authorProfilePicture)
    {
        return new StoryDto
        {
            Id = story.Id,
            AuthorId = story.UserId,
            AuthorProfilePicture = authorProfilePicture,
            MediaType = story.MediaType,
            MediaSasUrl = mediaSasUrl,
            PostedTime = story.CreatedAt
        };
    }
}