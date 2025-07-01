namespace FitFlare.Core.Entities;

public class Follow : BaseEntity
{
    public string FollowerId { get; set; }
    public AppUser Follower { get; set; } = null!; 
    public string FollowingId { get; set; }
    public AppUser Following { get; set; } = null!;
}