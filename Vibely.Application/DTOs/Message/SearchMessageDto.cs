namespace FitFlare.Application.DTOs.Message;

public class SearchMessageDto
{
    public required string SenderId { get; set; }
    public required string ReceiverId { get; set; }
    public required string MessageId { get; set; }
    public required string ChatName { get; set; }
    public string? ChatPictureSasUri { get; set; }
}