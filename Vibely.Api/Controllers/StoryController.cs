using System.Security.Claims;
using FitFlare.Api.Helpers;
using FitFlare.Application.DTOs.Story;
using FitFlare.Application.Services.Interfaces;
using FitFlare.Core.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FitFlare.Api.Controllers;

[Authorize(Roles = "Member,Admin,Owner")]
[ApiController]
public class StoryController(IStoryService storyService) : ControllerBase
{
    [HttpGet(ApiEndPoints.Story.GetById)]
    public async Task<IActionResult> GetById([FromRoute] string id)
        => Ok(await storyService.GetStoryById(id));

    [HttpPost(ApiEndPoints.Story.Create)]
    public async Task<IActionResult> CreateStory([FromForm] StoryCreateDto storyDto)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userId == null)
            return Unauthorized();
        var res = await storyService.UploadStory(storyDto, userId);
        return CreatedAtAction(nameof(GetById), new { id = res.Id }, res);
    }

    [HttpDelete(ApiEndPoints.Story.Delete)]
    public async Task<IActionResult> Delete([FromRoute] string id)
    {
        await storyService.DeleteStory(id);
        return Ok();
    }

    [HttpGet(ApiEndPoints.Story.GetViewers)]
    public async Task<IActionResult> GetViewers([FromRoute] string id)
    {
        var data = await storyService.GetStoryViewers(id);
        return Ok(data);
    }

    [HttpPost(ApiEndPoints.Story.View)]
    public async Task<IActionResult> ViewStory(string id)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userId == null)
            return Unauthorized();
        await storyService.ViewStory(id, userId);
        return Ok();
    }

    [HttpGet(ApiEndPoints.Story.AuthUserStories)]
    public async Task<IActionResult> GetUserFollowingStories()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userId == null)
            return Unauthorized();
        var data = await storyService.GetUserFollowingStories(userId);
        return Ok(data);
    }
}