namespace FitFlare.Core;

public interface IBaseEntity
{
    public string Id { get; set; }
    public DateTime CreatedAt { get; init; }

}