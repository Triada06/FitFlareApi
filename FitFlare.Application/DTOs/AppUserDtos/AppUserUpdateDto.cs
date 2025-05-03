using FluentValidation;
using Microsoft.AspNetCore.Http;

namespace FitFlare.Application.DTOs.AppUserDTos;

public class AppUserUpdateDto
{
    public string? FullName { get; set; }
    public required string UserName { get; set; }
    public IFormFile? ProfilePicture { get; set; }
}

public class AppUserUpdateDtoValidator : AbstractValidator<AppUserUpdateDto>
{
    public AppUserUpdateDtoValidator()
    {
        RuleFor(model => model.FullName)
            .MaximumLength(30).WithMessage("Full Name must not exceed 30 characters.");
        RuleFor(model => model.UserName)
            .NotEmpty().WithMessage("Username must not be empty.")
            .NotNull().WithMessage("UserName must not be empty.")
            .MaximumLength(30).WithMessage("Username must not exceed 30 characters.");
    }
}