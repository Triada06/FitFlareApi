using System.Security.Claims;
using FitFlare.Api.Helpers;
using FitFlare.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FitFlare.Api.Controllers.AdminControllers;

[ApiController]
[Authorize(Roles = "Admin,Owner")]
public class PostAdminController(IPostService postService) : ControllerBase
{
    [HttpDelete(ApiEndPointsAdmin.Post.TakeDown)]
    public async Task<IActionResult> Delete([FromRoute] string id)
    {
        await postService.TakeDownAsync(id);
        return Ok();
    }
}