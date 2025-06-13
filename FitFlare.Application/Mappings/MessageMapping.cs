using FitFlare.Application.DTOs.Message;
using FitFlare.Core.Entities;

namespace FitFlare.Application.Mappings;

public static class MessageMapping
{
    public static MessageDto MapToMessageDto(this Message message)
    {
        return new MessageDto
        {
            Id = message.Id,
            SenderId = message.SenderId,
            ReceiverId = message.ReceiverId,
            EncryptedContent = message.EncryptedContent,
            SentAt = message.CreatedAt,
        };
    }
}