using System.Security.Claims;
using FitFlare.Api.Helpers;
using FitFlare.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FitFlare.Api.Controllers;

[Authorize(Roles = "Member,Admin,Owner")]
[ApiController]
public class FollowController(IFollowService followService) : ControllerBase
{
    [HttpGet(ApiEndPoints.Follow.GetFollowersByIdUserId)]
    public async Task<IActionResult> GetFollowersByIdUserId([FromRoute] string userId)
    {
        var data = await followService.GetFollowersByIdFollowingId(userId);
        return Ok(data);
    }

    [HttpGet(ApiEndPoints.Follow.GetFollowingsByIdUserId)]
    public async Task<IActionResult> GetFollowingsByIdUserId([FromRoute] string userId)
    {
        var data = await followService.GetFollowingsByIdFollowerId(userId);
        return Ok(data);
    }

    [HttpPost(ApiEndPoints.Follow.FollowUser)]
    public async Task<IActionResult> FollowUser([FromRoute] string userId)
    {
        var authUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (authUserId is null)
            return Unauthorized();
        await followService.Follow(authUserId, userId);
        return Ok();
    }

    [HttpDelete(ApiEndPoints.Follow.UnFollowUser)]
    public async Task<IActionResult> UnFollowUser([FromRoute] string userId)
    {
        var authUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (authUserId is null)
            return Unauthorized();
        await followService.UnFollow(authUserId, userId);
        return Ok();
    }
}