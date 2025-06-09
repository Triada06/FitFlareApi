namespace FitFlare.Application.DTOs.Notification;

public class NotificationDto
{
    public required string Id { get; set; }
    public required string Type { get; set; }
    public required string AddressedUserId { get; set; }
    public required string Message { get; set; }
    public required string TriggerUserId { get; set; }
    public required string TriggerUserName { get; set; }
    public string? TriggerUserProfilePicture { get; set; }
    public string? PostId { get; set; }
    public string? PostMediaUri { get; set; }
    public required DateTime CreatedAt { get; set; }
    public required bool IsRead { get; set; }
}