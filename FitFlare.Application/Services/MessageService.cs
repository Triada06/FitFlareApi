using FitFlare.Application.DTOs.Message;
using FitFlare.Application.Helpers.Exceptions;
using FitFlare.Application.Mappings;
using FitFlare.Application.Services.Interfaces;
using FitFlare.Core.Entities;
using FitFlare.Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FitFlare.Application.Services;

public class MessageService(
    IMessageRepository messageRepository,
    IAppUserRepository userRepository,
    IBlobService
        blobService) : IMessageService
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

    public async Task<IEnumerable<SearchMessageDto>> SearchMessages(string searchText, string authUserId)
    {
        var user = await userRepository.GetByIdAsync(authUserId);
        if (user is null)
            throw new UserNotFoundException();
        var messages =
            await messageRepository.FindAsync(m => EF.Functions.ILike(m.EncryptedContent, $"%{searchText}%"));
        if (messages.Count == 0)
            return [];

        var returnList = new List<SearchMessageDto>();
        foreach (var message in messages)
        {
            var chat = await userRepository.GetByIdAsync(message!.SenderId != user.Id ? message.SenderId : message.ReceiverId);
            if (chat is not null)
                returnList.Add(message.MapToMessageSearchDto(chat.FullName ?? chat.UserName!,
                    chat.ProfilePictureUri is not null ? blobService.GetBlobSasUri(chat.ProfilePictureUri) : null));
        }

        return returnList;
    }
}