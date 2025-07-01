using System.ComponentModel.DataAnnotations;

namespace FitFlare.Core.Entities;

public class Post : BaseEntity
{
    public required string Media { get; set; }
    public string? Description { get; set; }
    public int LikeCount { get; set; }
    public int SaveCount { get; set; }
    public required string MediaType { get; init; }
    public required string Status { get; init; }
    
    public string UserId { get; set; }
    public AppUser User { get; set; }

    public ICollection<Notification> Notifications { get; set; } = [];
    public ICollection<PostSave> SavedBy { get; set; } = [];
    public ICollection<PostLike> LikedBy { get; set; } = [];
    public ICollection<Comment> Comments { get; set; } = [];    
    public ICollection<Tag> Tags { get; set; } = [];
}