using FitFlare.Api.Helpers;
using FitFlare.Application.DTOs.Posts;
using FitFlare.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FitFlare.Api.Controllers;

[Authorize(Roles = "Member,Admin,Owner")]
[ApiController]
public class PostController(IPostService postService) : ControllerBase
{
    [HttpPost(ApiEndPoints.Post.Create)]
    public async Task<ActionResult<PostDto>> CreateAsync([FromForm]PostCreateDto post)
    {
        var postDto = await postService.CreateAsync(post);
        return Ok(postDto);
    }
}