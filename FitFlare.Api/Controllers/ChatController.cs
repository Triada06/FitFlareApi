using System.Security.Claims;
using FitFlare.Api.Helpers;
using FitFlare.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FitFlare.Api.Controllers;

[Authorize(Roles = "Member,Admin,Owner")]
[ApiController]
public class ChatController(IChatService chatService) : ControllerBase
{
    [HttpGet(ApiEndPoints.Chat.GetMessagesWithUser)]
    public async Task<IActionResult> GetMessagesWithUser(string userId)
    {
        var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (currentUserId == null)
            return Unauthorized();

        var messages = await chatService.GetChatHistoryAsync(currentUserId, userId);

        return Ok(messages);
    }

    [HttpGet(ApiEndPoints.Chat.LoadChats)]
    public async Task<IActionResult> LoadChats()
    {
        var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (currentUserId == null)
            return Unauthorized();
        return Ok(await chatService.GetChatsByUserIdAsync(currentUserId));
    }
}