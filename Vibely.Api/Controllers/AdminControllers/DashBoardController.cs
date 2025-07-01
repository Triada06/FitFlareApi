using FitFlare.Api.Helpers;
using FitFlare.Application.DTOs.Admin;
using FitFlare.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FitFlare.Api.Controllers.AdminControllers;

[ApiController]
[Authorize(Roles = "Admin,Owner")]
public class DashBoardController(IPostService postService, IBanService banService) : ControllerBase
{
    [HttpGet(ApiEndPointsAdmin.DashBoard.Monthly)]
    public async Task<IActionResult> Monthly()
    {
        var monthlyBans = await banService.GetMonthlyBans();
        var monthlyUploaded = await postService.GetMonthlyUploadedPosts();
        return Ok(new DashBoardDto
        {
            BannedUsers = monthlyBans,
            UploadedPosts = monthlyUploaded
        });
    }
}