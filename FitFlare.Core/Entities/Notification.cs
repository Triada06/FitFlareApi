namespace FitFlare.Core.Entities;

public class Notification : BaseEntity
{
    public string UserId { get; set; } // who the notification is for
    public AppUser User { get; set; } = null!;

    public string Type { get; set; }
    public string Message { get; set; } = null!;
    public bool IsRead { get; set; }
}