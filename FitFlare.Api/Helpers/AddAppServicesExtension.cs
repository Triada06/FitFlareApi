using Azure.Storage.Blobs;
using FitFlare.Api.BackgroundServices;
using FitFlare.Application.Services;
using FitFlare.Application.Services.Interfaces;
using FitFlare.Application.Services.Shared;
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
        
        //bg services
        services.AddHostedService<TempFileCleanupService>();

        return services;
    }
}