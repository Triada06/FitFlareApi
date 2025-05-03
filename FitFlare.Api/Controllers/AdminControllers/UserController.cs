using FitFlare.Api.Helpers;
using FitFlare.Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FitFlare.Api.Controllers.AdminControllers;

[ApiController]
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
}