using FitFlare.Application.DTOs.Message;
using FitFlare.Core.Entities;

namespace FitFlare.Application.Services.Interfaces;

public interface IMessageService
{
    public Task DeleteMessage(string messageId);
    public Task<Message?> GetMessageBetween(string senderId, string receiverId);
    public Task<IEnumerable<SearchMessageDto>> SearchMessages(string searchText, string authUserId);
}