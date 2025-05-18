using System.Linq.Expressions;
using FitFlare.Application.DTOs.Posts;
using FitFlare.Application.Helpers.Exceptions;
using FitFlare.Application.Mappings;
using FitFlare.Application.Services.Interfaces;
using FitFlare.Core.Entities;
using FitFlare.Infrastructure.Repositories.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Query;

namespace FitFlare.Application.Services;

public class PostService(IPostRepository postRepository, IBlobService blobService, UserManager<AppUser> userManager)
    : IPostService
{
    public Task<bool> UpdateAsync(PostUpdateDto post, string postId)
    {
        throw new NotImplementedException();
    }

    public async Task<PostDto> CreateAsync(PostCreateDto post)
    {
        var user = await userManager.FindByIdAsync(post.UserId);
        if (user == null)
            throw new BadRequestException();

        var contentType = post.Media.ContentType.ToLower();
        string mediaType;
        if (contentType.StartsWith("image/"))
        {
            mediaType = "image";
        }
        else if (contentType.StartsWith("video/"))
        {
            mediaType = "video";
        }
        else throw new ContentTypeException();

        var mediaFileName = post.Media.FileName + Guid.NewGuid();
        await blobService.UploadBlobAsync(post.Media,mediaFileName);
        var media = blobService.GetBlobSasUri(mediaFileName);

        var postToCreate = new Post
        {
            UserId = user.Id,
            MediaType = mediaType,
            Media = mediaFileName,
            Description = post.Description,
        };

        var res = await postRepository.CreateAsync(postToCreate);
        if (!res)
            throw new InternalServerErrorException();
        return postToCreate.MapToPostDto(media);
    }

    public Task<bool> DeleteAsync(string userId)
    {
        throw new NotImplementedException();
    }

    public Task<PostDto?> GetById(string id,
        Func<IQueryable<Post>, IIncludableQueryable<AppUser, object>>? include = null, bool tracking = true)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<PostDto>> GetAll(int page, string? sort, int pageSize = 5, bool tracking = true,
        string? searchText = null)
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<PostDto?>> FindAsync(Expression<Func<Post, bool>> predicate,
        Func<IQueryable<Post>, IIncludableQueryable<Post, object>>? include = null, bool tracking = true)
    {
        var data = await postRepository.FindAsync(predicate, include, tracking);
        var posts = data.ToList();
        var returnDtos = new List<PostDto>();
        if (!posts.Any())
            return returnDtos;

        returnDtos.AddRange(posts.Select(post => post!.MapToPostDto(blobService.GetBlobSasUri(post!.Media))));
        return returnDtos;
    }
}