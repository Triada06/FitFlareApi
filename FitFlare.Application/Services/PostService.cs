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
            var existingTags = await tagRepository.FindAsync(tag => post.Tags.Contains(tag.Name)) ?? [];

            var existingTagNames = existingTags.Select(t => t.Name).ToList();
            var newTagNames = post.Tags.Except(existingTagNames).ToList();

            var newTags = newTagNames.Select(name => new Tag { Name = name, UsedCount = 1 }).ToList();

            foreach (var tag in existingTags)
            {
                tag.UsedCount++;
            }

            var oldTags = userPost.Tags.ToList();
            foreach (var oldTag in oldTags)
            {
                if (!post.Tags.Contains(oldTag.Name))
                {
                    oldTag.UsedCount = Math.Max(0, oldTag.UsedCount - 1);
                }
            }

            userPost.Tags.Clear();
            foreach (var tag in existingTags.Concat(newTags))
            {
                userPost.Tags.Add(tag);
            }
        }
        else
        {
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
            throw new InternalServerErrorException("Failed to create post");
        return postToCreate.MapToPostDto(media, user.UserName!, post.HashTags, false, false,
            user.ProfilePictureUri is not null ? blobService.GetBlobSasUri(user.ProfilePictureUri) : null);
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

    public async Task<IEnumerable<PostDto>> GetAll(string userId, int page, string? sort, int pageSize = 5,
        bool tracking = true)
    {
        var user = await userManager.FindByIdAsync(userId);
        if (user == null)
            throw new UserNotFoundException();

        var data = await postRepository.GetAllAsync(page, pageSize, tracking, i => i
            .Include(m => m.Comments)
            .Include(m => m.User)
            .Include(m => m.LikedBy)
            .Include(m => m.LikedBy)
            .Include(m => m.Comments)
            .Include(m => m.Tags));

        return data.Select(m => m.MapToPostDto(blobService.GetBlobSasUri(m.Media), m.User.UserName!,
            m.Tags.Select(t => t.Name).ToList(),
            m.LikedBy.Any(l => l.UserId == userId), m.SavedBy.Any(l => l.UserId == userId),
            m.User.ProfilePictureUri is not null ? blobService.GetBlobSasUri(m.User.ProfilePictureUri) : null));
    }

    public async Task<IEnumerable<PostDto?>> GetByTagAsync(string tagId, string userId)
    {
        var user = await userManager.FindByIdAsync(userId);
        if (user == null)
            throw new UserNotFoundException();
        var posts = await postRepository.FindAsync(m => m.Tags.Any(t => t.Id == tagId),
            i => i
                .Include(m => m.Comments)
                .Include(m => m.User)
                .Include(m => m.LikedBy)
                .Include(m => m.LikedBy)
                .Include(m => m.Comments)
                .Include(m => m.Tags), false);
        return posts.Select(m => m?.MapToPostDto(blobService.GetBlobSasUri(m.Media), m.User.UserName!,
            m.Tags.Select(t => t.Name).ToList(),
            m.LikedBy.Any(l => l.UserId == userId), m.SavedBy.Any(l => l.UserId == userId),
            m.User.ProfilePictureUri is not null ? blobService.GetBlobSasUri(m.User.ProfilePictureUri) : null));
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
                    returnDtos.Add(post.MapToPostDto(blobService.GetBlobSasUri(post.Media), post.User.UserName!,
                        post.Tags.Select(m => m.Name).ToList(), post.LikedBy.Contains(postLike),
                        post.SavedBy.Contains(postSave),
                        post.User.ProfilePictureUri is not null
                            ? blobService.GetBlobSasUri(post.User.ProfilePictureUri)
                            : null));
                }
                else
                {
                    returnDtos.Add(post.MapToPostDto(blobService.GetBlobSasUri(post.Media), post.User.UserName!,
                        post.Tags.Select(m => m.Name).ToList(), post.LikedBy.Contains(postLike), false,
                        post.User.ProfilePictureUri is not null
                            ? blobService.GetBlobSasUri(post.User.ProfilePictureUri)
                            : null));
                }
            }
            else
            {
                if (postSave != null)
                {
                    returnDtos.Add(post.MapToPostDto(blobService.GetBlobSasUri(post.Media), post.User.UserName!,
                        post.Tags.Select(m => m.Name).ToList(), false,
                        post.SavedBy.Contains(postSave),
                        post.User.ProfilePictureUri is not null
                            ? blobService.GetBlobSasUri(post.User.ProfilePictureUri)
                            : null));
                }
                else
                {
                    returnDtos.Add(post.MapToPostDto(blobService.GetBlobSasUri(post.Media), post.User.UserName!,
                        post.Tags.Select(m => m.Name).ToList(), false, false,
                        post.User.ProfilePictureUri is not null
                            ? blobService.GetBlobSasUri(post.User.ProfilePictureUri)
                            : null));
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
            throw new InternalServerErrorException("Failed to analyse post");

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