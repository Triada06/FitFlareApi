using System.Text;
using Azure.Storage.Blobs;
using DotNetEnv;
using FitFlare.Api;
using FitFlare.Application.Helpers;
using FitFlare.Application.Services;
using FitFlare.Application.Services.Interfaces;
using FitFlare.Core.Entities;
using FitFlare.Infrastructure.Data;
using FitFlare.Infrastructure.Repositories;
using FitFlare.Infrastructure.Repositories.Interfaces;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

Env.Load(); // Load .env file at startup

var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;

// Load secrets from .env

var dbConn = ResolveEnv(config.GetConnectionString("DefaultConnection"));
var blobConn = ResolveEnv(config.GetConnectionString("AzureBlobStorageConString"));
var blobContainer = ResolveEnv(config["BlobContainerName"]);
if (string.IsNullOrWhiteSpace(blobContainer))
    throw new Exception("BLOB_CONTAINER_NAME is missing in .env");
builder.Services.Configure<BlobStorageOptions>(options =>
    options.ContainerName = Environment.GetEnvironmentVariable("BLOB_CONTAINER_NAME")
);

// CORS Policy
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.AllowAnyOrigin() //  Change this in prod
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

// Swagger & API
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(opt =>
{
    opt.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Auth header using Bearer scheme. Example: 'Bearer {token}'",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    opt.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});


// Dependency Injection (split later into static if you want)
builder.Services.AddSingleton(_ => new BlobServiceClient(blobConn));
builder.Services.AddScoped<IBlobService, BlobService>();

builder.Services.AddScoped(typeof(IBaseRepository<>), typeof(BaseRepository<>));
builder.Services.AddScoped<IAppUserService, AppUserService>();
builder.Services.AddScoped<IAppUserRepository, AppUserRepository>();

// Validation
builder.Services.AddExceptionHandler<GlobalExceptionHandlerMiddleware>();
builder.Services.AddProblemDetails();
builder.Services.AddFluentValidationAutoValidation();
foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
    builder.Services.AddValidatorsFromAssembly(assembly);

// Database
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(dbConn)
);

// Identity Config
builder.Services.AddIdentityCore<AppUser>(options =>
    {
        options.Password.RequireDigit = true;
        options.Password.RequireLowercase = true;
        options.Password.RequireNonAlphanumeric = true;
        options.Password.RequireUppercase = true;
        options.Password.RequiredLength = 8;
        options.Password.RequiredUniqueChars = 1;

        options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
        options.User.RequireUniqueEmail = true;
    })
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();


var jwtSettings = config.GetSection("Jwt");

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings["Issuer"],
            ValidAudience = jwtSettings["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]))
        };
    });


builder.Services.AddAuthorization();
builder.Services.AddControllers();

var app = builder.Build();

// Middleware
app.UseStaticFiles();
app.UseRouting();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowFrontend");
app.UseAuthentication();
app.UseAuthorization();
app.UseHttpsRedirection();
app.UseExceptionHandler();
app.MapControllers();

app.Run();
return;

string ResolveEnv(string value) =>
    value.StartsWith("env:") ? Environment.GetEnvironmentVariable(value[4..]) : value;