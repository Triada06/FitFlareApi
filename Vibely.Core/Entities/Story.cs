namespace FitFlare.Core.Entities;

public class Story : BaseEntity
{
    public required string MediaUri { get; set; }
    public required string MediaType { get; set; }
    public ICollection<StoryView> Viewers  { get; set; } = [];

    public string UserId { get; set; }
    public AppUser User { get; set; } 
}