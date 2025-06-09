using System.Security.Claims;
using FitFlare.Api.Helpers;
using FitFlare.Application.DTOs.Comment;
using FitFlare.Application.DTOs.Posts;
using FitFlare.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FitFlare.Api.Controllers;

[Authorize(Roles = "Member,Admin,Owner")]
[ApiController]
public class PostController(IPostService postService, IWebHostEnvironment environment) : ControllerBase
{
    [HttpPost(ApiEndPoints.Post.Create)]
    public async Task<ActionResult<PostDto>> CreateAsync([FromForm] PostCreateDto post)
    {
        if (post.Status == "Publishing")
        {
            var postDto = await postService.CreateAsync(post);
            return CreatedAtAction(nameof(GetById), new { id = postDto.Id }, postDto);
        }

        var uploadsRoot = Path.Combine(environment.WebRootPath, "temp");

        if (!Directory.Exists(uploadsRoot))
            Directory.CreateDirectory(uploadsRoot);

        var fileName = $"{Guid.NewGuid()} - {post.Media.FileName}";
        var filePath = Path.Combine(uploadsRoot, fileName);

        await using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await post.Media.CopyToAsync(stream);
        }

        var postAnalyseDto = new PostAnalyseDto
        {
            Media = post.Media,
            UserId = post.UserId,
            LocalFileName = fileName,
        };

        var aiResponse = await postService.AiAnalyse(postAnalyseDto);
        return Ok(aiResponse);
    }

    [HttpGet(ApiEndPoints.Post.GetById)]
    public async Task<ActionResult<PostDto>> GetById([FromRoute] string id)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userId == null)
            return Unauthorized();
        var res = await postService.GetByIdAsync(id, userId);
        return Ok(res);
    }

    [HttpGet(ApiEndPoints.Post.GetByTag)]
    public async Task<ActionResult<IEnumerable<PostDto>>> GetByTag([FromRoute] string tagId)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userId == null)
            return Unauthorized();
        var data = await postService.GetByTagAsync(tagId, userId);
        return Ok(data);
    }

    [HttpDelete(ApiEndPoints.Post.Delete)]
    public async Task<ActionResult> Delete([FromRoute] string id)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userId == null)
            return Unauthorized();
        await postService.DeleteAsync(id, userId);
        return Ok();
    }

    [HttpPut(ApiEndPoints.Post.Update)]
    public async Task<ActionResult> Update([FromBody] PostUpdateDto post, [FromRoute] string id)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userId == null)
            return Unauthorized();
        await postService.UpdateAsync(post, id, userId);
        return Ok();
    }

    [HttpPut(ApiEndPoints.Post.Like)]
    public async Task<ActionResult> Like([FromRoute] string id)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userId == null)
            return Unauthorized();
        await postService.LikePost(id, userId);
        return Ok();
    }

    [HttpPut(ApiEndPoints.Post.UnLike)]
    public async Task<ActionResult> UnLike([FromRoute] string id)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userId == null)
            return Unauthorized();
        await postService.UnlikePost(id, userId);
        return Ok();
    }

    [HttpPut(ApiEndPoints.Post.Save)]
    public async Task<ActionResult> Save([FromRoute] string id)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userId == null)
            return Unauthorized();
        await postService.SavePost(id, userId);
        return Ok();
    }

    [HttpPut(ApiEndPoints.Post.UnSave)]
    public async Task<ActionResult> UnSave([FromRoute] string id)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userId == null)
            return Unauthorized();
        await postService.UnSavePost(id, userId);
        return Ok();
    }

    [HttpGet(ApiEndPoints.Post.GetAll)]
    public async Task<ActionResult<CommentDto[]>> GetAll([FromQuery] int page)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userId == null)
            return Unauthorized();
        var res = await postService.GetAll(userId, page, null, 15, false);
        return Ok(res);
    }
}