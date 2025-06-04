using System.Security.Claims;
using FitFlare.Api.Helpers;
using FitFlare.Application.DTOs.Comment;
using FitFlare.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FitFlare.Api.Controllers;

[Authorize(Roles = "Member,Admin,Owner")]
[ApiController]
public class CommentController(ICommentService commentService) : ControllerBase
{
    [HttpGet(ApiEndPoints.Comment.GetByPostId)]
    public async Task<IActionResult> GetByPostId(
        [FromRoute] string postId,
        [FromQuery] int page)
    {
        var data = await commentService.GetAllByPostId(postId, page, 20, false);
        return Ok(data);
    }

    [HttpGet(ApiEndPoints.Comment.GetById)]
    public async Task<IActionResult> GetById(string id)
    {
        var data = await commentService.GetById(id);
        return Ok(data);
    }

    [HttpPost(ApiEndPoints.Comment.Create)]
    public async Task<IActionResult> Create([FromBody] CommentCreateDto request)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userId == null)
            return Unauthorized();
        var commentDto = await commentService.CreateAsync(request, userId);
        return CreatedAtAction(nameof(GetById), new { id = commentDto.Id }, commentDto);
    }
}