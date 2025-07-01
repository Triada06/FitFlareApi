using System.Security.Claims;
using FitFlare.Api.Helpers;
using FitFlare.Application.DTOs.Notification;
using FitFlare.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FitFlare.Api.Controllers;

[Authorize(Roles = "Member,Admin,Owner")]
[ApiController]
public class NotificationController(INotificationService notificationService) : ControllerBase
{
    [HttpPut(ApiEndPoints.Notification.MarkAsRead)]
    public async Task<IActionResult> MarkAsRead([FromRoute] string id)
    {
        await notificationService.MarkAsReadAsync(id);
        return Ok();
    }

    [HttpDelete(ApiEndPoints.Notification.Delete)]
    public async Task<IActionResult> Delete([FromRoute] string id)
    {
        await notificationService.DeleteASync(id);
        return Ok();
    }

    [HttpPut(ApiEndPoints.Notification.MarkAllAsRead)]
    public async Task<IActionResult> MarkAllAsRead()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userId == null)
            return Unauthorized();
        await notificationService.MarkAllAsReadAsync(userId);
        return Ok();
    }

    [HttpGet(ApiEndPoints.Notification.GetAllByUserId)]
    public async Task<IActionResult> GetAllByUserId()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if(userId is null) return Unauthorized();
        var data = await notificationService.GetAllByUserIdAsync(userId);
        return Ok(data);
    }

    [HttpGet(ApiEndPoints.Notification.GetById)]
    public async Task<IActionResult> GetById([FromRoute] string id)
    {
        var data = await notificationService.GetByIdAsync(id);
        return Ok(data);
    }

    [HttpPost(ApiEndPoints.Notification.Create)]
    public async Task<IActionResult> Create([FromBody] CreateNotificationRequest request)
    {
        await notificationService.CreateAsync(request);
        return Ok();
    }
}