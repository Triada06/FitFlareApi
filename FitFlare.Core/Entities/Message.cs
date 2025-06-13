namespace FitFlare.Core.Entities;

public class Message : BaseEntity
{
    public string SenderId { get; set; } = default!;
    public string ReceiverId { get; set; } = default!;
    public string EncryptedContent { get; set; } = default!;
}