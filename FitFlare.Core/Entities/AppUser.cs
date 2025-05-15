using Microsoft.AspNetCore.Identity;

namespace FitFlare.Core.Entities;

public class AppUser : IdentityUser, IBaseEntity
{
    public string? FullName { get; set; }
    public string? ProfilePictureUri { get; set; }
    public bool IsBanned { get; set; }
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
    
    public ICollection<Post> Posts { get; set; }= [];
    public ICollection<Comment> Comments { get; set; }= [];      //user's comments
    public ICollection<CommentLike> CommentLikes { get; set; } = [];    //which comments user liked

    public ICollection<Story> Stories { get; set; } = [];
    
    public ICollection<Follow> Followers { get; set; } = [];   // people following ME
    public ICollection<Follow> Following { get; set; } = [];   // people I am following
}