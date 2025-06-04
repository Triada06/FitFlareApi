using FitFlare.Application.DTOs.Comment;
using FitFlare.Application.Helpers.Exceptions;
using FitFlare.Application.Mappings;
using FitFlare.Application.Services.Interfaces;
using FitFlare.Core.Entities;
using FitFlare.Infrastructure.Repositories.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;

namespace FitFlare.Application.Services;

public class CommentService(
    ICommentRepository commentRepository,
    UserManager<AppUser> userManager,
    ICommentLikeRepository commentLikeRepository,
    IBlobService blobService) : ICommentService
{
    public async Task<CommentDto> CreateAsync(CommentCreateDto commentDto, string userId)
    {
        var user = await userManager.FindByIdAsync(userId);
        if (user == null)
            throw new UserNotFoundException();
        var comment = commentDto.MapToComment(userId);
        await commentRepository.CreateAsync(comment);
        return comment.MapToCommentDto(user.ProfilePictureUri is not null
            ? blobService.GetBlobSasUri(user.ProfilePictureUri)
            : null, userId);
    }

    public async Task<CommentDto> AddReply(CommentReplyCreateDto commentReplyDto, string userId, string parentCommentId)
    {
        var user = await userManager.FindByIdAsync(userId);
        if (user is null)
            throw new UserNotFoundException();
        var reply = commentReplyDto.MapToReply(userId, parentCommentId);
        await commentRepository.CreateAsync(reply);
        return reply.MapToCommentDto(user.ProfilePictureUri is not null
            ? blobService.GetBlobSasUri(user.ProfilePictureUri)
            : null, userId);
    }

    public async Task DeleteAsync(string commentId)
    {
        var comment = await commentRepository.GetByIdAsync(commentId);
        if (comment is null)
            throw new NotFoundException("Comment not found");
        if (!await commentRepository.DeleteAsync(comment))
            throw new InternalServerErrorException("Comment deleted");
    }

    public async Task<CommentDto?> GetById(string id,string userId,
        Func<IQueryable<Comment>, IIncludableQueryable<Comment, object>>? include = null, bool tracking = true)
    {
        var data = await commentRepository.GetByIdAsync(id, i => i
            .Include(m => m.Post)
            .Include(m => m.Likes)
            .Include(m => m.User)
            .Include(m => m.Replies), tracking);
        return data?.MapToCommentDto(data.User.ProfilePictureUri is not null
            ? blobService.GetBlobSasUri(data.User.ProfilePictureUri)
            : null,userId);
    }

    public async Task<IEnumerable<CommentDto>> GetAllByPostId(string postId,string userId, int page,
        int pageSize = 20, bool tracking = true)
    {
        var data = await commentRepository.GetAllByPostId(postId, page, pageSize, tracking);
        return data.Select(m =>
            m.MapToCommentDto(m.User.ProfilePictureUri is not null
                ? blobService.GetBlobSasUri(m.User.ProfilePictureUri)
                : null,userId));
    }

    public async Task<IEnumerable<CommentDto>> LoadReplies(string postId,string userId, string parentCommentId, int page,
        int pageSize = 20, bool tracking = true)
    {
        var data = await commentRepository.LoadReplies(postId, parentCommentId, page, pageSize, tracking);
        return data.Select(m => m.MapToCommentDto(m.User.ProfilePictureUri is not null
            ? blobService.GetBlobSasUri(m.User.ProfilePictureUri)
            : null,userId));
    }

    public async Task LikeComment(string commentId, string userId)
    {
        var user = await userManager.FindByIdAsync(userId);
        if (user is null)
            throw new UserNotFoundException();
        var comment = await commentRepository.GetByIdAsync(commentId);
        if (comment is null)
            throw new NotFoundException("Comment not found");

        if (await commentLikeRepository.ExistsAsync(commentId, userId))
            throw new BadRequestException("Comment already liked");
        await commentLikeRepository.AddAsync(new CommentLike { CommentId = commentId, UserId = userId });
    }

    public async Task UnlikeComment(string commentId, string userId)
    {
        var user = await userManager.FindByIdAsync(userId);
        if (user is null)
            throw new UserNotFoundException();
        var comment = await commentRepository.GetByIdAsync(commentId);
        if (comment is null)
            throw new NotFoundException("Comment not found");
        var commentLike = await commentLikeRepository.GetAsync(commentId, userId);
        if (commentLike is null)
            throw new BadRequestException("Comment already unliked");
        await commentLikeRepository.RemoveAsync(commentLike);
    }
}