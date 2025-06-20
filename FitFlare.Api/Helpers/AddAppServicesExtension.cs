using Azure.Storage.Blobs;
using FitFlare.Api.BackgroundServices;
using FitFlare.Application.Services;
using FitFlare.Application.Services.Interfaces;
using FitFlare.Application.Services.Shared;
using FitFlare.Infrastructure.BackgroundServices;
using FitFlare.Infrastructure.Repositories;
using FitFlare.Infrastructure.Repositories.Interfaces;

namespace FitFlare.Api.Helpers;

public static class AddAppServicesExtension
{
    public static IServiceCollection AddAppServices(this IServiceCollection services)
    {
        //services and repositories
        services.AddScoped<IBlobService, BlobService>();
        services.AddScoped(typeof(IBaseRepository<>), typeof(BaseRepository<>));
        services.AddScoped<IAppUserService, AppUserService>();
        services.AddScoped<IAppUserRepository, AppUserRepository>();
        services.AddScoped<IPostService, PostService>();
        services.AddScoped<IPostRepository, PostRepository>();
        services.AddScoped<IPostLikeRepository, PostLikeRepository>();
        services.AddScoped<IPostSaveRepository, PostSaveRepository>();
        services.AddScoped<ITagRepository, TagRepository>();
        services.AddScoped<ITagService, TagService>();
        services.AddScoped<ICommentRepository, CommentRepository>();
        services.AddScoped<ICommentService, CommentService>();
        services.AddScoped<ICommentLikeRepository, CommentLikeRepository>();
        services.AddScoped<IFollowRepository, FollowRepository>();
        services.AddScoped<IFollowService, FollowService>();
        services.AddScoped<INotificationRepository, NotificationRepository>();
        services.AddScoped<INotificationService, NotificationService>();
        services.AddScoped<IStoryViewRepository, StoryViewRepository>();
        services.AddScoped<IStoryRepository, StoryRepository>();
        services.AddScoped<IStoryService, StoryService>();
        services.AddScoped<IMessageRepository, MessageRepository>();
        services.AddScoped<IMessageService, MessageService>();
        services.AddScoped<IChatService, ChatService>();
        services.AddScoped<IEmailService, EmailService>();
        services.AddScoped<IBanRepository, BanRepository>();
        services.AddScoped<IBanService, BanService>();
        //bg services
        services.AddHostedService<TempFileCleanupService>();
        services.AddHostedService<OldNotificationsCleanUpService>();
        services.AddHostedService<StoryCleanUpService>();

        return services;
    }
}