namespace FitFlare.Core.Entities;

public class StoryView 
{
    public string UserId { get; set; }
    public AppUser User { get; set; }

    public string StoryId { get; set; }
    public Story Story { get; set; }

    public DateTime PostedAt { get; set; } = DateTime.UtcNow;
}