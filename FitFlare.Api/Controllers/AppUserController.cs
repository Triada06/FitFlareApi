using System.Security.Claims;
using FitFlare.Api.Helpers;
using FitFlare.Application.Contracts.Requests;
using FitFlare.Application.DTOs.AppUser;
using FitFlare.Application.Services;
using FitFlare.Application.Services.Interfaces;
using FitFlare.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace FitFlare.Api.Controllers;

[Authorize(Roles = "Member,Admin,Owner")]
[ApiController]
public class AppUserController(IAppUserService appUserService, RoleManager<IdentityRole> roleManager) : ControllerBase
{
    [HttpGet(ApiEndPoints.AppUser.GetById)]
    public async Task<IActionResult> GetById([FromRoute] string id)
    {
        var result = await appUserService.GetById(id);
        if (result == null) return NotFound();
        return Ok(result);
    }

    [AllowAnonymous]
    [HttpPost(ApiEndPoints.AppUser.SignUp)]
    public async Task<IActionResult> SignUp([FromBody] AppUserSignUpDto request)
    {
        var res = await appUserService.SignUpAsync(request);
        return Ok(res);
    }

    [AllowAnonymous]
    [HttpPost(ApiEndPoints.AppUser.SignIn)]
    public async Task<IActionResult> SignIn([FromBody] AppUserSignInDto request)
    {
        var token = await appUserService.SignInAsync(request);
        return Ok(token);
    }


    [HttpGet(ApiEndPoints.AppUser.GetAll)]
    public async Task<IActionResult> GetAll(
        [FromQuery] string? sort,
        [FromQuery] string? searchText,
        [FromQuery] int page)
    {
        if (page < 1) page = 1;
        var data = await appUserService.GetAll(page, sort, 5, false, searchText);
        return Ok(data);
    }

    [HttpGet(ApiEndPoints.AppUser.Search)]
    public async Task<IActionResult> GetSearch([FromQuery] string? searchText)
    {
        var data = await appUserService.SearchAsync(searchText);
        return Ok(data);
    }

    [HttpDelete(ApiEndPoints.AppUser.Delete)]
    public async Task<IActionResult> Delete([FromRoute] string id)
    {
        var res = await appUserService.DeleteAsync(id);
        return Ok();
    }

    [HttpPut(ApiEndPoints.AppUser.EditProfile)]
    public async Task<IActionResult> EditProfile([FromForm] AppUserUpdateDto request)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userId == null)
            return Unauthorized();
        await appUserService.UpdateAsync(request, userId);
        return Ok();
    }

    [HttpPut(ApiEndPoints.AppUser.EditProfilePrivacy)]
    public async Task<IActionResult> ChangeProfilePrivacy()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userId == null)
            return Unauthorized();
        await appUserService.ChangePrivacy(userId);
        return Ok();
    }

    [HttpPost(ApiEndPoints.AppUser.VerifyPassword)]
    public async Task<IActionResult> VerifyPassword([FromBody] PasswordVerifyRequest passwordDto)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userId == null)
            return Unauthorized();
        var response = await appUserService.VerifyPassword(userId, passwordDto.Password);
        return response ? Ok() : BadRequest(new { message = "Wrong password bro" });
    }

    [HttpPut(ApiEndPoints.AppUser.ChangePassword)]
    public async Task<IActionResult> ChangePassword([FromBody] PasswordChangeRequest request)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userId is null)
            return Unauthorized();
        await appUserService.ChangePassword(userId, request);
        return Ok();
    }

    /*[AllowAnonymous]
    [HttpPost("api/addroles")]
    public async Task<IActionResult> CreateRole()
    {
        foreach (var role in Enum.GetValues(typeof(AppRoles)))
        {
            if (!await roleManager.RoleExistsAsync(role.ToString()))
                await roleManager.CreateAsync(new IdentityRole(role.ToString()));
        }

        return Ok();
    }*/
}