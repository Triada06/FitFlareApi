namespace FitFlare.Core.Entities;

public class Comment : BaseEntity
{
    public required string Content { get; set; }
    public int RepliesCount { get; set; }
    public string PostId { get; set; }
    public Post Post { get; set; }

    public string UserId { get; set; }
    public AppUser User { get; set; }

    public ICollection<CommentLike> Likes { get; set; }= [];
    
    public string? ParentCommentId { get; set; }  // null if top-level
    public Comment? ParentComment { get; set; }
    public ICollection<Comment> Replies { get; set; } = new List<Comment>();
}