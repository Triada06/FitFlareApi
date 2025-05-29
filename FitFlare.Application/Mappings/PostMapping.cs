using FitFlare.Application.DTOs.Posts;
using FitFlare.Core.Entities;

namespace FitFlare.Application.Mappings;

public static class PostMapping
{
    public static PostDto MapToPostDto(this Post post, string mediaUri, List<string>? tags, bool isLikedByUser, bool isSavedByUser)
    {
        return new PostDto
        {
            PostedById = post.UserId,
            CommentCount = post.Comments.Count,
            Description = post.Description,
            LikeCount = post.LikeCount,
            MediaType = post.MediaType,
            Id = post.Id,
            MediaUri = mediaUri,
            PostedWhen = post.CreatedAt,
            HashTags = tags,
            IsLikedByUser = isLikedByUser,
            IsSavedByUSer = isSavedByUser,
        };
    }
}