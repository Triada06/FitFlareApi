using System.Linq.Expressions;
using FitFlare.Application.DTOs.Posts;
using FitFlare.Core.Entities;
using Microsoft.EntityFrameworkCore.Query;

namespace FitFlare.Application.Services.Interfaces;

public interface IPostService
{
    public Task<bool> UpdateAsync(PostUpdateDto post, string postId, string userId);
    public Task<PostDto> CreateAsync(PostCreateDto post);
    public Task<bool> DeleteAsync(string postId, string userId);

    public Task<PostDto?> GetByIdAsync(string id,string userId, Func<IQueryable<Post>,
        IIncludableQueryable<AppUser, object>>? include = null, bool tracking = true);

    public Task<IEnumerable<PostDto>> GetAll(string userId,int page, string? sort, int pageSize = 5, bool tracking = true);

    public Task<IEnumerable<PostDto?>> GetByTagAsync(string tagId,string userId);
    public Task<IEnumerable<PostDto?>> FindAsync(Expression<Func<Post, bool>> predicate, Func<IQueryable<Post>,
        IIncludableQueryable<Post, object>>? include = null, bool tracking = true, string userIdForLikeChecking = "");
    public Task<string> AiAnalyse(PostAnalyseDto dto);
    public string GetMediaType(string contentType);
    public Task LikePost(string postId, string userId);
    public Task UnlikePost(string postId, string userId);
    public Task SavePost (string postId, string userId);
    public Task UnSavePost(string postId, string userId);
    public Task DeleteDraftedMediaAsync();
}