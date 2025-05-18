using Azure.Storage.Blobs;
using FitFlare.Application.Services;
using FitFlare.Application.Services.Interfaces;
using FitFlare.Infrastructure.Repositories;
using FitFlare.Infrastructure.Repositories.Interfaces;

namespace FitFlare.Api.Helpers;

public static class AddAppServicesExtension
{
    public static IServiceCollection AddAppServices(this IServiceCollection services)
    {
        services.AddScoped<IBlobService, BlobService>();
        services.AddScoped(typeof(IBaseRepository<>), typeof(BaseRepository<>));
        services.AddScoped<IAppUserService, AppUserService>();
        services.AddScoped<IAppUserRepository, AppUserRepository>();
        services.AddScoped<IPostService, PostService>();
        services.AddScoped<IPostRepository, PostRepository>();
        return services;
    }
}