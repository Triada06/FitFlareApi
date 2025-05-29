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
    IPostLikeRepository postLikeRepository,
    IPostSaveRepository postSaveRepository,
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
        if (post.HashTags != null)
            tags.AddRange(from item in post.HashTags
                where !string.IsNullOrWhiteSpace(item)
                select new Tag { Name = item });

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
        return postToCreate.MapToPostDto(media, post.HashTags, false, false);
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
        Func<IQueryable<Post>, IIncludableQueryable<Post, object>>? include = null, bool tracking = true,
        string userIdForLikeChecking = "")
    {
        var data = await postRepository.FindAsync(predicate, include, tracking);
        var returnDtos = new List<PostDto>();
        if (!data.Any())
            return returnDtos;

        var user = await userManager.FindByIdAsync(userIdForLikeChecking);
        if (user == null)
            throw new UserNotFoundException();

        foreach (var post in data)
        {
            var postLike = await postLikeRepository.GetAsync(post!.Id, userIdForLikeChecking);
            var postSave = await postSaveRepository.GetByIdAsync(post.UserId, post.Id);

            if (postLike != null)
            {
                if (postSave != null)
                {
                    returnDtos.Add(post.MapToPostDto(blobService.GetBlobSasUri(post.Media),
                        post.Tags.Select(m => m.Name).ToList(), post.LikedBy.Contains(postLike),
                        post.SavedBy.Contains(postSave)));
                }
                else
                {
                    returnDtos.Add(post.MapToPostDto(blobService.GetBlobSasUri(post.Media),
                        post.Tags.Select(m => m.Name).ToList(), post.LikedBy.Contains(postLike), false));
                }
            }
            else
            {
                if (postSave != null)
                {
                    returnDtos.Add(post.MapToPostDto(blobService.GetBlobSasUri(post.Media),
                        post.Tags.Select(m => m.Name).ToList(), false,
                        post.SavedBy.Contains(postSave)));
                }
                else
                {
                    returnDtos.Add(post.MapToPostDto(blobService.GetBlobSasUri(post.Media),
                        post.Tags.Select(m => m.Name).ToList(), false, false));
                }
            }
        }

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

    public async Task LikePost(string postId, string userId)
    {
        var user = await userManager.FindByIdAsync(userId);
        if (user == null)
            throw new BadRequestException();

        var alreadyLiked = await postLikeRepository.ExistsAsync(postId, userId);
        if (alreadyLiked)
            throw new BadRequestException("Post is already liked by this user");

        var post = await postRepository.GetByIdAsync(postId);
        if (post == null)
            throw new BadRequestException("Post not found");

        var like = new PostLike
        {
            PostId = postId,
            UserId = userId,
            LikedAt = DateTime.UtcNow
        };
        await postLikeRepository.AddAsync(like);

        post.LikeCount++;
        await postRepository.UpdateAsync(post);
    }

    public async Task UnlikePost(string postId, string userId)
    {
        var user = await userManager.FindByIdAsync(userId);
        if (user == null)
            throw new BadRequestException();

        var alreadyUnLiked = await postLikeRepository.ExistsAsync(postId, userId);
        if (!alreadyUnLiked)
            throw new BadRequestException("Post is already unliked by this user");

        var post = await postRepository.GetByIdAsync(postId);
        if (post == null)
            throw new BadRequestException("Post not found");
        var postLike = await postLikeRepository.GetAsync(postId, userId);
        if (postLike == null)
            throw new BadRequestException("Post or user id is wrong");
        await postLikeRepository.RemoveAsync(postLike);

        if (post.LikeCount == 1)
        {
            post.LikeCount = 0;
            await postRepository.UpdateAsync(post);
            return;
        }

        post.LikeCount--;
        await postRepository.UpdateAsync(post);
    }

    public async Task SavePost(string postId, string userId)
    {
        var user = await userManager.FindByIdAsync(userId);
        if (user == null)
            throw new BadRequestException();
        var post = await postRepository.GetByIdAsync(postId);
        if (post is null)
            throw new BadRequestException();
        var alreadySaved = await postSaveRepository.ExistsAsync(postId, userId);
        if (alreadySaved)
            throw new BadRequestException("Post is already saved");
        var postSave = new PostSave
        {
            PostId = postId,
            UserId = userId,
        };
        await postSaveRepository.SaveAsync(postSave);
        post.SaveCount++;
        await postRepository.UpdateAsync(post);
    }

    public async Task UnSavePost(string postId, string userId)
    {
        var user = await userManager.FindByIdAsync(userId);
        if (user == null)
            throw new BadRequestException();
        var post = await postRepository.GetByIdAsync(postId);
        if (post is null)
            throw new BadRequestException();
        var alreadyUnSaved = await postSaveRepository.ExistsAsync(postId, userId);
        if (!alreadyUnSaved)
            throw new BadRequestException("Post is already unsaved");

        var postSave = await postSaveRepository.GetByIdAsync(userId, postId);
        if (postSave == null)
            throw new BadRequestException("Post not found");

        await postSaveRepository.UnSaveAsync(postSave);
        if (post.SaveCount == 1)
        {
            post.SaveCount = 0;
            await postRepository.UpdateAsync(post);
            return;
        }

        post.SaveCount--;
        await postRepository.UpdateAsync(post);
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