using System.Security.Claims;
using FitFlare.Api.Helpers;
using FitFlare.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FitFlare.Api.Controllers;

[Authorize(Roles = "Member,Admin,Owner")]
[ApiController]
public class MessageController(IMessageService messageService) : ControllerBase
{
    [HttpDelete(ApiEndPoints.Message.Delete)]
    public async Task<IActionResult> Delete([FromRoute] string id)
    {
        await messageService.DeleteMessage(id);
        return Ok();
    }

    [HttpGet(ApiEndPoints.Message.Search)]
    public async Task<IActionResult> Search([FromQuery] string searchText)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userId == null)
            return Unauthorized();
        var res = await messageService.SearchMessages(searchText, userId);
        return Ok(res);
    }
}