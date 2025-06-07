using FitFlare.Api.Helpers;
using FitFlare.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FitFlare.Api.Controllers;

[Authorize(Roles = "Member,Admin,Owner")]
[ApiController]
public class TagController(ITagService tagService) : ControllerBase
{
    [HttpGet(ApiEndPoints.Tag.Search)]
    public async Task<IActionResult> GetSearch([FromQuery] string? searchText)
    {
        var data = await tagService.SearchAsync(searchText);
        return Ok(data);
    }
}