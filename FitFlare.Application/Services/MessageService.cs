using FitFlare.Application.Helpers.Exceptions;
using FitFlare.Application.Services.Interfaces;
using FitFlare.Core.Entities;
using FitFlare.Infrastructure.Repositories.Interfaces;

namespace FitFlare.Application.Services;

public class MessageService(IMessageRepository messageRepository, IAppUserRepository userRepository) : IMessageService
{
    public async Task DeleteMessage(string messageId)
    {
        var message = await messageRepository.GetByIdAsync(messageId);
        if (message is null)
            throw new NotFoundException("Message not found");
        await messageRepository.DeleteAsync(message);
    }

    public async Task<Message?> GetMessageBetween(string senderId, string receiverId)
        => await messageRepository.GetByIdAsync(receiverId);

}