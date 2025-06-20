using FitFlare.Application.DTOs.Ban;
using FitFlare.Application.DTOs.Posts;
using FitFlare.Core.Entities;

namespace FitFlare.Application.DTOs.Admin.UserDto;

public class ViewUserDto
{
    public required string Id { get; set; }
    public string? FullName { get; set; }
    public string? ProfilePictureUri { get; set; }
    public string? Bio { get; set; }
    public int FollowersCount { get; set; }
    public int FollowingCount { get; set; }
    public bool IsPrivate { get; set; }
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
    public IEnumerable<PostDto?> Posts { get; set; } = [];
    public IEnumerable<BanDto?> Bans { get; set; } = [];
}