namespace FitFlare.Core;

public class BaseEntity : IBaseEntity
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
}