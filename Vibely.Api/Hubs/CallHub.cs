using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace FitFlare.Api.Hubs;

[Authorize]
public class CallHub : Hub
{
    public async Task CallUser(string targetUserId)
        => await Clients.User(targetUserId).SendAsync("IncomingCall", Context.UserIdentifier);

    public async Task SendOffer(string targetUserId, string offer, bool isVideoCall) =>
        await Clients.User(targetUserId)
            .SendAsync("ReceiveOffer", Context.UserIdentifier, offer, isVideoCall);

    public async Task SendAnswer(string targetUserId, string answer)
        => await Clients.User(targetUserId).SendAsync("ReceiveAnswer", Context.UserIdentifier, answer);

    public async Task SendIceCandidate(string targetUserId, string candidate)
        => await Clients.User(targetUserId).SendAsync("ReceiveIceCandidate", Context.UserIdentifier, candidate);

    public async Task EndCall(string targetUserId)
        => await Clients.User(targetUserId).SendAsync("CallEnded", Context.UserIdentifier);
}