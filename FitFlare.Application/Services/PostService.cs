using System.Linq.Expressions;
using FitFlare.Application.DTOs.Posts;
using FitFlare.Application.Services.Interfaces;
using FitFlare.Core.Entities;
using Microsoft.EntityFrameworkCore.Query;

namespace FitFlare.Application.Services;

public class PostService : IPostService
{
    public Task<bool> UpdateAsync(PostUpdateDto post, string postId)
    {
        throw new NotImplementedException();
    }

    public Task<PostDto> CreateAsync(PostCreateDto post)
    {
        throw new NotImplementedException();
    }

    public Task<bool> DeleteAsync(string userId)
    {
        throw new NotImplementedException();
    }

    public Task<PostDto?> GetById(string id, Func<IQueryable<Post>, IIncludableQueryable<AppUser, object>>? include = null, bool tracking = true)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<PostDto>> GetAll(int page, string? sort, int pageSize = 5, bool tracking = true, string? searchText = null)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<PostDto?>> Find(Expression<Func<Post, bool>> predicate, Func<IQueryable<Post>, IIncludableQueryable<Post, object>>? include = null, bool tracking = true)
    {
        throw new NotImplementedException();
    }
}