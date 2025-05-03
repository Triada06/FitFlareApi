using FitFlare.Api.Helpers;
using FitFlare.Application.DTOs.AppUserDTos;
using FitFlare.Application.Services;
using FitFlare.Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FitFlare.Api.Controllers;

[ApiController]
public class AppUserController(IAppUserService appUserService) : ControllerBase
{
    [HttpGet(ApiEndPoints.AppUser.GetById)]
    public async Task<IActionResult> GetById([FromRoute] string id)
    {
        var result = await appUserService.GetById(id);
        if (result == null) return NotFound();
        return Ok(result);
    }

    [HttpPost(ApiEndPoints.AppUser.SignUp)]
    public async Task<IActionResult> SignUp([FromForm] AppUserCreateDto request)
    {
        var userDto = await appUserService.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = userDto.Id }, userDto);
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
}