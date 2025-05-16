using FitFlare.Api.Helpers;
using FitFlare.Application.DTOs.AppUserDTos;
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
    public async Task<IActionResult> SignUp([FromForm] AppUserSignUpDto request)
    {
        var res = await appUserService.SignUpAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = res.UserDto.Id }, res);
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

    [HttpDelete(ApiEndPoints.AppUser.Delete)]
    public async Task<IActionResult> Delete([FromRoute] string id)
    {
        var res = await appUserService.DeleteAsync(id);
        return Ok();
    }

    [HttpPut(ApiEndPoints.AppUser.Update)]
    public async Task<IActionResult> Update([FromForm] AppUserUpdateDto request, [FromRoute] string id)
    {
        var res = await appUserService.UpdateAsync(request, id);
        return Ok();
    }


    /*
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