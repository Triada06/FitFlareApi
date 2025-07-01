using FluentValidation;

namespace FitFlare.Application.DTOs.AppUser;

public class AppUserSignUpDto
{
    public required string UserName { get; set; }
    public required string Email { get; set; }
    public required string PassWord { get; set; }
}

public class AppUserCreateValidator : AbstractValidator<AppUserSignUpDto>
{
    public AppUserCreateValidator()
    {
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
            .Matches("[0-9]").WithMessage("Password must contain at least one number.");
    }
}