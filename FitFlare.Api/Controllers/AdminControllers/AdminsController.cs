using FitFlare.Api.Helpers;
using FitFlare.Application.Contracts.Requests;
using FitFlare.Application.DTOs.Admin.Admins;
using FitFlare.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FitFlare.Api.Controllers.AdminControllers;

[ApiController]
[Authorize(Roles = "Owner")]
public class AdminsController(IAdminsService adminsService) : ControllerBase
{
    [HttpGet(ApiEndPointsAdmin.Admin.GetAll)]
    public async Task<IActionResult> GetAllAdminsAsync()
    {
        var data = await adminsService.GetAllAdminsAsync();
        return Ok(data);
    }

    [HttpPut(ApiEndPointsAdmin.Admin.MakeAppOwner)]
    public async Task<IActionResult> MakeAdminAsync([FromRoute] string id)
    {
        await adminsService.MakeAppOwnerAsync(id);
        return Ok();
    }
    [AllowAnonymous] 
    [HttpGet(ApiEndPointsAdmin.Admin.ConfirmOwnerTransfer)]
    public async Task<IActionResult> ConfirmOwnerTransfer(string token)
    {
        await adminsService.ConfirmOwnerTransferAsync(token);
        return Ok("Ownership transferred successfully.");
    }

    [HttpPut(ApiEndPointsAdmin.Admin.PromoteToAdmin)]
    public async Task<IActionResult> PromoteToAdmin([FromRoute] string id)
    {
        await adminsService.PromoteToAdminAsync(id);
        return Ok();
    }

    [HttpDelete(ApiEndPointsAdmin.Admin.RemoveAdmin)]
    public async Task<IActionResult> RemoveAdmin(string id)
    {
        await adminsService.RemoveAdminAsync(id);
        return Ok();
    }

    [HttpPost(ApiEndPointsAdmin.Admin.FindByEmail)]
    public async Task<IActionResult> GetAdminByEmail([FromBody]PromotionRequest request)
        {
        var data = await adminsService.FindByEmail(request.Email);
        return Ok(data);
    }
}