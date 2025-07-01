using FitFlare.Api.Helpers;
using FitFlare.Application.DTOs.Ban;
using FitFlare.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FitFlare.Api.Controllers.AdminControllers;

[ApiController]
[Authorize(Roles = "Admin,Owner")]
public class BanController(IBanService banService) : ControllerBase
{
    [HttpPost(ApiEndPointsAdmin.Ban.Create)]
    public async Task<IActionResult> Create([FromBody] BanCreateDto request)
    {
        await banService.BanUser(request);
        return Ok();
    }

    [HttpDelete(ApiEndPointsAdmin.Ban.Delete)]
    public async Task<IActionResult> Delete([FromRoute] string id)
    {
        await banService.RemoveBan(id);
        return Ok();
    }

    [HttpGet(ApiEndPointsAdmin.Ban.GetById)]
    public async Task<IActionResult> GetById([FromRoute] string id) =>
        Ok(await banService.GetById(id));

    [HttpPut(ApiEndPointsAdmin.Ban.Update)]
    public async Task<IActionResult> Update([FromRoute] string id, [FromBody] BanUpdateDto request)
    {
        var data = await banService.Update(id, request);
        return Ok(data);
    }
}