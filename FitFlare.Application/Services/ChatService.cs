using FitFlare.Application.DTOs.Chat;
using FitFlare.Application.DTOs.Message;
using FitFlare.Application.Helpers.Exceptions;
using FitFlare.Application.Mappings;
using FitFlare.Application.Services.Interfaces;
using FitFlare.Core.Entities;
using FitFlare.Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FitFlare.Application.Services;

public class ChatService(
    IMessageRepository messageRepository,
    IAppUserRepository userRepository,
    IFollowRepository followRepository,
    IMessageService messageService,
    IBlobService blobService) : IChatService
{
    public async Task SaveMessageAsync(string senderId, string receiverId, string encryptedContent)
    {
        var message = new Message
        {
            SenderId = senderId,
            ReceiverId = receiverId,
            EncryptedContent = encryptedContent
        };

        await messageRepository.CreateAsync(message);
    }

    public async Task<IEnumerable<ChatDto>> GetChatsByUserIdAsync(string userId)
    {
        var user = await userRepository.GetByIdAsync(userId,
            i => i.Include(m => m.Followers).Include(m => m.Following));
        if (user is null)
            throw new UserNotFoundException();
        var followChangers =
            await followRepository.FindAsync(m => m.FollowingId == userId && user.Followers.Contains(m),
                tracking: false);
        if (followChangers.Count == 0) return [];

        var chats = followChangers
            .Where(f => f!.FollowerId != userId)
            .Select(follow => follow!.Follower).ToList();

        var returnData = new List<ChatDto>();
        foreach (var chat in chats)
        {
            var lastMessage = await messageService.GetMessageBetween(userId, chat.Id);
            returnData.Add(chat.MapToChatDto(
                chat.ProfilePictureUri is not null ? blobService.GetBlobSasUri(chat.ProfilePictureUri) : null,
                lastMessage?.EncryptedContent, lastMessage?.CreatedAt));
        }

        return returnData;
    }

    public async Task<List<MessageDto>> GetChatHistoryAsync(string userId1, string userId2)
    {
        var messages = await messageRepository.GetMessagesAsync(userId1, userId2);
        return messages.Select(m => m.MapToMessageDto()).ToList();
    }
}