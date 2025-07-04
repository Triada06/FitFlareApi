﻿using FitFlare.Core.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FitFlare.Infrastructure.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : IdentityDbContext<AppUser>(options)
{
    public DbSet<Post> Posts { get; set; }
    public DbSet<Comment> Comments { get; set; }
    public DbSet<Notification> Notifications { get; set; }
    public DbSet<CommentLike> CommentLikes { get; set; }
    public DbSet<Story> Stories { get; set; }
    public DbSet<Follow> Follows { get; set; }
    public DbSet<Tag> Tags { get; set; }
    public DbSet<PostLike> PostLikes { get; set; }
    public DbSet<PostSave> PostSaves { get; set; }
    public DbSet<StoryView> StoryViews { get; set; }
    public DbSet<Message> Messages { get; set; }
    public DbSet<Ban> Bans { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<StoryView>()
            .HasKey(sv => new { sv.UserId, sv.StoryId });

        // Follow relationship
        builder.Entity<Follow>()
            .HasOne(f => f.Follower)
            .WithMany(u => u.Following)
            .HasForeignKey(f => f.FollowerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Follow>()
            .HasOne(f => f.Following)
            .WithMany(u => u.Followers)
            .HasForeignKey(f => f.FollowingId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Follow>()
            .HasIndex(f => new { f.FollowerId, f.FollowingId })
            .IsUnique();

        // PostLike relationship
        builder.Entity<PostLike>()
            .HasKey(pl => new { pl.UserId, pl.PostId });

        builder.Entity<PostLike>()
            .HasOne(pl => pl.User)
            .WithMany(u => u.LikedPosts)
            .HasForeignKey(pl => pl.UserId);

        builder.Entity<PostLike>()
            .HasOne(pl => pl.Post)
            .WithMany(p => p.LikedBy)
            .HasForeignKey(pl => pl.PostId);

        //Post Tag Join table 
        builder.Entity<Post>()
            .HasMany(p => p.Tags)
            .WithMany(t => t.Posts)
            .UsingEntity(j => j.ToTable("PostTags"));


        // PostSave relationship
        builder.Entity<PostSave>()
            .HasKey(ps => new { ps.UserId, ps.PostId });

        builder.Entity<PostSave>()
            .HasOne(ps => ps.User)
            .WithMany(u => u.SavedPosts)
            .HasForeignKey(ps => ps.UserId);

        builder.Entity<PostSave>()
            .HasOne(ps => ps.Post)
            .WithMany(p => p.SavedBy)
            .HasForeignKey(ps => ps.PostId);

        //comment self references
        builder.Entity<Comment>()
            .HasOne(c => c.ParentComment)
            .WithMany(c => c.Replies)
            .HasForeignKey(c => c.ParentCommentId)
            .OnDelete(DeleteBehavior.Cascade);

        //notification relation management
        builder.Entity<Notification>()
            .HasOne(n => n.TriggeredBy)
            .WithMany()
            .HasForeignKey(n => n.TriggeredById)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Notification>()
            .HasOne(n => n.Post)
            .WithMany(p => p.Notifications)
            .HasForeignKey(n => n.PostId)
            .OnDelete(DeleteBehavior.Cascade);


        // Property constraints
        builder.Entity<Comment>()
            .Property(m => m.Content)
            .HasMaxLength(150);

        builder.Entity<AppUser>()
            .Property(m => m.FullName)
            .HasMaxLength(30);

        builder.Entity<AppUser>()
            .Property(m => m.Bio)
            .HasMaxLength(150);

        builder.Entity<Post>()
            .Property(m => m.MediaType)
            .HasMaxLength(5);

        builder.Entity<Post>()
            .Property(m => m.Description)
            .HasMaxLength(150);

        builder.Entity<Post>()
            .Property(m => m.Status)
            .HasMaxLength(10);

        builder.Entity<Tag>()
            .HasIndex(x => x.Name)
            .IsUnique();
        builder.Entity<Tag>()
            .Property(x => x.Name)
            .HasMaxLength(50);
        builder.Entity<Ban>()
            .Property(x => x.Reason)
            .HasMaxLength(50);
    }
}