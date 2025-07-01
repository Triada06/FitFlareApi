namespace FitFlare.Application.DTOs.Chat;

public class ChatDto
{
    public required string Id { get; set; }
    public string? ChatPicture  { get; set; }
    public required string FullNameOrUserName { get; set; }
    public string? LastMessage { get; set; }
    public DateTime? LastMessageTime { get; set; }
}