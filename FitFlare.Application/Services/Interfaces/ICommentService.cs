using FitFlare.Application.DTOs.Comment;
using FitFlare.Core.Entities;
using Microsoft.EntityFrameworkCore.Query;

namespace FitFlare.Application.Services.Interfaces;

public interface ICommentService
{
    public Task<CommentDto> CreateAsync(CommentCreateDto commentDto, string userId);
    public Task DeleteAsync(string commentId);

    public Task<CommentDto?> GetById(string id, Func<IQueryable<Comment>,
        IIncludableQueryable<Comment, object>>? include = null, bool tracking = true);

    public Task<IEnumerable<CommentDto>> GetAllByPostId(string postId,int page, int pageSize = 20, bool tracking = true);
}