using FitFlare.Application.DTOs.Comment;
using FitFlare.Core.Entities;

namespace FitFlare.Application.Mappings;

public static class CommentMapping
{
    public static Comment MapToComment(this CommentCreateDto commentDto, string userId)
    {
        return new Comment
        {
            Content = commentDto.Content,
            UserId = userId,
            PostId = commentDto.PostId,
        };
    }

    public static CommentDto MapToCommentDto(this Comment comment, string? commentatorProfilePictureSasUri)
    {
        return new CommentDto
        {
            Id = comment.Id,
            CommenterId = comment.UserId,
            CommenterName = comment.User.UserName!,
            Content = comment.Content,
            CommentedWhen = comment.CreatedAt,
            CommenterProfilePicture = commentatorProfilePictureSasUri,
            CommentLikeCount = comment.Likes.Count,
            ReplyCount = comment.Replies.Count
        };
    }
}