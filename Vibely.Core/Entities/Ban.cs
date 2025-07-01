namespace FitFlare.Core.Entities;

public class Ban : BaseEntity
{
    public string Reason { get; set; }
    public DateTime? ExpiresAt { get; set; }
    public bool IsPermanent => ExpiresAt == null;

    public string AppUserId { get; set; }
    public AppUser AppUser { get; set; }
}