namespace FitFlare.Application.DTOs.Notification;

public class CreateNotificationRequest
{
    public required string NotificationType { get; set; }
    public required string AddressedUserId { get; set; }
    public required string TriggeredUserId { get; set; }
    public string? PostId { get; set; }
    public string? PostMediaUri { get; set; }
}