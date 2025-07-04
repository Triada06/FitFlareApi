﻿using FitFlare.Application.DTOs.Posts;

namespace FitFlare.Application.DTOs.AppUser;

public class AppUserDto
{
    public required string Id { get; set; }
    public required string UserName { get; set; }
    public required string Email { get; set; }
    public string? FullName { get; set; }
    public string? Bio { get; set; }
    public int PostsCount { get; set; }
    public bool IsBanned { get; set; }
    public bool IsPrivate { get; set; }
    public string? ProfilePictureUri { get; set; }
    public int FollowersCount { get; set; }
    public int FollowingCount { get; set; }
    public IEnumerable<PostDto?> Posts { get; set; } = [];
    public IEnumerable<PostDto?> SavedPosts { get; set; } = [];
}