using System.ComponentModel.DataAnnotations;

namespace FitFlare.Core.Entities;

public class Post : BaseEntity
{
    [Range(0, 5)] public double AverageScore { get; set; }

    public required string Image { get; set; }
    public string? Description { get; set; }

    public string UserId { get; set; }
    public AppUser User { get; set; }

    public ICollection<Comment> Comments { get; set; } = [];
    public ICollection<Rating> Ratings { get; set; } = [];
    public ICollection<Tag> Tags { get; set; } = [];
}