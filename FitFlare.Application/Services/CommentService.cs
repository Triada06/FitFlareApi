using FitFlare.Application.DTOs.Comment;
using FitFlare.Application.Helpers.Exceptions;
using FitFlare.Application.Mappings;
using FitFlare.Application.Services.Interfaces;
using FitFlare.Core.Entities;
using FitFlare.Infrastructure.Repositories.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Query;

namespace FitFlare.Application.Services;

public class CommentService(
    ICommentRepository commentRepository,
    UserManager<AppUser> userManager,
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
            : null);
    }

    public Task DeleteAsync(string commentId)
    {
        throw new NotImplementedException();
    }

    public Task<CommentDto?> GetById(string id,
        Func<IQueryable<Comment>, IIncludableQueryable<Comment, object>>? include = null, bool tracking = true)
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<CommentDto>> GetAllByPostId(string postId, int page,
        int pageSize = 20, bool tracking = true)
    {
        var data = await commentRepository.GetAllByPostId(postId, page, pageSize, tracking);
        return data.Select(m =>
            m.MapToCommentDto(m.User.ProfilePictureUri is not null
                ? blobService.GetBlobSasUri(m.User.ProfilePictureUri)
                : null));
    }
}