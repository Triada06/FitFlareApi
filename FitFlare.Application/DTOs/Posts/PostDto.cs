using FitFlare.Core.Entities;

namespace FitFlare.Application.DTOs.Posts;

public class PostDto
{
    public required string Id { get; set; }
    public required string MediaUri { get; set; }
    public string? Description { get; set; }
    public int LikeCount { get; set; }
    public required string MediaType { get; set; }
    public required string PostedById { get; set; }
    public int CommentCount { get; set; }
    public bool IsLikedByUser  { get; set; }
    public bool IsSavedByUSer { get; set; }
    public DateTime PostedWhen { get; set; }
    public IEnumerable<string>? HashTags { get; set; } 
}