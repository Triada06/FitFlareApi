namespace FitFlare.Application.DTOs.Message;

public class MessageDto
{
    public required string Id { get; set; }
    public required string SenderId { get; set; }
    public required string ReceiverId { get; set; }
    public string EncryptedContent { get; set; } = default!;
    public DateTime SentAt { get; set; }
}