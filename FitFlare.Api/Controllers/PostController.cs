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
    public async Task<ActionResult<PostDto>> CreateAsync([FromForm] PostCreateDto post)
    {
        var postDto = await postService.CreateAsync(post);
        return CreatedAtAction(nameof(GetById), new { id = postDto.Id }, postDto);
    }
    
    [HttpGet(ApiEndPoints.Post.GetById)]
    public async Task<ActionResult<PostDto>> GetById([FromRoute] string id)
    {
        var res = await postService.GetById(id);
        return Ok(res);
    }
}