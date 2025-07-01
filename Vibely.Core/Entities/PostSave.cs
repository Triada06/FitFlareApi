namespace FitFlare.Core.Entities;

public class PostSave
{
    public string UserId { get; set; }
    public AppUser User { get; set; }

    public string PostId { get; set; }
    public Post Post { get; set; }
    
    public DateTime SavedAt { get; set; } = DateTime.UtcNow;
}