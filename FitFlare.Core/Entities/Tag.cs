namespace FitFlare.Core.Entities;

public class Tag : BaseEntity
{
    public required string Name { get; set; }
    public int UsedCount { get; set; }
    public ICollection<Post> Posts { get; set; } = [];
}