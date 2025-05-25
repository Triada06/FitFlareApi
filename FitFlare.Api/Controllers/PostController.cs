using FitFlare.Api.Helpers;
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
        var res = await postService.GetById(id);
        return Ok(res);
    }
}