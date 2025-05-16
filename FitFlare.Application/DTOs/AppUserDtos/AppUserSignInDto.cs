using FluentValidation;

namespace FitFlare.Application.DTOs.AppUserDTos;

public class AppUserSignInDto
{
    public required string EmailOrUserName { get; set; }
    public required string PassWord { get; set; }
}

public class AppUserSignInDtoValidator : AbstractValidator<AppUserSignInDto>
{
    public AppUserSignInDtoValidator()
    {
        RuleFor(m=>m.EmailOrUserName)
            .NotNull().WithMessage("Password can't be null")
            .NotEmpty().WithMessage("Password format is invalid.");
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