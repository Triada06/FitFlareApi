using FitFlare.Api.Helpers;
using FitFlare.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FitFlare.Api.Controllers.AdminControllers;

[ApiController]
[Authorize(Roles = "Admin,Owner")]
public class UserController(IAppUserService appUserService) : ControllerBase
{
    [HttpGet(ApiEndPointsAdmin.AppUser.GetAll)]
    public async Task<IActionResult> GetAll(
        [FromQuery] string? sort,
        [FromQuery] string? searchText,
        [FromQuery] int page,
        [FromQuery] int pageSize)
    {
        if (page < 1) page = 1;
        var data = await appUserService.GetAll(page, sort, pageSize, false, searchText);
        return Ok(data);
    }

    [HttpGet(ApiEndPointsAdmin.AppUser.GetById)]
    public async Task<IActionResult> GetById([FromRoute] string id)
    {
        var data = await appUserService.GetSingleUser(id);
        return Ok(data);
    }
}