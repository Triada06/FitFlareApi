namespace FitFlare.Core.Entities;

public class Story : BaseEntity
{
    public required string MediaUri { get; set; }
    public IEnumerable<string> ViewerIds { get; set; } = [];

    public string UserId { get; set; }
    public AppUser User { get; set; } 
}