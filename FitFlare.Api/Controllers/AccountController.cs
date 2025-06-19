using FitFlare.Api.Helpers;
using FitFlare.Application.DTOs.Account;
using FitFlare.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;

namespace FitFlare.Api.Controllers;

[AllowAnonymous]
[ApiController]
public class AccountController(IAppUserService appUserService) : ControllerBase
{
    [HttpPost(ApiEndPoints.Account.ForgotPassword)]
    public async Task<ActionResult> ForgotPassword([FromBody] ForgotPasswordDto request)
    {
        await appUserService.ForgotPassword(request);
        return Ok();
    }
    [HttpPost(ApiEndPoints.Account.ResetPassword)]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto request)
    {
        var res = await appUserService.ResetPassword(request);
        return res ? Ok() : BadRequest();
    }
}