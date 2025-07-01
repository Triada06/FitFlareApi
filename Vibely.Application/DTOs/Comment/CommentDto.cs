namespace FitFlare.Application.DTOs.Comment;

public class CommentDto
{
    public required string Id { get; set; }
    public required string CommenterId { get; set; }
    public required string CommenterName { get; set; }
    public string? CommenterProfilePicture { get; set; }
    public required string Content { get; set; }
    public DateTime CommentedWhen { get; set; }
    public int CommentLikeCount { get; set; }
    public bool IsLikedByUser { get; set; }
    public int ReplyCount { get; set; }
}