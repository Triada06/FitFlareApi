using FluentValidation;
using Microsoft.AspNetCore.Http;

namespace FitFlare.Application.DTOs.AppUser;

public class AppUserUpdateDto
{
    public string? FullName { get; set; }
    public required string UserName { get; set; }
    public IFormFile? ProfilePicture { get; set; }
    public string? Bio { get; set; }
}

public class AppUserUpdateDtoValidator : AbstractValidator<AppUserUpdateDto>
{
    public AppUserUpdateDtoValidator()
    {
        RuleFor(model => model.FullName)
            .MaximumLength(30).WithMessage("Full Name must not exceed 30 characters.");
        RuleFor(model => model.UserName)
            .NotEmpty().WithMessage("Username must not be empty.")
            .MaximumLength(30).WithMessage("Username must not exceed 30 characters.");
        RuleFor(model=>model.Bio)
            .MaximumLength(150).WithMessage("Profile description must not exceed 150 characters.");
        
        When(model => model.ProfilePicture != null, () =>
        {
            RuleFor(model => model.ProfilePicture!.Length)
                .LessThanOrEqualTo(4 * 1024 * 1024)
                .WithMessage("Profile picture must not be larger than 4MB.");

            RuleFor(model => model.ProfilePicture!.ContentType)
                .Must(ct => ct is "image/jpeg" or "image/png" )
                .WithMessage("Only JPG, PNG  images are allowed.");
        });
    }
}