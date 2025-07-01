using FitFlare.Application.DTOs.Chat;
using FitFlare.Application.DTOs.Message;
using FitFlare.Core.Entities;

namespace FitFlare.Application.Services.Interfaces;

public interface IChatService
{
    public Task SaveMessageAsync(string senderId, string receiverId, string encryptedContent);
    public Task<IEnumerable<ChatDto>> GetChatsByUserIdAsync(string userId);
    public Task<List<MessageDto>> GetChatHistoryAsync(string userId1, string userId2);
}