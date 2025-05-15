using System.Linq.Expressions;
using FitFlare.Application.DTOs.Posts;
using FitFlare.Core.Entities;
using Microsoft.EntityFrameworkCore.Query;

namespace FitFlare.Application.Services.Interfaces;

public interface IPostService
{
    public Task<bool> UpdateAsync(PostUpdateDto post, string postId);
    public Task<PostDto> CreateAsync(PostCreateDto post);
    public Task<bool> DeleteAsync(string userId);

    public Task<PostDto?> GetById(string id, Func<IQueryable<Post>,
        IIncludableQueryable<AppUser, object>>? include = null, bool tracking = true);

    public Task<IEnumerable<PostDto>> GetAll(int page, string? sort, int pageSize = 5, bool tracking = true,
        string? searchText = null);

    public Task<IEnumerable<PostDto?>> Find(Expression<Func<Post, bool>> predicate, Func<IQueryable<Post>,
        IIncludableQueryable<Post, object>>? include = null, bool tracking = true);
}