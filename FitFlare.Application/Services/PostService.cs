using System.Linq.Expressions;
using FitFlare.Application.DTOs.Posts;
using FitFlare.Application.Helpers.Exceptions;
using FitFlare.Application.Mappings;
using FitFlare.Application.Services.Interfaces;
using FitFlare.Core.Entities;
using FitFlare.Infrastructure.Repositories.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.Configuration;
using UGemini;
using UGemini.Enums;

namespace FitFlare.Application.Services;

public class PostService(
    IPostRepository postRepository,
    IAppUserRepository appUserRepository,
    ITagRepository tagRepository,
    IBlobService blobService,
    UserManager<AppUser> userManager,
    IPostLikeRepository postLikeRepository,
    IPostSaveRepository postSaveRepository,
    IConfiguration configuration)
    : IPostService
{
    public async Task<bool> UpdateAsync(PostUpdateDto post, string postId, string userId)
    {
        var user = await appUserRepository.GetByIdAsync(userId, i => i.Include(m => m.Posts).ThenInclude(p => p.Tags));
        if (user == null)
            throw new UserNotFoundException();

        var userPost = user.Posts.FirstOrDefault(x => x.Id == postId);
        if (userPost == null)
            throw new NotFoundException("Post not found");

        userPost.Description = post.Description;

        if (post.Tags is not null && post.Tags.Any())
        {
            // Step 1: Get existing tags from DB
            var existingTags = await tagRepository.FindAsync(tag => post.Tags.Contains(tag.Name)) ?? [];

            var existingTagNames = existingTags.Select(t => t.Name).ToList();
            var newTagNames = post.Tags.Except(existingTagNames).ToList();

            // Step 2: Create new tag entities
            var newTags = newTagNames.Select(name => new Tag { Name = name, UsedCount = 1 }).ToList();

            // Step 3: Increment used count on existing tags
            foreach (var tag in existingTags)
            {
                tag.UsedCount++;
            }

            // (Optional Step 4): Decrement removed tags' used count
            var oldTags = userPost.Tags.ToList();
            foreach (var oldTag in oldTags)
            {
                if (!post.Tags.Contains(oldTag.Name))
                {
                    oldTag.UsedCount = Math.Max(0, oldTag.UsedCount - 1);
                }
            }

            // Step 5: Set final tag list
            userPost.Tags.Clear();
            foreach (var tag in existingTags.Concat(newTags))
            {
                userPost.Tags.Add(tag);
            }
        }
        else
        {
            // If no tags provided, clear existing ones and decrement their count
            foreach (var oldTag in userPost.Tags)
            {
                oldTag.UsedCount = Math.Max(0, oldTag.UsedCount - 1);
            }
            userPost.Tags.Clear();
        }

        return await postRepository.UpdateAsync(userPost);
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

        if (post.HashTags?.Any() == true)
        {
            var found = await tagRepository.FindAsync(
                tag => post.HashTags.Contains(tag.Name),
                i => i.Include(m => m.Posts));
            var existingTags = found
                .Where(t => t != null)
                .Select(t => t!)
                .ToList();

            var existingTagNames = existingTags.Select(t => t.Name).ToHashSet();

            foreach (var tag in existingTags)
            {
                tag.UsedCount++;
            }

            var newTags = post.HashTags
                .Where(tag => !existingTagNames.Contains(tag))
                .Select(tag => new Tag
                {
                    Name = tag,
                    UsedCount = 1
                });

            tags.AddRange(existingTags);
            tags.AddRange(newTags);
        }


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

    public async Task<bool> DeleteAsync(string postId, string userId)
    {
        var user = await appUserRepository.GetByIdAsync(userId, i => i.Include(m => m.Posts).ThenInclude(m => m.Tags));
        if (user == null)
            throw new UserNotFoundException();
        var userPost = user.Posts.FirstOrDefault(x => x.Id == postId);
        if (userPost == null)
            throw new NotFoundException("post not found");

        var postTags = userPost.Tags;
        foreach (var tag in postTags)
        {
            tag.UsedCount--;
        }

        await tagRepository.UpdateRangeAsync(postTags);

        return await postRepository.DeleteAsync(userPost);
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