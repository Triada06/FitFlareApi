using System.Security.Claims;
using FitFlare.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace FitFlare.Api.Hubs;

[Authorize]
public class ChatHub(IChatService chatService) : Hub
{
    private static readonly Dictionary<string, string> ConnectedUsers = new(); // userId -> connectionId

    public async Task SendMessage(string receiverId, string encryptedContent)
    {
        try
        {
            var senderId = Context.UserIdentifier!;
            await chatService.SaveMessageAsync(senderId, receiverId, encryptedContent);
            if (senderId != receiverId)
            {
                await Clients.User(receiverId).SendAsync("ReceiveMessage", senderId, encryptedContent, DateTime.UtcNow);
            }

            await Clients.User(senderId).SendAsync("ReceiveMessage", senderId, encryptedContent, DateTime.UtcNow);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public override async Task OnConnectedAsync()
    {
        var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!string.IsNullOrEmpty(userId))
        {
            ConnectedUsers[userId] = Context.ConnectionId;
            await Clients.All.SendAsync("UserConnected", userId);
        }

        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var userId = ConnectedUsers.FirstOrDefault(x => x.Value == Context.ConnectionId).Key;
        if (!string.IsNullOrEmpty(userId))
        {
            ConnectedUsers.Remove(userId);
            await Clients.All.SendAsync("UserDisconnected", userId);
        }

        await base.OnDisconnectedAsync(exception);
    }

    public static bool IsUserOnline(string userId)
    {
        return ConnectedUsers.ContainsKey(userId);
    }

    public static Dictionary<string, bool> GetOnlineStatusForUsers(List<string> userIds)
    {
        return userIds.ToDictionary(id => id, id => ConnectedUsers.ContainsKey(id));
    }

    public Task<List<string>> CheckUsersOnline(List<string> userIds)
    {
        var online = userIds.Where(id => ConnectedUsers.ContainsKey(id)).ToList();
        return Task.FromResult(online);
    }
}