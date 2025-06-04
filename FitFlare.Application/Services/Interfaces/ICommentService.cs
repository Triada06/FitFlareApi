using FitFlare.Application.DTOs.Comment;
using FitFlare.Core.Entities;
using Microsoft.EntityFrameworkCore.Query;

namespace FitFlare.Application.Services.Interfaces;

public interface ICommentService
{
    public Task<CommentDto> CreateAsync(CommentCreateDto commentDto, string userId);
    public Task<CommentDto> AddReply(CommentReplyCreateDto commentReplyDto, string userId, string parentCommentId);
    public Task DeleteAsync(string commentId);

    public Task<CommentDto?> GetById(string id,string userId, Func<IQueryable<Comment>,
        IIncludableQueryable<Comment, object>>? include = null, bool tracking = true);

    public Task<IEnumerable<CommentDto>> GetAllByPostId(string postId,string userId, int page, int pageSize = 20,
        bool tracking = true);

    public Task<IEnumerable<CommentDto>> LoadReplies(string postId,string userId, string parentCommentId, int page, int pageSize = 20,
        bool tracking = true);
    
    public Task LikeComment(string commentId, string userId);
    public Task UnlikeComment(string commentId, string userId);
}