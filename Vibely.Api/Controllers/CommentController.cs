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
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userId == null)
            return Unauthorized();
        var data = await commentService.GetAllByPostId(postId, userId, page, 20, false);
        return Ok(data);
    }

    [HttpGet(ApiEndPoints.Comment.GetById)]
    public async Task<IActionResult> GetById(string id)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userId == null)
            return Unauthorized();
        var data = await commentService.GetById(id, userId, tracking: false);
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

    [HttpDelete(ApiEndPoints.Comment.Delete)]
    public async Task<IActionResult> Delete(string id)
    {
        await commentService.DeleteAsync(id);
        return Ok();
    }

    [HttpPost(ApiEndPoints.Comment.AddReply)]
    public async Task<IActionResult> AddReply([FromBody] CommentReplyCreateDto request, [FromRoute] string commentId)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userId == null)
            return Unauthorized();
        var res = await commentService.AddReply(request, userId, commentId);
        return CreatedAtAction(nameof(GetById), new { id = res.Id }, res);
    }

    [HttpGet(ApiEndPoints.Comment.Replies)]
    public async Task<IActionResult> LoadReplies([FromRoute] string parentCommentId, [FromQuery] string postId,
        [FromQuery] int page)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userId == null)
            return Unauthorized();
        var data = await commentService.LoadReplies(postId, userId, parentCommentId, page, 20, false);
        return Ok(data);
    }

    [HttpPut(ApiEndPoints.Comment.Like)]
    public async Task<IActionResult> Like([FromRoute] string id)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userId == null)
            return Unauthorized();
        await commentService.LikeComment(id, userId);
        return Ok();
    }

    [HttpPut(ApiEndPoints.Comment.UnLike)]
    public async Task<IActionResult> Unlike([FromRoute] string id)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userId == null)
            return Unauthorized();
        await commentService.UnlikeComment(id, userId);
        return Ok();
    }
}