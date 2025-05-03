using FitFlare.Core.Entities;
using FluentValidation;
using Microsoft.AspNetCore.Http;

namespace FitFlare.Application.DTOs.AppUserDTos;

public class AppUserCreateDto
{
    public string? FullName { get; set; }
    public required string UserName { get; set; }
    public required string Email { get; set; }
    public required string PassWord { get; set; }
    public IFormFile? ProfilePicture { get; set; }
}

public class AppUserCreateValidator : AbstractValidator<AppUserCreateDto>
{
    public AppUserCreateValidator()
    {
        RuleFor(model => model.FullName)
            .MaximumLength(30).WithMessage("Full Name must not exceed 30 characters.");
        RuleFor(model => model.UserName)
            .NotEmpty().WithMessage("Username must not be empty.")
            .NotNull().WithMessage("Username must not be empty.")
            .MaximumLength(30).WithMessage("Username must not exceed 30 characters.");
        RuleFor(model => model.Email)
            .NotNull().WithMessage("Email address can't be null")
            .NotEmpty().WithMessage("Email address can't be empty.")
            .EmailAddress().WithMessage("Email address format is invalid.");
        RuleFor(model => model.PassWord)
            .NotNull().WithMessage("Password can't be null")
            .NotEmpty().WithMessage("Password is required.")
            .MinimumLength(8).WithMessage("Password must be at least 8 characters long.")
            .Matches("[A-Z]").WithMessage("Password must contain at least one uppercase letter.")
            .Matches("[a-z]").WithMessage("Password must contain at least one lowercase letter.")
            .Matches("[0-9]").WithMessage("Password must contain at least one number.")
            .Matches("[^a-zA-Z0-9]").WithMessage("Password must contain at least one special character.");
    }
}