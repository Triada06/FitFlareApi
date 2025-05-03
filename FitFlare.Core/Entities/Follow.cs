namespace FitFlare.Core.Entities;

public class Follow : BaseEntity
{
    public string FollowerId { get; set; }       // who follows
    public AppUser Follower { get; set; }

    public string FollowingId { get; set; }      // who gets followed
    public AppUser Following { get; set; }
}