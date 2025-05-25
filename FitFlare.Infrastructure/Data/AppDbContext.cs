using FitFlare.Core.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FitFlare.Infrastructure.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : IdentityDbContext<AppUser>(options)
{
    public DbSet<Post> Posts { get; set; }
    public DbSet<Comment> Comments { get; set; }
    public DbSet<CommentLike> CommentLikes { get; set; }
    public DbSet<Story> Stories { get; set; }
    public DbSet<Follow> Follows { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Follow>()
            .HasOne(f => f.Follower)
            .WithMany(u => u.Following)     // “Following” is the nav on ApplicationUser
            .HasForeignKey(f => f.FollowerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Follow>()
            .HasOne(f => f.Following)
            .WithMany(u => u.Followers)     // “Followers” is the nav on ApplicationUser
            .HasForeignKey(f => f.FollowingId)
            .OnDelete(DeleteBehavior.Restrict);

        // Prevent duplicate follow records
        builder.Entity<Follow>()
            .HasIndex(f => new { f.FollowerId, f.FollowingId })
            .IsUnique();
        
        builder.Entity<Comment>()
            .Property(m=>m.Content).HasMaxLength(150);
        builder.Entity<AppUser>()
            .Property(m=>m.FullName).HasMaxLength(30);
        builder.Entity<Post>()
            .Property(m=>m.MediaType).HasMaxLength(5);
        builder.Entity<Post>()
            .Property(m=>m.Description).HasMaxLength(150);
        builder.Entity<Post>()
            .Property(m=>m.Status).HasMaxLength(10);
        
    }
}
