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
}