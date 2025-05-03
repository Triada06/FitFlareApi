using System.ComponentModel.DataAnnotations;

namespace FitFlare.Core.Entities;

public class Rating : BaseEntity
{
    [Range(0, 5)] public int Score { get; set; }

    public string PostId { get; set; }
    public Post Post { get; set; }

    public string UserId { get; set; }
    public AppUser User { get; set; }
}