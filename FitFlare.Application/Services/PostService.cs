using System.Linq.Expressions;
using FitFlare.Application.DTOs.Posts;
using FitFlare.Application.Helpers.Exceptions;
using FitFlare.Application.Mappings;
using FitFlare.Application.Services.Interfaces;
using FitFlare.Core.Entities;
using FitFlare.Infrastructure.Repositories.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.Configuration;
using UGemini;
using UGemini.Enums;

namespace FitFlare.Application.Services;

public class PostService(
    IPostRepository postRepository,
    IBlobService blobService,
    UserManager<AppUser> userManager,
    IConfiguration configuration)
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
        var mediaType = GetMediaType(post.Media.ContentType);

        var mediaFileName = post.Media.FileName + Guid.NewGuid();
        await blobService.UploadBlobAsync(post.Media, mediaFileName);
        var media = blobService.GetBlobSasUri(mediaFileName);

        List<Tag> tags = [];
        tags.AddRange(from item in post.HashTags where !string.IsNullOrWhiteSpace(item) select new Tag { Name = item });

        var postToCreate = new Post
        {
            UserId = user.Id,
            MediaType = mediaType,
            Media = mediaFileName,
            Description = post.Description,
            Tags = tags,
            Status = "Published",
        };

        var res = await postRepository.CreateAsync(postToCreate);
        if (!res)
            throw new InternalServerErrorException();
        return postToCreate.MapToPostDto(media, post.HashTags.ToList());
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

        returnDtos.AddRange(posts.Select(post =>
            post!.MapToPostDto(blobService.GetBlobSasUri(post!.Media),
                post.Tags.Select(m => m.Name).ToList())));
        return returnDtos;
    }

    public async Task<string> AiAnalyse(PostAnalyseDto dto)
    {
        using var memoryStream = new MemoryStream();
        await dto.Media.CopyToAsync(memoryStream);
        memoryStream.Position = 0;
        string base64Image = Convert.ToBase64String(memoryStream.ToArray());
        string mimeType = dto.Media.ContentType;
        var mediaType = GetMediaType(dto.Media.ContentType);

        var post = new Post
        {
            Media = dto.LocalFileName,
            UserId = dto.UserId,
            Status = dto.Status,
            MediaType = mediaType,
        };

        if (!await postRepository.CreateAsync(post))
            throw new InternalServerErrorException();

        var apiKey = configuration["GeminiApiKey"];
        var client = new GeminiClient(apiKey);
        var result = await client.GenerateTextWithImageAsync(
            "I'm posting this image on a social media, review this image and give a short (about 150 characters) advise on how can i improve it",
            base64Image, mimeType, GeminiModel.Gemini15Flash);

        return result;
    }

    public string GetMediaType(string contentType)
    {
        var mediaType = contentType.ToLower();
        string result;
        if (mediaType.StartsWith("image/"))
            result = "image";
        else if (mediaType.StartsWith("video/"))
            result = "video";
        else throw new ContentTypeException();

        return result;
    }

    public async Task DeleteDraftedMediaAsync()
    {
        var data = await postRepository.FindAsync(m => m.Status == "Drafted");
        var listData = data.ToList();
        if (!listData.Any()) return;
        foreach (var item in listData)
            await postRepository.DeleteAsync(item!);
    }
}